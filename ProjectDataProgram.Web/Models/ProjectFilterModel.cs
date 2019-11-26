using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDataProgram.Web.Models
{
    public class ProjectFilterModel
    {
        [DisplayName("Наименование")]
        public string Name { get; set; }
        [DisplayName("Наименование компании-исполнителя")]
        public string ContractorCompany { get; set; }
        [DisplayName("Наименование компании-заказчика")]
        public string CustomerCompany { get; set; }
        [DisplayName("Включить Приоритет в фильтр")]
        public bool IsPriority { get; set; }
        [DisplayName("Приоритет")]
        public int Priority { get; set; }
        [DisplayName("Искать по руководителю проекта")]
        public bool IsSpervisorUser { get; set; }
        [DisplayName("Руководитель проекта")]
        public SelectList SpervisorUser { get; set; }
        [DisplayName("Руководитель проекта")]
        public int? SpervisorUserId { get; set; }
        [DisplayName("Искать по исполнителю")]
        public bool IsUser { get; set; }
        [DisplayName("Исполнитель")]
        public SelectList User { get; set; }
        [DisplayName("Исполнитель")]
        public int? UserId { get; set; }
        [DisplayName("Искать по периоду")]
        public bool IsDatePeriod { get; set; }
        [DisplayName("Дата начала")]
        public DateTime DateBegin { get; set; }
        [DisplayName("Дата окончания")]
        public DateTime DateEnd { get; set; }

        public ProjectFilterModel()
        {
            Name = string.Empty;
            CustomerCompany = string.Empty;
            ContractorCompany = string.Empty;
            DateBegin = DateTime.Now;
            DateEnd = DateTime.Now;
        }
    }
}
