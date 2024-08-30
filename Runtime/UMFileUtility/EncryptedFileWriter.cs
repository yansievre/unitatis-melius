using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace UM.Runtime.UMFileUtility
{
    public class FileWriterException : Exception
    {
        public FileWriterException()
        {
        }

        protected FileWriterException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FileWriterException(string message) : base(message)
        {
        }

        public FileWriterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    
    public class EncryptedFileWriter
    {
        private readonly string _encodeKey;
        private readonly string _filePath;

        public EncryptedFileWriter(string filePath, string encodeKey)
        {
            _filePath = filePath;
            _encodeKey = encodeKey;
        }
        
        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory ?? string.Empty);
        }

        public async UniTask<bool> Write(string text, CancellationToken token)
        {

            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_encodeKey);
                using (var dataStream = new FileStream(_filePath, FileMode.Create))
                {
                    try
                    {
                        await dataStream.WriteAsync(aes.IV, 0, aes.IV.Length, token).AsUniTask().SuppressCancellationThrow();
                        if (token.IsCancellationRequested)
                        {
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new FileWriterException("Failed to write IV", e);
                    }

                    using (var cryptoStream = new CryptoStream(
                        dataStream,
                        aes.CreateEncryptor(aes.Key, aes.IV),
                        CryptoStreamMode.Write
                    ))
                    {
                        using (var writer = new StreamWriter(cryptoStream))
                        {
                            try
                            {
                                await writer.WriteAsync(text);
                                await writer.FlushAsync().AsUniTask(false).AttachExternalCancellation(token).SuppressCancellationThrow();
                                if (token.IsCancellationRequested) return false;
                                return true;
                            }
                            catch (Exception e)
                            {
                                throw new FileWriterException("Writing encrypted file failed", e);
                            }
                        }
                        
                    }
            
                    
                }
                
            }
            return true;
        }

        public void RemoveTargetFile()
        {
            File.Delete(_filePath);
        }

    }
}