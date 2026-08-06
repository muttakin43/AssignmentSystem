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
    public class ClassCourseConfiguration : IEntityTypeConfiguration<ClassCourse>
    {
        public void Configure(EntityTypeBuilder<ClassCourse> builder)
        {
            builder.ToTable("Classes"); 
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        }
    }
}
