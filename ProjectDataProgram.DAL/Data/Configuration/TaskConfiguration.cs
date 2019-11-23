using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectDataProgram.Core.DataBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.DAL.Data.Configuration
{
    class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);

            builder.HasOne(p => p.Author)
                .WithMany(t => t.TaskAuthors)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Executor)
                .WithMany(t => t.TaskExecutors)
                .HasForeignKey(p => p.ExecutorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Project)
                .WithMany(t => t.Tasks)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
