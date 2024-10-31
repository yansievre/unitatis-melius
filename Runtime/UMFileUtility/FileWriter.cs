using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UM.Runtime.UMFileUtility
{
    public class FileWriter : IFileWriter
    {
        private readonly string _filePath;

        public FileWriter(string filePath)
        {
            _filePath = filePath;
        }

        public async UniTask<bool> Write(string text, CancellationToken token)
        {
            await using var streamWriter = new StreamWriter(_filePath, false);

            try
            {
                await streamWriter.WriteAsync(text).AsUniTask(false)
                    .AttachExternalCancellation(token);
                return true;
            }
            catch (OperationCanceledException e)
            {
                throw new FileReaderException($"Writing file cancelled", e);
            }
            catch (Exception e)
            {
                throw new FileReaderException($"Writing file failed", e);
            }
        }

        public void RemoveTargetFile()
        {
            File.Delete(_filePath);
        }
    }
}