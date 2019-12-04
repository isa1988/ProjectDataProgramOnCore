using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Web.Models
{
    public class ProjectUserModel
    {
        public bool IsAdd { get; set; }
        public bool IsDelete { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public ProjectUserStatus Status { get; set; }
        public int ProjectUserId { get; set; }

        public ProjectUserModel()
        {
            FullName = string.Empty;
            Email = string.Empty;
            if ((int)Status == 0)
                Status = ProjectUserStatus.Save;
        }
    }
}
