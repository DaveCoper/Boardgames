using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.Client.ViewModels
{
    public class ContentViewModel : ViewModelBase, IAsyncLoad
    {
        private readonly ILogger<ContentViewModel> logger;

        private bool loadingInProgress;


        public ContentViewModel(string label) : this(label, null)
        {
        }


        public ContentViewModel(string label, ILogger<ContentViewModel> logger) : base()
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentException($"'{nameof(label)}' must contain value!", nameof(label));
            }

            this.Title = label;
            this.logger = logger ?? NullLogger<ContentViewModel>.Instance;
        }

        public ContentViewModel(
            string label,
            IMessenger messenger,
            ILogger<ContentViewModel> logger)
            : base(messenger)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentException($"'{nameof(label)}' must contain value!", nameof(label));
            }

            this.Title = label;
            this.logger = logger ?? NullLogger<ContentViewModel>.Instance;
        }

        public bool LoadingInProgress
        {
            get => loadingInProgress;
            set => Set(ref loadingInProgress, value);
        }

        public string Title { get; }

        public async Task LoadDataAsync()
        {
            this.LoadingInProgress = true;
            try
            {
                await LoadDataInternalAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to load data in screen {0}.", this.GetType().FullName);
            }
            finally
            {
                this.LoadingInProgress = false;
            }
        }

        protected virtual Task LoadDataInternalAsync()
        {
            return Task.CompletedTask;
        }
    }
}