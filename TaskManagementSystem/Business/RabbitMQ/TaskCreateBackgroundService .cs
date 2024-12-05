namespace TaskManagementSystem.Business.RabbitMQ
{
    public class TaskCreateBackgroundService : BackgroundService
    {
        private readonly ServiceBusHandler _serviceBusHandler;
        private readonly TaskCreateProcessor _taskCreateProcessor;

        public TaskCreateBackgroundService(ServiceBusHandler serviceBusHandler, TaskCreateProcessor taskCreateProcessor)
        {
            _serviceBusHandler = serviceBusHandler;
            _taskCreateProcessor = taskCreateProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _serviceBusHandler.ReceiveMessageAsync(_taskCreateProcessor.ProcessCreateTaskAsync, "task_create_queue");
        }
    }

}
