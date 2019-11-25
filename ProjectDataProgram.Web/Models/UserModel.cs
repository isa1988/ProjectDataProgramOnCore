using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Web.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public ProjectUserStatus Status { get; set; }

        public UserModel()
        {
            FullName = string.Empty;
            Email = string.Empty;
            Status = ProjectUserStatus.Save;
        }
    }
}
