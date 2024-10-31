using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace UM.Runtime.UMFileUtility
{
    public class FileReaderException : Exception
    {
        public FileReaderException()
        {
        }

        protected FileReaderException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FileReaderException(string message) : base(message)
        {
        }

        public FileReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class EncryptedFileReader : IFileReader
    {
        private readonly string _encodeKey;
        private readonly string _filePath;

        public EncryptedFileReader(string filePath, string encodeKey)
        {
            _filePath = filePath;
            _encodeKey = encodeKey;
        }

        public async UniTask<string> Read(CancellationToken token)
        {
            if (!File.Exists(_filePath))
            {
                throw new FileReaderException($"Invalid file path: {_filePath}");
            }

            var byteKey = Convert.FromBase64String(_encodeKey);
            // Create new AES instance.
            using var oAes = Aes.Create();

            var outputIv = new byte[oAes.IV.Length];

            await using var dataStream = new FileStream(_filePath, FileMode.Open);

            try
            {
                var isCancelled = await dataStream.ReadAsync(outputIv, 0, outputIv.Length, token).AsUniTask(false).SuppressCancellationThrow();
                if (isCancelled.IsCanceled) return default;
            }
            catch (Exception e)
            {
                throw new FileReaderException($"Reading IV key failed",e);
            }

            try
            {
                await using var oStream = new CryptoStream(
                    dataStream,
                    oAes.CreateDecryptor(byteKey, outputIv),
                    CryptoStreamMode.Read);

                using var reader = new StreamReader(oStream);

                try
                {
                    var res = await reader.ReadToEndAsync().AsUniTask(false).AttachExternalCancellation(token).SuppressCancellationThrow();
                    return res.IsCanceled ? default : res.Result;
                }
                catch (Exception e)
                {
                    throw new FileReaderException($"Reading encrypted file failed",e);
                }
            }
            catch (Exception e)
            {
                throw new FileReaderException($"Reading encrypted file failed",e);
            }
        }
    }
}