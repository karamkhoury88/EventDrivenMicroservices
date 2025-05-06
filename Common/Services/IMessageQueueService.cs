using Common.Events.Dtos;
using RabbitMQ.Client.Events;

namespace Common.Services
{
    public interface IMessageQueueService
    {
        /// <summary>
        /// Publish message to a topic exchange.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        Task PublishMessageToTopicExchange<T>(string exchangeName, string routingKey, T eventData) where T : EventDto;

        /// <summary>
        /// Consume message from a topic exchange.
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        /// <param name="processMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ConsumeMessageFromTopicExchange(
             string exchangeName,
             string queueName,
        string routingKey,
             Func<object?, BasicDeliverEventArgs, CancellationToken, Task> processMessage,
             CancellationToken cancellationToken);
    }
}