using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UM.Runtime.UMDataSystem.Abstract;
using UM.Runtime.UMFileUtility;
using UnityEngine;

namespace UM.Runtime.UMDataSystem.Impl
{
    /// <summary>
    /// This data handler will save data to Assets/Saves
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EncryptedDataHandler<T> : IDataHandler<T>
    {
        private const string EncodeKey = "2D4A614E645266556A586E3272357538";
        private readonly EncryptedFileReader _fileReader;
        private readonly EncryptedFileWriter _fileWriter;
        
        public EncryptedDataHandler(string userId, string fileName)
        {
            DataFileNameWithoutExtension = fileName;
            DataFileExtension = "json";
            DataPath = GeneratePath(userId, DataFileName);
            EnsureDirectory();
            _fileReader = new EncryptedFileReader(DataPath,EncodeKey);
            _fileWriter = new EncryptedFileWriter(DataPath,EncodeKey);
        }

        private string GeneratePath(string userId, string fileName)
        {
            return Path.Combine(Application.persistentDataPath,userId,fileName+".json");
        }
        
        public async UniTask<string> ReadData(CancellationToken token)
        {
            try
            {
                return await _fileReader.Read(token);
            }
            catch (Exception e)
            {
                throw new DataSystemException("Failed to read json value",e);
            }
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

        public async UniTask<T> ReadObject(CancellationToken token)
        {
            var jsonValue = await ReadData(token);
            try
            {
                return JsonUtility.FromJson<T>(jsonValue);
            }
            catch (Exception e)
            {
                throw new DataSystemException("Failed to cast json value to object type");
            }
        }

        public async UniTask<bool> WriteData(string serializedData, CancellationToken token)
        {
            try
            {
                return await _fileWriter.Write(serializedData, token);
            }
            catch (Exception e)
            {
                throw new DataSystemException("Failed to write json value",e);
            }
        }
        
        public async UniTask<bool> WriteData(T targetObject, CancellationToken token)
        {
            
            var jsonValue = JsonUtility.ToJson(targetObject);
            try
            {
                return await WriteData(jsonValue, token);
            }
            catch (Exception e)
            {
                throw new DataSystemException("Failed to cast json value to object type");
            }
        }
        public string DataPath { get; set; }
        public string DataFileName => DataFileNameWithoutExtension + "." + DataFileExtension;
        public string DataFileExtension { get;  set; }
        public string DataFileNameWithoutExtension { get;  set; }
    }
}