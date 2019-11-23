using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace ProjectDataProgram.Core.DataBase
{
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
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


        /// <summary>
        /// Задачи где пользователь как автор
        /// </summary>
        public virtual ICollection<Task> TaskAuthors { get; set; }

        /// <summary>
        /// Задачи где пользователь как исполнитель
        /// </summary>
        public virtual ICollection<Task> TaskExecutors { get; set; }

        public User()
        {
            FullName = string.Empty;
            ProjectSupervisors = new List<Project>();
            ProjectUsers = new List<ProjectUser>();
            TaskAuthors = new List<Task>();
            TaskExecutors = new List<Task>();
        }
    }
}
