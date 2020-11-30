using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels
{
    public class ScreenViewModel : ViewModelBase
    {
        public ScreenViewModel(string label) : base()
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new System.ArgumentException($"'{nameof(label)}' cannot be null or empty", nameof(label));
            }

            Label = label;
        }

        public ScreenViewModel(string label, IMessenger messenger) : base(messenger)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new System.ArgumentException($"'{nameof(label)}' cannot be null or empty", nameof(label));
            }

            Label = label;
        }

        public string Label { get; }
    }
}