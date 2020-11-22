using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Boardgames.Shared.Services;

namespace Boardgames.Wpf.Client.Services
{
    internal class SecureStore : ISecureStore
    {
        private const string entropy = "ThisIsMagicKey";

        private readonly IFileStore fileStore;

        public SecureStore(IFileStore fileStore)
        {
            this.fileStore = fileStore ?? throw new ArgumentNullException(nameof(fileStore));
        }

        public async Task StoreStringAsync(string data, string filePath)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte[] toEncrypt = Encoding.UTF8.GetBytes(data);
            byte[] byteEntropy = Encoding.ASCII.GetBytes(entropy);

            // Encrypt the data and store the result in a new byte array. The original data remains unchanged.
            byte[] encryptedData = ProtectedData.Protect(toEncrypt, byteEntropy, DataProtectionScope.CurrentUser);

            await fileStore.WriteBytesAsync(filePath, encryptedData);
        }

        public async Task<string> ReadStringAsync(string filePath)
        {
            byte[] toDecrypt = await fileStore.ReadAllBytesAsync(filePath);
            byte[] byteEntropy = Encoding.ASCII.GetBytes(entropy);

            // Encrypt the data and store the result in a new byte array. The original data remains unchanged.
            byte[] data = ProtectedData.Unprotect(toDecrypt, byteEntropy, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(data);
        }
    }
}