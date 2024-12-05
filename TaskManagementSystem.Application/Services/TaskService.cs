using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.Dto;
using TaskManagementSystem.DataAccess;

namespace TaskManagementSystem.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork<DataAccess.Entities.Task> _unitOfWork;

        public readonly ILogger<TaskService> _logger;
        private readonly IMapper _mapper;

        public TaskService(
            IUnitOfWork<DataAccess.Entities.Task> unitOfWork,
            ILogger<TaskService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> CreateTaskAsync(TaskDto taskDto)
        {
            try
            {
                var taskEntity = _mapper.Map<DataAccess.Entities.Task>(taskDto);

                await _unitOfWork.InsertAsync(taskEntity);
                var result = await _unitOfWork.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a task.");
                throw;
            }
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasks()
        {
            try
            {
                var taskEntities = await _unitOfWork.GetManyAsync();

                return _mapper.Map<IEnumerable<TaskDto>>(taskEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all tasks.");
                throw;
            }
        }

        public async Task<bool> UpdateTaskAsync(TaskDto taskDto)
        {
            try
            {
                var taskEntity = await _unitOfWork.GetByIdAsync(taskDto.ID);

                if (taskEntity == null)
                {
                    _logger.LogWarning($"Task with ID {taskDto.ID} not found.");
                    return false;
                }

                _mapper.Map(taskDto, taskEntity);

                _unitOfWork.Update(taskEntity);
                var result = await _unitOfWork.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating the task with ID {taskDto.ID}.");
                throw;
            }
        }
    }
}
