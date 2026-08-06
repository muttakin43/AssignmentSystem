using AssignmentSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities
{
    public class TeacherAssignment : BaseEntity

    {
        public Guid TeacherId { get; set; }

        public User Teacher { get; set; } = null!;

        public Guid ClassId { get; set; }

        public ClassCourse Class { get; set; } = null!;

        public Guid SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
