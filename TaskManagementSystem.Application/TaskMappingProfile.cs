using AutoMapper;
using TaskManagementSystem.Application.Dto;

namespace TaskManagementSystem
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<DataAccess.Entities.Task, TaskDto>().ReverseMap();
        }
    }
}
