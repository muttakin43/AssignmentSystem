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
    public class ClassSubjectConfiguration : IEntityTypeConfiguration<ClassSubject>
    {
        public void Configure(EntityTypeBuilder<ClassSubject> builder)
        {
            builder.ToTable("ClassSubjects");

            builder.HasOne(cs => cs.Class)
                   .WithMany(c => c.ClassSubjects)
                   .HasForeignKey(cs => cs.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Subject)
                   .WithMany(s => s.ClassSubjects)
                   .HasForeignKey(cs => cs.SubjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(cs => new { cs.ClassId, cs.SubjectId }).IsUnique();
        }
    }
}
