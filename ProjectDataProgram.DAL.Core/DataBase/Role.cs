using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.Core.DataBase
{
    public class Role : IdentityRole<int>
    {
        public Role(string name) : base(name)
        {
        }
    }
}
