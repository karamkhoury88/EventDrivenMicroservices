using Microsoft.Extensions.DependencyInjection;

namespace Common.Services
{
    public static class ServiceExtentions
    {
        /// <summary>
        /// Register MessageQueueService for the application.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void RegisterMessageQueueService(this IServiceCollection services)
        {
            services.AddSingleton<IMessageQueueService, RabbitMqService>();
        }
    }
}
