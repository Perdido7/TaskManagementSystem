using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Common.Enums;

namespace TaskManagementSystem.DataAccess.Entities
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public required string Description { get; set; }

        public Status Status { get; set; }

        public string? AssignedTo { get; set; }
    }
}
