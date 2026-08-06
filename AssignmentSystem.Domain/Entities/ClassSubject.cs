using AssignmentSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities
{
    public class ClassSubject : BaseEntity
    {
        public Guid ClassId { get; set; }

        public ClassCourse Class { get; set; } = null!;

        public Guid SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;
    }
}
