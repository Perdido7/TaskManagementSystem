using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Application.Dto;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.DataAccess;

namespace TaskManagementSystem.UnitTests.Services
{
    public class TaskServiceTestCases
    {
        private readonly Mock<IUnitOfWork<TaskManagementSystem.DataAccess.Entities.Task>> _unitOfWorkMock;
        private readonly Mock<ILogger<TaskService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly TaskService _taskService;

        public TaskServiceTestCases()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<TaskManagementSystem.DataAccess.Entities.Task>>();
            _loggerMock = new Mock<ILogger<TaskService>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaskManagementSystem.DataAccess.Entities.Task, TaskDto>().ReverseMap();
            });
            _mapper = configuration.CreateMapper();

            _taskService = new TaskService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnTrue_WhenTaskIsCreatedSuccessfully()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Name = "Test Task",
                Description = "Description",
                AssignedTo = "User",
                Status = Common.Enums.Status.NotStarted
            };

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _taskService.CreateTaskAsync(taskDto);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.InsertAsync(It.IsAny<TaskManagementSystem.DataAccess.Entities.Task>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturnAllTasks()
        {
            // Arrange
            var tasks = new List<TaskManagementSystem.DataAccess.Entities.Task>
            {
                new TaskManagementSystem.DataAccess.Entities.Task { ID = 1, Name = "Task 1", Description = "Desc 1", AssignedTo = "User1", Status = Common.Enums.Status.NotStarted },
                new TaskManagementSystem.DataAccess.Entities.Task { ID = 2, Name = "Task 2", Description = "Desc 2", AssignedTo = "User2", Status = Common.Enums.Status.Completed }
            };

            _unitOfWorkMock.Setup(u => u.GetManyAsync(It.IsAny<Expression<Func<TaskManagementSystem.DataAccess.Entities.Task, bool>>[]>()))
                           .ReturnsAsync(tasks.ToArray());

            // Act
            var result = await _taskService.GetAllTasks();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Task 1", result.First().Name);
            _unitOfWorkMock.Verify(u => u.GetManyAsync(It.IsAny<Expression<Func<TaskManagementSystem.DataAccess.Entities.Task, bool>>[]>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnTrue_WhenTaskIsUpdatedSuccessfully()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                ID = 1,
                Name = "Updated Task",
                Description = "Updated Description",
                AssignedTo = "Updated User",
                Status = Common.Enums.Status.InProgress
            };

            var existingTask = new TaskManagementSystem.DataAccess.Entities.Task
            {
                ID = 1,
                Name = "Original Task",
                Description = "Original Description",
                AssignedTo = "Original User",
                Status = Common.Enums.Status.NotStarted
            };

            _unitOfWorkMock.Setup(u => u.GetByIdAsync(taskDto.ID)).ReturnsAsync(existingTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskDto);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.GetByIdAsync(taskDto.ID), Times.Once);
            _unitOfWorkMock.Verify(u => u.Update(existingTask), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskDto = new TaskDto { ID = 999, Name = "Test", Description = "Test" };

            _unitOfWorkMock.Setup(u => u.GetByIdAsync(taskDto.ID)).ReturnsAsync((TaskManagementSystem.DataAccess.Entities.Task)null);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskDto);

            // Assert
            Assert.False(result);
            _unitOfWorkMock.Verify(u => u.GetByIdAsync(taskDto.ID), Times.Once);
            _unitOfWorkMock.Verify(u => u.Update(It.IsAny<TaskManagementSystem.DataAccess.Entities.Task>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
