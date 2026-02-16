using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskService.Application.Interfaces;
using TaskService.Infrastructure.Data;
using TaskService.Infrastructure.Repositories;

namespace TaskService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<TaskDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("TaskDb"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddSingleton<IEventPublisher, ServiceBusPublisher>();
            return services;
        }
    }
}
