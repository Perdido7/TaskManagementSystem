namespace TaskManagementSystem.Business.RabbitMQ
{
    public class TaskUpdateBackgroundService : BackgroundService
    {
        private readonly ServiceBusHandler _serviceBusHandler;
        private readonly TaskUpdateProcessor _taskUpdateProcessor;

        public TaskUpdateBackgroundService(ServiceBusHandler serviceBusHandler, TaskUpdateProcessor taskUpdateProcessor)
        {
            _serviceBusHandler = serviceBusHandler;
            _taskUpdateProcessor = taskUpdateProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _serviceBusHandler.ReceiveMessageAsync(_taskUpdateProcessor.ProcessUpdateTaskAsync, "task_update_queue");
        }
    }

}
