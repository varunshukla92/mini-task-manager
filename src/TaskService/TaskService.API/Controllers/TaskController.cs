using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Interfaces;
using TaskService.Application.DTOs;
using TaskService.API.Extensions;

namespace TaskService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/tasks")]
[ApiVersion("1.0")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var task = await _taskService.CreateAsync(
            userId,
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id, version = "1.0" },
            task);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var task = await _taskService.GetByIdAsync(
            userId,
            id,
            cancellationToken);

        return task is null
            ? NotFound()
            : Ok(task);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTask(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        IReadOnlyList<TaskResponse> taskList = await _taskService.GetAllAsync(userId, cancellationToken);

        return taskList is null || !taskList.Any()
            ? NotFound() 
            : Ok(taskList);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        await _taskService.UpdateAsync(
            userId,
            id,
            request,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        await _taskService.DeleteAsync(
            userId,
            id,
            cancellationToken);

        return NoContent();
    }
}
