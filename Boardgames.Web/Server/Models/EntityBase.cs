using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boardgames.Web.Server.Models
{
    public class EntityBase
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
