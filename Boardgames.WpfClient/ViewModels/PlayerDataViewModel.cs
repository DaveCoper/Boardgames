using System;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight;

namespace Boardgames.WpfClient.ViewModels
{
    public class PlayerDataViewModel : ViewModelBase
    {
        private readonly PlayerData playerData;

        public PlayerDataViewModel(PlayerData playerData)
        {
            this.playerData = playerData ?? throw new System.ArgumentNullException(nameof(playerData));
        }

        public Uri AvatarUri => playerData.AvatarUri;

        public string Name => playerData.Name;
    }
}