using AssignmentSystem.Domain.Common;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities
{
    public class Assignment : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Guid ClassId { get; set; }
        public ClassCourse Class { get; set; } = null!;

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public DateTime Deadline { get; set; }
        public decimal MaxMarks { get; set; }
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
        public bool AllowUpdateAfterSubmit { get; set; } = true;
       

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
