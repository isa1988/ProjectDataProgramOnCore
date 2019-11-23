using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectDataProgram.Core.DataBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.DAL.Data.Configuration
{
    class ProjectUsersConfiguration : IEntityTypeConfiguration<ProjectUser>
    {
        public void Configure(EntityTypeBuilder<ProjectUser> builder)
        {
            builder.HasOne(p => p.Project)
                .WithMany(t => t.ProjectUsers)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.User)
                .WithMany(t => t.ProjectUsers)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
