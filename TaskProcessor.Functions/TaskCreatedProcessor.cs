using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TaskProcessor.Functions.Models;

namespace TaskProcessor.Functions;

public class TaskCreatedProcessor
{
    private readonly ILogger<TaskCreatedProcessor> _logger;

    public TaskCreatedProcessor(ILogger<TaskCreatedProcessor> logger)
    {
        _logger = logger;
    }

    [Function("TaskCreatedProcessor")]
    public async Task Run(
        [ServiceBusTrigger("task-created-queue", Connection = "ServiceBusConnectionString")]
        string message)
    {
        _logger.LogInformation("Raw message received: {Message}", message);

        var taskEvent = JsonSerializer.Deserialize<TaskCreatedEvent>(message);

        if (taskEvent == null)
        {
            _logger.LogError("Failed to deserialize TaskCreatedEvent!!");
            return;
        }

        _logger.LogInformation(
            "Task Created Event Processed | TaskId: {TaskId} | Title: {Title} | AssignedTo: {AssignedTo} | DueDate: {DueDate}",
            taskEvent.TaskId,
            taskEvent.Title,
            taskEvent.AssignedTo,
            taskEvent.DueDate);

        await Task.CompletedTask;
    }
}