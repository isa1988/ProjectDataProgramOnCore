using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Core.DataBase
{
    public class Task : Entity
    {
        /// <summary>
        /// Наименование задачи
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        public User Author { get; set; }
        /// <summary>
        /// Автор
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public User Executor { get; set; }
        /// <summary>
        /// Исполнитель
        /// </summary>
        public int ExecutorId { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Проект
        /// </summary>
        public Project Project { get; set; }
        /// <summary>
        /// Проект
        /// </summary>
        public int ProjectId { get; set; }

        public Task()
        {
            Name = string.Empty;
            Comment = string.Empty;
        }
    }
}
