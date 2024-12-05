using TaskManagementSystem.Application.Dto;

namespace TaskManagementSystem.Application.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllTasks();
        Task<bool> CreateTaskAsync(TaskDto task);
        Task<bool> UpdateTaskAsync(TaskDto task);
    }
}
