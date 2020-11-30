using System;

namespace Boardgames.WebServer.Models
{
    public interface IEntity
    {
        DateTime CreatedAt { get; set; }
        int Id { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}