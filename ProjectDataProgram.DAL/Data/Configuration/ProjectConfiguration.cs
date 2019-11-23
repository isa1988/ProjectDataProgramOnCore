using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectDataProgram.Core.DataBase;

namespace ProjectDataProgram.DAL.Data.Configuration
{
    class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.ContractorCompany).IsRequired().HasMaxLength(150);
            builder.Property(p => p.CustomerCompany).IsRequired().HasMaxLength(150);

            builder.HasOne(p => p.SupervisorUser)
                .WithMany(t => t.ProjectSupervisors)
                .HasForeignKey(p => p.SupervisorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
