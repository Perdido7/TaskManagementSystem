using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskManagementSystem.Options;

public class ServiceBusHandler : IDisposable
{
    private readonly RabbitMQOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    public ServiceBusHandler(IOptions<RabbitMQOptions> options)
    {
        _options = options.Value;
    }

    private async Task EnsureConnectionAsync(string queueName)
    {
        if (_connection == null || !_connection.IsOpen)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to connect to RabbitMQ.", ex);
            }
        }

        if (_channel == null || !_channel.IsOpen)
        {
            try
            {
                _channel = await _connection.CreateChannelAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create RabbitMQ channel.", ex);
            }
        }

        try
        {
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to declare queue {queueName}.", ex);
        }
    }

    public async Task SendMessageAsync<T>(string queueName, T message)
    {
        await EnsureConnectionAsync(queueName);

        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }

        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        try
        {
            await _channel.BasicPublishAsync("", queueName, body, CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to publish message.", ex);
        }
    }

    public async Task ReceiveMessageAsync(Func<string, Task> processMessage, string queueName)
    {
        await EnsureConnectionAsync(queueName);

        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            await processMessage(message);

            await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
        };

        await _channel.BasicConsumeAsync(queue: queueName,
                                         autoAck: false,
                                         consumer: consumer);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }
}