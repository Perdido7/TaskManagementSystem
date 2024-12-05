using System.Text.Json;
using TaskManagementSystem.Application.Dto;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Business.RabbitMQ
{
    public class TaskUpdateProcessor
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskUpdateProcessor> _logger;

        public TaskUpdateProcessor(ITaskService taskService, ILogger<TaskUpdateProcessor> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task ProcessUpdateTaskAsync(string message)
        {
            try
            {
                var taskDto = JsonSerializer.Deserialize<TaskDto>(message);

                if (taskDto == null)
                    throw new InvalidOperationException("Invalid message format.");

                var updated = await _taskService.UpdateTaskAsync(taskDto);
                if (updated)
                {
                    _logger.LogInformation($"Task {taskDto.ID} updated to {taskDto.Status}.");
                }
                else
                {
                    _logger.LogWarning($"Task {taskDto.ID} not found for update.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing update task message.");
            }
        }
    }
}
