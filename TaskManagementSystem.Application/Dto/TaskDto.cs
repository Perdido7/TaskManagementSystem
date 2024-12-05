using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Common.Enums;

namespace TaskManagementSystem.Application.Dto
{
    public class TaskDto
    {
        public int ID { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public Status Status { get; set; }

        public string? AssignedTo { get; set; }
    }
}
