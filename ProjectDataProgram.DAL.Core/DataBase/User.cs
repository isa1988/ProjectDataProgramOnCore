using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace ProjectDataProgram.DAL.Core.DataBase
{
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// ФИО
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Проекты где пользователь ответственный
        /// </summary>
        public virtual ICollection<Project> ProjectSupervisors { get; set; }

        /// <summary>
        /// Проекты где пользователь как исполнитель
        /// </summary>
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }

        public User()
        {
            FullName = string.Empty;
            ProjectSupervisors = new List<Project>();
            ProjectUsers = new List<ProjectUser>();
        }
    }
}
