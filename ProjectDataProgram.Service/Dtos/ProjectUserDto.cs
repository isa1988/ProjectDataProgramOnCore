using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Service.Dtos
{
    public class ProjectUserDto
    {
        public UserDto User { get; set; }
        public ProjectDto Project { get; set; }
        public ProjectUserStatus Status { get; set; }

        public ProjectUserDto()
        {
            Status = ProjectUserStatus.Save;
        }
    }

    public enum ProjectUserStatus : int
    {
        /// <summary>
        /// Добавлен
        /// </summary>
        New = 0,
        /// <summary>
        /// Уже есть в БД
        /// </summary>
        Save = 1,
        /// <summary>
        /// Удалить
        /// </summary>
        Delete = 2,
        /// <summary>
        /// Есть в БД но нет в проекте
        /// </summary>
        Free = 3
    }
}
