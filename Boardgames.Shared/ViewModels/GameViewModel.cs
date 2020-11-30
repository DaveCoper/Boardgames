using System;
using Boardgames.Shared.Models;

namespace Boardgames.Shared.ViewModels
{
    public class GameViewModel
    {
        public GameType Type { get; set; }

        public string Name { get; set; }

        public Uri Icon32x32 { get; set; }

        public Uri Icon128x128 { get; set; }
    }
}