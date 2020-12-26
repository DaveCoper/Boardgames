using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    public class MissionObjectives
    {
        public string Name { get; set; }

        public string MissionText { get; set; }

        public int NumberOfTasks { get; set; } = 0;

        public List<TaskCardModifier> TaskCardModifiers { get; set; } = new List<TaskCardModifier>();

        public List<string> SpecialRules { get; set; } = new List<string>();
    }
}