using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Service
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            PointOfSaleMapping();
        }

        private void PointOfSaleMapping()
        {
            CreateMap<List<Project>, List<ProjectDto>>();
            CreateMap<List<ProjectDto>, List<Project>>();
        }
    }
}
