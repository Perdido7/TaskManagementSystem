using TaskManagementSystem.Application.Dto;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Business.Sent
{
    public class SentService : ISentService
    {
        private readonly ServiceBusHandler _serviceBusHandler;

        public SentService(ITaskService taskService, ServiceBusHandler serviceBusHandler)
        {
            _serviceBusHandler = serviceBusHandler;
        }

        public async Task CreateTaskAsync(TaskDto taskDto)
        {
            await _serviceBusHandler.SendMessageAsync("TaskCreateQueue", taskDto);
        }

        public async Task UpdateTaskAsync(TaskDto taskDto)
        {
            await _serviceBusHandler.SendMessageAsync("TaskUpdateQueue", taskDto);
        }
    }
}
