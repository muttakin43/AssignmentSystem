using AssignmentSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities
{
    public class ClassCourse : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<User> Students { get; set; } = new List<User>();
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
        public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
