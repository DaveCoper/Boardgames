using Boardgames.Common.Messages;

namespace Boardgames.NinthPlanet.Messages
{
    public class TaskWasTaken : GameMessage
    {
        public int PlayerId { get; set; }

        public Models.TaskCard TaskCard { get; set; }
    }
}