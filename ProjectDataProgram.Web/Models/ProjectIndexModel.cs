using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDataProgram.Web.Models
{
    public class ProjectIndexModel
    {
        public List<ProjectModl> ProjectList { get; set; }
        public ProjectFilterModel Filter { get; set; }

        public ProjectIndexModel()
        {
            ProjectList = new List<ProjectModl>();
            Filter = new ProjectFilterModel();
        }
    }
}
