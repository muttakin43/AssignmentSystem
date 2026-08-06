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
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");

            builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(a => a.Class)
                   .WithMany(c => c.Assignments)
                   .HasForeignKey(a => a.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Subject)
                   .WithMany(s => s.Assignments)
                   .HasForeignKey(a => a.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Teacher)
                   .WithMany(u => u.CreatedAssignments)
                   .HasForeignKey(a => a.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
