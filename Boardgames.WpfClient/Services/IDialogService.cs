using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.WpfClient.Services
{
    public interface IDialogService
    {
        bool? ShowDialog(object viewModel);

        Task<bool?> ShowDialogAsync(object viewModel, CancellationToken cancellationToken = default);
    }
}