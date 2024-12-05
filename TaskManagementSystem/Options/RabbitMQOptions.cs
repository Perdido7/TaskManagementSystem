namespace TaskManagementSystem.Options
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public required string UserName { get; set;}
        public required string Password { get; set;}
        public QueuesOptions Queues { get; set; } = new QueuesOptions();
    }

    public class QueuesOptions
    {
        public string CreateQueue { get; set; } = "task_create_queue";
        public string UpdateQueue { get; set; } = "task_update_queue";
    }
}
