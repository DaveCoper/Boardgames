using System.Windows.Input;

namespace Boardgames.Wpf.Client.ViewModels
{
    public class MenuButtonViewModel
    {
        public MenuButtonViewModel(string label, ICommand command)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new System.ArgumentException($"'{nameof(label)}' cannot be null or empty", nameof(label));
            }

            Label = label;
            Command = command ?? throw new System.ArgumentNullException(nameof(command));
        }

        public string Label { get; }

        public ICommand Command { get; }
    }
}