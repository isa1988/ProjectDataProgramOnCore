using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Service.Dtos
{
    public class UserDto
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        public int ProjectUserId { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        public string EMail { get; set; }

        public ProjectUserStatus Status { get; set; }

        public UserDto()
        {
            Name = string.Empty;
            EMail = string.Empty;
            if ((int)Status == 0)
                Status = ProjectUserStatus.Save;
        }
    }

    public enum ProjectUserStatus
    {
        Save = 0,
        Free = 1,
        New = 2,
        Delete = 3
    }
}
