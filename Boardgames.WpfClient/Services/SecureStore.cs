using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Boardgames.Client.Services;

namespace Boardgames.WpfClient.Services
{
    internal class SecureStore : ISecureStore
    {
        private const string entropy = "ThisIsMagicKey";

        private readonly IFileStore fileStore;

        public SecureStore(IFileStore fileStore)
        {
            this.fileStore = fileStore ?? throw new ArgumentNullException(nameof(fileStore));
        }

        public Task<Stream> ReadFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> WriteFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            return await ReadAllTextAsync(path, Encoding.UTF8);
        }

        public async Task<string> ReadAllTextAsync(string path, Encoding encoding)
        {
            var data = await ReadAllBytesAsync(path);
            return encoding.GetString(data);
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            byte[] toDecrypt = await fileStore.ReadAllBytesAsync(path);
            byte[] byteEntropy = Encoding.ASCII.GetBytes(entropy);

            // Encrypt the data and store the result in a new byte array. The original data remains unchanged.
            byte[] data = ProtectedData.Unprotect(toDecrypt, byteEntropy, DataProtectionScope.CurrentUser);
            return data;
        }

        public async Task WriteBytesAsync(string path, byte[] data)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty", nameof(path));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte[] byteEntropy = Encoding.ASCII.GetBytes(entropy);

            // Encrypt the data and store the result in a new byte array. The original data remains unchanged.
            byte[] encryptedData = ProtectedData.Protect(data, byteEntropy, DataProtectionScope.CurrentUser);
            await fileStore.WriteBytesAsync(path, encryptedData);
        }

        public async Task WriteTextAsync(string path, string data)
        {
            await WriteTextAsync(path, data, Encoding.UTF8);
        }

        public async Task WriteTextAsync(string path, string data, Encoding encoding)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte[] toEncrypt = encoding.GetBytes(data);
            await WriteBytesAsync(path, toEncrypt);
        }

        public async Task DeleteAsync(string path)
        {
            await this.fileStore.DeleteAsync(path);
        }
    }
}