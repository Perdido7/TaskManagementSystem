using System.Text.Json;
using TaskManagementSystem.Application.Dto;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Business.RabbitMQ
{
    public class TaskCreateProcessor
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskCreateProcessor> _logger;

        public TaskCreateProcessor(ITaskService taskService, ILogger<TaskCreateProcessor> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task ProcessCreateTaskAsync(string message)
        {
            try
            {
                var taskDto = JsonSerializer.Deserialize<TaskDto>(message);

                if (taskDto == null)
                    throw new InvalidOperationException("Invalid message format.");

                await _taskService.CreateTaskAsync(taskDto);
                _logger.LogInformation($"Task {taskDto.ID} created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing create task message.");
            }
        }
    }
}
