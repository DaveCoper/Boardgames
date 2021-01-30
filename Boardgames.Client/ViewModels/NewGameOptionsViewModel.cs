using System;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight;

namespace Boardgames.Client.ViewModels
{
    public class NewGameOptionsViewModel : ViewModelBase
    {
        private GameType type;

        private string name;

        private Uri icon32x32;

        private Uri icon128x128;

        public GameType Type
        {
            get => type;
            set => Set(ref type, value);
        }

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public Uri Icon32x32
        {
            get => icon32x32;
            set => Set(ref icon32x32, value);
        }

        public Uri Icon128x128
        {
            get => icon128x128;
            set => Set(ref icon128x128, value);
        }
    }
}