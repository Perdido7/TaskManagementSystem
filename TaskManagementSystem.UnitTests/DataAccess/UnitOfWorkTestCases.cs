using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.DataAccess;

namespace TaskManagementSystem.UnitTests.DataAccess
{
    public class UnitOfWorkTestCases
    {
        private readonly DbContextOptions<TaskManagementSystemDbContext> _options;
        private readonly Mock<ILogger<UnitOfWork<TaskManagementSystem.DataAccess.Entities.Task>>> _loggerMock;

        public UnitOfWorkTestCases()
        {
            _options = new DbContextOptionsBuilder<TaskManagementSystemDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _loggerMock = new Mock<ILogger<UnitOfWork<TaskManagementSystem.DataAccess.Entities.Task>>>();
        }

        [Fact]
        public async Task InsertAsync_ShouldAddEntity()
        {
            using (var context = new TaskManagementSystemDbContext(_options))
            {
                context.Database.EnsureDeleted();

                var repository = new UnitOfWork<TaskManagementSystem.DataAccess.Entities.Task>(context, _loggerMock.Object);
                var entity = new TaskManagementSystem.DataAccess.Entities.Task
                {
                    Name = "Test",
                    Description = "Test",
                    AssignedTo = "Test",
                    Status = Common.Enums.Status.NotStarted
                };

                await repository.InsertAsync(entity);
                var saveResult = await repository.SaveChangesAsync();

                Assert.True(saveResult);
                Assert.Equal(1, await context.Set<TaskManagementSystem.DataAccess.Entities.Task>().CountAsync());
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            using (var context = new TaskManagementSystemDbContext(_options))
            {
                context.Database.EnsureDeleted();

                var repository = new UnitOfWork<TaskManagementSystem.DataAccess.Entities.Task>(context, _loggerMock.Object);
                var entity = new TaskManagementSystem.DataAccess.Entities.Task
                {
                    Name = "Test",
                    Description = "Test",
                    AssignedTo = "Test",
                    Status = Common.Enums.Status.NotStarted
                };

                await repository.InsertAsync(entity);
                await repository.SaveChangesAsync();
                var beforeUpdateEntity = await repository.GetManyAsync(x => x.ID == 1);
                var beforeUpdateStatus = beforeUpdateEntity[0].Status;

                entity.Status = Common.Enums.Status.InProgress;

                repository.Update(entity);
                await repository.SaveChangesAsync();
                var afterUpdateEntity = await repository.GetByIdAsync(1);
                var afterUpdateStatus = afterUpdateEntity.Status;

                Assert.NotNull(beforeUpdateEntity);
                Assert.NotNull(afterUpdateEntity);
                Assert.Equal(Common.Enums.Status.NotStarted, beforeUpdateStatus);
                Assert.Equal(Common.Enums.Status.InProgress, afterUpdateStatus);
            }
        }
    }
}
