using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Service.Dtos
{
    public class ProjectTaskDto
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование задачи
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        public UserDto Author { get; set; }
        
        /// <summary>
        /// Исполнитель
        /// </summary>
        public UserDto Executor { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public StatusTask Status { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }
        
        /// <summary>
        /// Проект
        /// </summary>
        public ProjectDto Project { get; set; }

        public int Priority { get; set; }

        public ProjectTaskDto()
        {
            Name = string.Empty;
            Comment = string.Empty;
        }
    }

    public enum StatusTask : int
    {
        ToDo = 0,
        InProgress = 1,
        Done = 2
    }
}
