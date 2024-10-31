using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UM.Runtime.UMFileUtility
{
    public class FileReader : IFileReader
    {
        private readonly string _filePath;

        public FileReader(string filePath)
        {
            _filePath = filePath;
        }

        public async UniTask<string> Read(CancellationToken token)
        {
            if (!File.Exists(_filePath))
            {
                throw new FileReaderException($"Invalid file path: {_filePath}");
            }

            await using var fileStream = new FileStream(_filePath, FileMode.Open);

            using var streamReader = new StreamReader(fileStream);

            try
            {
                return await streamReader.ReadToEndAsync().AsUniTask(false)
                    .AttachExternalCancellation(token);
            }
            catch (OperationCanceledException e)
            {
                throw new FileReaderException($"Reading file cancelled", e);
            }
            catch (Exception e)
            {
                throw new FileReaderException($"Reading file failed", e);
            }
        }
    }
}