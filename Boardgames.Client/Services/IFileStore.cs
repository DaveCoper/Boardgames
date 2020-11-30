using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Client.Services
{
    public interface IFileStore
    {
        Task<Stream> ReadFileAsync(string path);

        Task<string> ReadAllTextAsync(string path);

        Task<string> ReadAllTextAsync(string path, Encoding encoding);

        Task<byte[]> ReadAllBytesAsync(string path);

        Task<Stream> WriteFileAsync(string path);

        Task WriteBytesAsync(string path, byte[] data);

        Task WriteTextAsync(string path, string data);

        Task WriteTextAsync(string path, string data, Encoding encoding);

        Task DeleteAsync(string path);
    }
}