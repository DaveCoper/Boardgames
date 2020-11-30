using System;
using Boardgames.Common.Models;

namespace Boardgames.Client.ViewModels
{
    public class GameViewModel
    {
        public GameType Type { get; set; }

        public string Name { get; set; }

        public Uri Icon32x32 { get; set; }

        public Uri Icon128x128 { get; set; }
    }
}