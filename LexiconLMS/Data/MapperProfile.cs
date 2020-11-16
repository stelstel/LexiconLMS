using AutoMapper;
using LexiconLMS.Models.Entities;
using LexiconLMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ModuleActivityCreateViewModel, Module>();

        }

    }
}
