using AssignmentSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string Action { get; set; } = string.Empty;      
        public string EntityName { get; set; } = string.Empty; 
        public Guid EntityId { get; set; }
        public string? Details { get; set; }
    }
}
