using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Service.Dtos
{
    public class ProjectFilterDto
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Наименование компании-исполнителя
        /// </summary>
        public string ContractorCompany { get; set; }
        
        /// <summary>
        /// Наименование компании-заказчика
        /// </summary>
        public string CustomerCompany { get; set; }
        
        /// <summary>
        /// Включить Приоритет в фильтр
        /// </summary>
        public bool IsPriority { get; set; }
        
        /// <summary>
        /// Приоритет
        /// </summary>
        public int? Priority { get; set; }
        
        /// <summary>
        /// Искать по руководителю проекта
        /// </summary>
        public bool IsSpervisorUser { get; set; }

        /// <summary>
        /// Руководитель проекта
        /// </summary>
        public int? SpervisorUserId { get; set; }
        
        /// <summary>
        /// Искать по исполнителю
        /// </summary>
        public bool IsUser { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Включить фильтр по дате
        /// </summary>
        public bool IsDatePeriod { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime DateBegin { get; set; }
        
        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime DateEnd { get; set; }

        public ProjectFilterDto()
        {
            Name = string.Empty;
            CustomerCompany = string.Empty;
            ContractorCompany = string.Empty;
            DateBegin = DateTime.Now;
            DateEnd = DateTime.Now;
        }
    }
}
