using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boardgames.WebServer.Models
{
    public class EntityBase : IEntity
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
