using AssignmentSystem.Domain.Common;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } 

        public Guid? ClassId { get; set; } 
        public bool IsActive { get; set; } = true;
        public ClassCourse? Class { get; set; }
        public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();

        public ICollection<Assignment> CreatedAssignments { get; set; } = new List<Assignment>();

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
