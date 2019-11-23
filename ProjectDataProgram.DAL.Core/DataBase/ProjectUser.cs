using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Core.DataBase
{
    public class ProjectUser : Entity
    {
        public User User { get; set; }
        public int UserId { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}
