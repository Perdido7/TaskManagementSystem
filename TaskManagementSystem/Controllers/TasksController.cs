using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Dto;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Business.Sent;

namespace TaskManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ISentService _sentService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ISentService sentService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _sentService = sentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        /// <returns>List of tasks</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasks();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tasks.");
                return StatusCode(500, "An error occurred while fetching tasks.");
            }
        }

        /// <summary>
        /// Add a new task
        /// </summary>
        /// <param name="taskDto">Task to add</param>
        /// <returns>Result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddTask([FromBody] TaskDto taskDto)
        {
            try
            {
                if (taskDto == null)
                    return BadRequest("Task data is required.");

                await _sentService.CreateTaskAsync(taskDto);

                return CreatedAtAction(nameof(GetTasks), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a task.");
                return StatusCode(500, "An error occurred while creating the task.");
            }
        }

        /// <summary>
        /// Update task status
        /// </summary>
        /// <param name="taskDto">Task to update</param>
        /// <returns>Result of the operation</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskDto taskDto)
        {
            try
            {
                if (taskDto == null || id != taskDto.ID)
                    return BadRequest("Invalid task data or ID mismatch.");

                await _sentService.UpdateTaskAsync(taskDto);

                return Ok("Task status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating task with ID {id}.");
                return StatusCode(500, "An error occurred while updating the task.");
            }
        }
    }
}
