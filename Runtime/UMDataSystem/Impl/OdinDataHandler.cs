﻿#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;
using UM.Runtime.UMDataSystem.Abstract;
using UM.Runtime.UMFileUtility;
using UM.Runtime.UMUtility.Streams;
using UnityEngine;
using IDataReader = Sirenix.Serialization.IDataReader;
using IDataWriter = Sirenix.Serialization.IDataWriter;
using Object = UnityEngine.Object;

namespace UM.Runtime.UMDataSystem.Impl
{
    public class OdinDataHandler<T> : IDataHandler<T>
    {
        private const string K_EncodeKey = "2D4A614E645266556A586E3272357538";
        private const string K_SaveGameName = "Save_";
        private readonly IDataReader _fileReader;
        private readonly IDataWriter _fileWriter;
        
        public OdinDataHandler(int saveSlot, bool encryptionEnabled = false)
        {
            DataFileNameWithoutExtension = K_SaveGameName + saveSlot;
            DataFileExtension = "json";
            DataPath = GeneratePath();
            EnsureDirectory();
            
            
            Stream reader = new FileStream(DataPath, FileMode.OpenOrCreate, FileAccess.Read);
            if(encryptionEnabled) reader = new EncryptedStream(reader, K_EncodeKey, CryptoStreamMode.Read);
            _fileReader = SerializationUtility.CreateReader(reader,
                new DeserializationContext(new StreamingContext(StreamingContextStates.File)), DataFormat.Binary);
            
            Stream writer = new FileStream(DataPath, FileMode.OpenOrCreate, FileAccess.Write);
            if(encryptionEnabled) writer = new EncryptedStream(writer, K_EncodeKey, CryptoStreamMode.Write);
            _fileWriter = SerializationUtility.CreateWriter(writer,
                new SerializationContext(new StreamingContext(StreamingContextStates.File)), DataFormat.Binary);
            
        }

        private string GeneratePath()
        {
            return Path.Combine(Application.dataPath, DataFileName);
        }
        
        public UniTask<string> ReadData(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public DataState CheckFile()
        {
            return File.Exists(DataPath)?DataState.Found:DataState.NotFound;
        }

        private void EnsureDirectory()
        {
            var parentDirectory = Path.GetDirectoryName(DataPath);
            if(string.IsNullOrEmpty(parentDirectory)) throw new DataSystemException("Failed to get directory name from data path.");
            if (!Directory.Exists(parentDirectory)) Directory.CreateDirectory(parentDirectory);
        }

        public UniTask<T> ReadObject(CancellationToken token)
        {
            try
            {
                return UniTask.FromResult(SerializationUtility.DeserializeValue<T>(_fileReader));
            }
            catch (Exception e)
            {
                throw new DataSystemException("Failed to deserialize object", e);
            }
        }

        public async UniTask<bool> WriteData(string serializedData, CancellationToken token)
        {
            throw new NotImplementedException();
        }
        
        public UniTask<bool> WriteData(T targetObject, CancellationToken token)
        {
            try
            {
                SerializationUtility.SerializeValue(targetObject, _fileWriter, out var list);
                if(list.Count > 0) throw new DataSystemException("Failed to serialize object");

                return UniTask.FromResult<bool>(true);
            }
            catch (Exception e)
            {
                throw new DataSystemException("Failed to serialize object", e);

                return UniTask.FromResult<bool>(false);
            }
        }
        public string DataPath { get; set; }
        public string DataFileName => DataFileNameWithoutExtension + "." + DataFileExtension;
        public string DataFileExtension { get;  set; }
        public string DataFileNameWithoutExtension { get;  set; }
    }
}
#endif