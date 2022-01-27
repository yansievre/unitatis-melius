using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UMLogger.Plugins.UMLogger.Interfaces;
using UnityEngine;

namespace Plugins.UMUtility
{
    public class UniTaskQueue
    {
        public enum TaskType
        {
            None,
            Prep,
            Task,
            Clean
        }
        private class QueueTaskWrapper
        {
            internal QueueTask QueueTask;
            internal bool IsComplete = false;

            internal UniTask GetAwaiter()
            {
                return UniTask.WaitUntil(() => IsComplete);
            }
        }
        
        public delegate UniTask PrepAction();
        public delegate UniTask CleanAction();
        public delegate UniTask QueueTask(CancellationToken token);
        public delegate UniTask QueueTask<in T>(CancellationToken token,T param1);
        public delegate UniTask QueueTask<in T, in K>(CancellationToken token,T param1, K param2);
        public delegate UniTask QueueTask<in T, in K,in J>(CancellationToken token,T param1, K param2, J param3);
        public delegate UniTask QueueTask<in T, in K,in J, in M>(CancellationToken token,T param1, K param2, J param3, M param4);

        private Queue<QueueTaskWrapper> _taskQueue;
        private bool _isEnabled = false;
        private bool _isPaused = false;
        private CancellationTokenSource _cts;
        private IUMLogger _logger;
        private TaskType _currentTask = TaskType.None;


        #region Actions

        private PrepAction _prepAction;
        private CleanAction _cleanAction;

        #endregion
        
        private UniTaskQueue(IUMLogger logger)
        {
            _taskQueue = new Queue<QueueTaskWrapper>();
            _logger = logger;
        }

        /// <summary>
        /// If the UniTaskQueue is enabled, it will start going through tasks as soon as they're available
        /// </summary>
        public bool IsEnabled => _isEnabled;

        /// <summary>
        /// True if there is currently a running task
        /// </summary>
        public bool IsRunning => _currentTask!=TaskType.None;
        /// <summary>
        /// True if there is currently a running task
        /// </summary>
        public bool IsRunningPrepAction => _currentTask==TaskType.Prep;
        /// <summary>
        /// True if there is currently a running task
        /// </summary>
        public bool IsRunningTaskAction => _currentTask==TaskType.Task;
        /// <summary>
        /// True if there is currently a running task
        /// </summary>
        public bool IsRunningCleanAction => _currentTask==TaskType.Clean;

        /// <summary>
        /// Is the queue paused
        /// </summary>
        public bool IsPaused => _isPaused;
        /// <summary>
        /// Amount of tasks currently in the queue, the currently active one is excluded.
        /// </summary>
        public int Count => _taskQueue.Count;

        /// <summary>
        /// Amount of tasks currently in the queue and the currently active task
        /// </summary>
        public int CountWithActiveTask => _taskQueue.Count + (IsRunningTaskAction ? 1 : 0);

        /// <summary>
        /// Will start going through the queue. If there are actively running prep/clean actions won't be able to start.
        /// </summary>
        public void Start()
        {
            if (IsRunning) return;
            _isEnabled = true;
            AttemptStart_Internal();
        }


        /// <summary>
        /// Waits until the sequence is no longer running to start again
        /// </summary>
        public async UniTask StartWhenAble()
        {
            await UniTask.WaitUntil(() => !IsRunning);
            Start();
        }

        /// <summary>
        /// Called before the start of each sequence
        /// </summary>
        /// <param name="prepAction"></param>
        public void AddBeforeEachSequencePrepAction(PrepAction prepAction)
        {
            _prepAction += prepAction;
        }

        /// <summary>
        /// Called when a sequence is fully complete and there are no more actions remaining in the queue
        /// </summary>
        /// <param name="cleanAction"></param>
        public void AddAfterSequenceOverAction(CleanAction cleanAction)
        {
            _cleanAction = cleanAction;
        }
        
        /// <summary>
        /// Adds a task to the queue 
        /// </summary>
        /// <param name="task">An awaiter for the completion of the specific task</param>
        /// <returns></returns>
        public UniTask Enqueue(QueueTask task)
        {
            var wrapper = new QueueTaskWrapper()
            {
                QueueTask = task,
                IsComplete = false
            };
            _taskQueue.Enqueue(wrapper);
            
            if (!IsEnabled) return wrapper.GetAwaiter();
            if (IsRunning) return wrapper.GetAwaiter();
            AttemptStart_Internal();
            return wrapper.GetAwaiter();
        }
        
        /// <summary>
        /// Adds a task to the queue 
        /// </summary>
        /// <param name="task">An awaiter for the completion of the specific task</param>
        /// <returns></returns>
        public UniTask Enqueue<T>(QueueTask<T> task,T param1)
        {
            var wrapper = new QueueTaskWrapper()
            {
                QueueTask = async (CancellationToken token) =>
                {
                    await task.Invoke(token, param1);
                },
                IsComplete = false
            };
            _taskQueue.Enqueue(wrapper);
            if (!IsEnabled) return wrapper.GetAwaiter();
            if (IsRunning) return wrapper.GetAwaiter();
            AttemptStart_Internal();
            return wrapper.GetAwaiter();
        }
        
        /// <summary>
        /// Adds a task to the queue 
        /// </summary>
        /// <param name="task">An awaiter for the completion of the specific task</param>
        /// <returns></returns>
        public UniTask Enqueue<T,K>(QueueTask<T,K> task,T param1,K param2)
        {
            var wrapper = new QueueTaskWrapper()
            {
                QueueTask = async (CancellationToken token) =>
                {
                    await task.Invoke(token, param1,param2);
                },
                IsComplete = false
            };
            _taskQueue.Enqueue(wrapper);
            if (!IsEnabled) return wrapper.GetAwaiter();
            if (IsRunning) return wrapper.GetAwaiter();
            AttemptStart_Internal();
            return wrapper.GetAwaiter();
        }
        
        /// <summary>
        /// Adds a task to the queue 
        /// </summary>
        /// <param name="task">An awaiter for the completion of the specific task</param>
        /// <returns></returns>
        public UniTask Enqueue<T,K,J>(QueueTask<T,K,J> task,T param1,K param2,J param3)
        {
            var wrapper = new QueueTaskWrapper()
            {
                QueueTask = async (CancellationToken token) =>
                {
                    await task.Invoke(token, param1,param2,param3);
                },
                IsComplete = false
            };
            _taskQueue.Enqueue(wrapper);
            if (!IsEnabled) return wrapper.GetAwaiter();
            if (IsRunning) return wrapper.GetAwaiter();
            AttemptStart_Internal();
            return wrapper.GetAwaiter();
        }
        
        /// <summary>
        /// Adds a task to the queue 
        /// </summary>
        /// <param name="task">An awaiter for the completion of the specific task</param>
        /// <returns></returns>
        public UniTask Enqueue<T,K,J,M>(QueueTask<T,K,J,M> task,T param1,K param2,J param3,M param4)
        {
            var wrapper = new QueueTaskWrapper()
            {
                QueueTask = async (CancellationToken token) =>
                {
                    await task.Invoke(token, param1,param2,param3,param4);
                },
                IsComplete = false
            };
            _taskQueue.Enqueue(wrapper);
            if (!IsEnabled) return wrapper.GetAwaiter();
            if (IsRunning) return wrapper.GetAwaiter();
            AttemptStart_Internal();
            return wrapper.GetAwaiter();
        }

        /// <summary>
        /// Clears the remaining queue but does not stop the current task from running
        /// </summary>
        public void ClearQueue()
        {
            _taskQueue.Clear();
        }

        /// <summary>
        /// Pauses the queue but does not stop the current task from running
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Continues a paused queue
        /// </summary>
        public void Continue()
        {
            _isPaused = false;
            AttemptStart_Internal();
        }

        /// <summary>
        /// Kills the sequence, all tasks will be cleared and the currently active one will be cancelled
        /// </summary>
        public void Kill()
        {
            _isEnabled = false;
            _isPaused = false;
            _taskQueue.Clear();
            _cts?.Cancel();
        }
        
        private void AttemptStart_Internal()
        {
            if (!IsEnabled) return;
            if (IsRunning||IsPaused) return;
            if (_taskQueue.Count == 0) return;
            EnactQueue();
        }
        

        // ReSharper disable Unity.PerformanceAnalysis
        private async UniTask EnactQueue()
        {
            if (_prepAction != null)
            {
                _currentTask = TaskType.Prep;
                await _prepAction();
            }

            _currentTask = TaskType.Task;
            while (IsEnabled && !IsPaused)
            {
                if (_taskQueue.Count == 0) break;
                var current = _taskQueue.Dequeue();
                _cts = new CancellationTokenSource();
                bool isCancelled = true;
                try
                {
                    isCancelled = await current.QueueTask(_cts.Token).AttachExternalCancellation(_cts.Token)
                        .SuppressCancellationThrow();
                }
                catch (Exception e)
                {
                    _logger.LogError("An error has occured while executing a task in UniTaskQueue");
                    _logger.LogException(e);
                }

                _cts = null;
                if (isCancelled) break;
            }

            if (_cleanAction != null)
            {
                _currentTask = TaskType.Clean;
                await _cleanAction();
            }
            _currentTask = TaskType.None;
        }
    }
}
