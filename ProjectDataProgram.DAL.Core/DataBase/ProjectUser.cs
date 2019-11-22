using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.DAL.Core.DataBase
{
    public class ProjectUser
    {
        public User User { get; set; }
        public int UserId { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}
