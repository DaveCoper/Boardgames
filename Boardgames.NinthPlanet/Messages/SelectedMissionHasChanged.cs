using Boardgames.Common.Messages;

namespace Boardgames.NinthPlanet.Messages
{
    public class SelectedMissionHasChanged : GameMessage
    {
        public int SelectedMission { get; set; }
    }
}