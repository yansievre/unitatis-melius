using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UM.Runtime.UMFileUtility;

namespace UM.Runtime.UMUtility.Streams
{
    public class EncryptedStream : Stream
    {
        private readonly CryptoStreamMode _streamMode;
        private Stream _wrappedBaseStream;
        private CryptoStream _cryptoStream;

        public EncryptedStream(Stream baseStream, string encodeKey, CryptoStreamMode streamMode)
        {
            _wrappedBaseStream = baseStream;
            _streamMode = streamMode;
            var byteKey = Convert.FromBase64String(encodeKey);
            // Create new AES instance.
            using var oAes = Aes.Create();

            var outputIv = new byte[oAes.IV.Length];
            
            try
            {
                var returnValue = baseStream.Read(outputIv, 0, outputIv.Length);
                if (returnValue != outputIv.Length)
                {
                    throw new FileReaderException($"Reading IV key failed");
                }
            }
            catch (Exception e)
            {
                throw new FileReaderException($"Reading IV key failed",e);
            }

            _cryptoStream = new CryptoStream(
                baseStream,
                _streamMode == CryptoStreamMode.Read ? oAes.CreateDecryptor(byteKey, outputIv) : oAes.CreateEncryptor(byteKey, outputIv),
                _streamMode);
        }
        
        public override void Flush()
        {
            _cryptoStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _cryptoStream.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _cryptoStream.Read(buffer, offset, count);
        }
        
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
        {
            var byteArray = buffer.ToArray();
            var readBytes = await _cryptoStream.ReadAsync(byteArray, 0, byteArray.Length, cancellationToken);
            buffer = new Memory<byte>(byteArray);
            return readBytes;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            _cryptoStream.SetLength(value);
        }

        
        public override void Write(byte[] buffer, int offset, int count)
        {
            _cryptoStream.Write(buffer, offset, count);
        }
        
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
        {
            var byteArray = buffer.ToArray();
            await _cryptoStream.WriteAsync(byteArray, 0, byteArray.Length, cancellationToken);
        }

        public override bool CanRead => _streamMode == CryptoStreamMode.Read;

        public override bool CanSeek => false;

        public override bool CanWrite => _streamMode == CryptoStreamMode.Write;

        public override long Length => _cryptoStream.Length;

        public override long Position
        {
            get => _cryptoStream.Position;
            set => _cryptoStream.Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            _cryptoStream.Dispose();
            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await _cryptoStream.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}