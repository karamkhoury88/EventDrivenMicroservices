using Common.Events.Dtos;
using Common.Services;
using RabbitMQ.Client.Events;

namespace Common.Events
{
    public abstract class Event<T> where T : EventDto
    {
        private readonly IMessageQueueService _svc;
        private readonly T? _data;

        protected Event(T? data, IMessageQueueService svc)
        {
            _data = data;
            _svc = svc;
        }

        protected abstract string EventName { get; }
        protected abstract string ExchangeName { get; }

        /// <summary>
        /// Publish an event to the queue.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task PublishAsync()
        {
            if(_data == null)
            {
                throw new InvalidOperationException("Data cannot be null when publishing an event.");
            }

            await _svc.PublishMessageToTopicExchange(ExchangeName, EventName, _data);
        }

        /// <summary>
        /// Consume messages from the queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="processMessageHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task ConsumeAsync(string queueName, Func<object?, BasicDeliverEventArgs, CancellationToken, Task> processMessageHandler, CancellationToken cancellationToken)
        {
            await _svc.ConsumeMessageFromTopicExchange(exchangeName: ExchangeName,
                 queueName: queueName,
                 routingKey: EventName,
                 processMessageHandler,
                 cancellationToken
                 );
        }
    }
}
