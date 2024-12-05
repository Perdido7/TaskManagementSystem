using TaskManagementSystem.Application.Dto;

namespace TaskManagementSystem.Business.Sent
{
    public interface ISentService
    {
        Task CreateTaskAsync(TaskDto taskDto);
        Task UpdateTaskAsync(TaskDto taskDto);
    }
}
