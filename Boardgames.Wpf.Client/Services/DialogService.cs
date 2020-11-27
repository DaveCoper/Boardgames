using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Boardgames.Wpf.Client.ViewModels;

namespace Boardgames.Wpf.Client.Services
{
    public class DialogService : IDialogService
    {
        private readonly Dictionary<Type, Type> dialogs;

        public DialogService()
        {
            dialogs = new Dictionary<Type, Type>()
            { {typeof(LoginViewModel), typeof(LoginWindow) } };
        }

        public bool? ShowDialog(object viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var vmType = viewModel.GetType();

            while (vmType != typeof(object) && vmType != null)
            {
                if (dialogs.TryGetValue(vmType, out var windowType))
                {
                    var window = (Window)Activator.CreateInstance(windowType);
                    window.DataContext = viewModel;
                    return window.ShowDialog();
                }

                vmType = vmType.BaseType;
            }

            throw new NotImplementedException();
        }

        public Task<bool?> ShowDialogAsync(object viewModel, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ShowDialog(viewModel));
        }
    }
}