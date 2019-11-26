using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Service.Dtos
{
    public class ProjectDto
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование проекта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Наименование компании-заказчика
        /// </summary>
        public string CustomerCompany { get; set; }

        /// <summary>
        /// Наименование компании-исполнителя
        /// </summary>
        public string ContractorCompany { get; set; }

        /// <summary>
        /// Руководитель проекта
        /// </summary>
        public UserDto SupervisorUser { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Исполнители
        /// </summary>
        public virtual List<ProjectUserDto> ProjectUsers { get; set; }

        /// <summary>
        /// Задания
        /// </summary>
        public virtual List<ProjectTaskDto> ProjectTasks { get; set; }

        public ProjectDto()
        {
            Name = string.Empty;
            CustomerCompany = string.Empty;
            ContractorCompany = string.Empty;
            ProjectUsers = new List<ProjectUserDto>();
            ProjectTasks = new List<ProjectTaskDto>();
        }
    }
}
