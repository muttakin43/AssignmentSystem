using AssignmentSystem.Domain.Common;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities
{
    
        public class Submission : BaseEntity
        {
            public Guid AssignmentId { get; set; }
            public Assignment Assignment { get; set; } = null!;
            public Guid StudentId { get; set; }
            public User Student { get; set; } = null!;
            public string? TextAnswer { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public string? ContentType { get; set; }
            public long? FileSizeBytes { get; set; }
            public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
            public decimal? MarksObtained { get; set; }
            public string? Feedback { get; set; }
            public DateTime SubmittedAtUtc { get; set; }
          
            public DateTime? GradedAtUtc { get; set; }
            public Guid? GradedByTeacherId { get; set; }
            public User? GradedByTeacher { get; set; }
        }
}
