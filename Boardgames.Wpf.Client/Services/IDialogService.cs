using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.Wpf.Client.Services
{
    public interface IDialogService
    {
        bool? ShowDialog(object viewModel);

        Task<bool?> ShowDialogAsync(object viewModel, CancellationToken cancellationToken = default);
    }
}