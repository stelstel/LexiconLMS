using AutoMapper;
using LexiconLMS.Models.Entities;
using LexiconLMS.Models.ViewModels;


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
