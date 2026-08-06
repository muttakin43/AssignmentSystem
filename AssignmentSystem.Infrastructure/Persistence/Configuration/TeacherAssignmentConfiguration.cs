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
    public class TeacherAssignmentConfiguration : IEntityTypeConfiguration<TeacherAssignment>
    {
        public void Configure(EntityTypeBuilder<TeacherAssignment> builder)
        {
            builder.ToTable("TeacherAssignments");

            builder.HasOne(ta => ta.Teacher)
                   .WithMany(u => u.TeacherAssignments)
                   .HasForeignKey(ta => ta.TeacherId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ta => ta.Class)
                   .WithMany(c => c.TeacherAssignments)
                   .HasForeignKey(ta => ta.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ta => ta.Subject)
                   .WithMany(s => s.TeacherAssignments)
                   .HasForeignKey(ta => ta.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
