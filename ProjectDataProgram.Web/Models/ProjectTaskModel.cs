using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Web.Models
{
    public class ProjectTaskModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование задачи
        /// </summary>
        [Required(ErrorMessage = "Наименование не должно быть пустым")]
        [DisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        [DisplayName("Автор")]
        public SelectList AuthorList { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        [DisplayName("Автор")]
        public int AuthorId { get; set; }
        /// <summary>
        /// Автор Имя
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        [Required(ErrorMessage = "Исполнитель не должнен быть пустым")]
        [DisplayName("Исполнитель")]
        public SelectList ExecutorList { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        [Required(ErrorMessage = "Исполнитель не должнен быть пустым")]
        [DisplayName("Исполнитель")]
        public int ExecutorId { get; set; }

        /// <summary>
        /// Исполнитель имя
        /// </summary>
        public string ExecutorName { get; set; }

        public SelectList StatusList { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        [Required(ErrorMessage = "Статус не должнен быть пустым")]
        [DisplayName("Статус")]
        public StatusTask Status { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }
        
        /// <summary>
        /// Проект
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        [Range(typeof(int), "1", "5")]
        [DisplayName("Приоритет")]
        public int Priority { get; set; }

        public ProjectTaskModel()
        {
            Name = string.Empty;
            Comment = string.Empty;
            var statusList = new List<StatusTaskModel>();
            statusList.Add(new StatusTaskModel(StatusTask.ToDo));
            statusList.Add(new StatusTaskModel(StatusTask.InProgress));
            statusList.Add(new StatusTaskModel(StatusTask.Done));
            StatusList = new SelectList(statusList, "Id", "Name");
        }
    }

    public class StatusTaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public StatusTaskModel()
        {
            Name = string.Empty;
        }

        public StatusTaskModel(StatusTask status)
        {
            Id = (int)status;
            Name = status.ToString();
        }
    }
}
