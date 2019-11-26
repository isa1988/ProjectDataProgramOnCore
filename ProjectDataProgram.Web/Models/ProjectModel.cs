using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjectDataProgram.Web.Models
{
    public class ProjectModl
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование проекта
        /// </summary>
        [Required(ErrorMessage = "Наименование не должно быть пустым")]
        [DisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Наименование компании-заказчика
        /// </summary>
        [Required(ErrorMessage = "Наименование компании-заказчика не должно быть пустым")]
        [DisplayName("Наименование компании-заказчика")]
        public string CustomerCompany { get; set; }

        /// <summary>
        /// Наименование компании-исполнителя
        /// </summary>
        [Required(ErrorMessage = "Наименование компании-исполнителя не должно быть пустым")]
        [DisplayName("Наименование компании-исполнителя")]
        public string ContractorCompany { get; set; }

        /// <summary>
        /// Руководитель проекта
        /// </summary>
        [Required(ErrorMessage = "Руководитель проекта не должнен быть пустым")]
        [DisplayName("Руководитель проекта")]
        public int SupervisorUserId { get; set; }

        /// <summary>
        /// Руководитель проекта имя
        /// </summary>
        [DisplayName("Руководитель проекта")]
        public string SupervisorUserName { get; set; }

        /// <summary>
        /// Руководители проекта
        /// </summary>
        [DisplayName("Руководитель проекта")]
        public SelectList SupervisorUserList { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        [Required(ErrorMessage = "Дата начала не должна быть пустым")]
        [DataType(DataType.Date)]
        [DisplayName("Дата начала")]
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        [Required(ErrorMessage = "Дата окончания не должна быть пустым")]
        [DataType(DataType.Date)]
        [DisplayName("Дата окончания")]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        [Range(typeof(int), "1", "5")]
        [DisplayName("Приоритет")]
        public int Priority { get; set; }

        /// <summary>
        /// Исполнители
        /// </summary>
        public virtual List<ProjectUserModel> ProjectUsers { get; set; }

        /// <summary>
        /// Задания
        /// </summary>
        public virtual List<ProjectTaskModel> ProjectTasks { get; set; }

        public ProjectModl()
        {
            Name = string.Empty;
            CustomerCompany = string.Empty;
            ContractorCompany = string.Empty;
            ProjectUsers = new List<ProjectUserModel>();
            ProjectTasks = new List<ProjectTaskModel>();
        }
    }
}
