using System.IO;
using System.Text;
using System.Threading.Tasks;
using Boardgames.Shared.Services;

namespace Boardgames.Wpf.Client.Services
{
    public class WpfFileStore : IFileStore
    {
        public Task DeleteAsync(string path)
        {
            File.Delete(path);
            return Task.CompletedTask;
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public async Task<string> ReadAllTextAsync(string path, Encoding encoding)
        {
            return await File.ReadAllTextAsync(path, encoding);
        }

        public Task<Stream> ReadFileAsync(string path)
        {
            return Task.FromResult<Stream>(File.OpenRead(path));
        }

        public async Task WriteBytesAsync(string path, byte[] data)
        {
            await File.WriteAllBytesAsync(path, data);
        }

        public Task<Stream> WriteFileAsync(string path)
        {
            return Task.FromResult<Stream>(File.OpenWrite(path));
        }

        public async Task WriteTextAsync(string path, string data)
        {
            await File.WriteAllTextAsync(path, data);
        }

        public async Task WriteTextAsync(string path, string data, Encoding encoding)
        {
            await File.WriteAllTextAsync(path, data, encoding);
        }
    }
}