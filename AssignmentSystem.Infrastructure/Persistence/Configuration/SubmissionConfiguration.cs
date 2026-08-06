using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Infrastructure.Persistence.Configuration
{
    public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.ToTable("Submissions");

            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.MarksObtained).HasColumnType("decimal(6,2)");

            builder.HasOne(s => s.Assignment)
                   .WithMany(a => a.Submissions)
                   .HasForeignKey(s => s.AssignmentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Student)
                   .WithMany(u => u.Submissions)
                   .HasForeignKey(s => s.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.GradedByTeacher)
                   .WithMany()
                   .HasForeignKey(s => s.GradedByTeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

           
            builder.HasIndex(s => new { s.AssignmentId, s.StudentId }).IsUnique();
        }
    }
}
