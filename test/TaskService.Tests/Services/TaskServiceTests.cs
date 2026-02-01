using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Application.DTOs;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;
using TaskService.Tests.Helpers;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;
using TaskStatus = TaskService.Domain.Enums.TaskStatus;
using TskService = TaskService.Application.Services.TaskService;

namespace TaskService.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _repositoryMock;

        private readonly TskService taskService;
        public TaskServiceTests()
        {            
            _repositoryMock = new Mock<ITaskRepository>();
            taskService = new TskService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_TaskRespone_When_Payload_Is_Valid()
        {
            // Arrange

            var userId = TaskTestDataFactory.CreateUserId();

            var request = TaskTestDataFactory.CreateCreateTaskRequest();

            TaskItem? savedTask = null;

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Callback<TaskItem, CancellationToken>((task, ct) =>
                {
                    savedTask = task;
                })
                .Returns(Task.CompletedTask);

            // Act

            var result = await taskService.CreateAsync(userId, request, It.IsAny<CancellationToken>());

            // Assert

            result.Should().NotBeNull();

            result.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            result.DueDate.Should().Be(request.DueDate);
            result.Priority.Should().Be(request.Priority);

            savedTask.Should().NotBeNull();
            savedTask?.UserId.Should().Be(userId);

            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
                Times.Once());

        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Title_Is_Empty_Creation_Fails()
        {
            // Arrange
            var userId = TaskTestDataFactory.CreateUserId();

            var request = TaskTestDataFactory.CreateCreateTaskRequest(
                title: ""
            );

            // Act
            Func<Task> act = () =>
                taskService.CreateAsync(userId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Title cannot be empty");

            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_User_Id_Is_Empty_Creation_Fails()
        {
            // Arrange
            var userId = new Guid();

            var request = TaskTestDataFactory.CreateCreateTaskRequest();

            // Act
            Func<Task> act = () =>
                taskService.CreateAsync(userId, request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("UserId is required");

            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFound_When_Task_Does_Not_Exist()
        {
            // Arrange
            var userId = TaskTestDataFactory.CreateUserId();
            var taskId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () =>
                taskService.GetByIdAsync(userId, taskId, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Task with id '{taskId}' was not found.");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Task_When_Task_Exists()
        {
            // Arrange
            var userId = TaskTestDataFactory.CreateUserId();
            var taskId = Guid.NewGuid();

            var task = TaskTestDataFactory.CreateTask(userId);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            // Act
            var result = await taskService.GetByIdAsync(userId, taskId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be(task.Title);
            result.Description.Should().Be(task.Description);
            result.Priority.Should().Be(task.Priority);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_Task_List()
        {
            // Arrange
            var userId = TaskTestDataFactory.CreateUserId();

            var tasks = new List<TaskItem>
            {
                TaskTestDataFactory.CreateTask(userId, "Task 1", "Desc 1", TaskPriority.Low, DateTime.UtcNow.AddDays(1)),
                TaskTestDataFactory.CreateTask(userId, "Task 2", "Desc 2", TaskPriority.High, DateTime.UtcNow.AddDays(2))
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tasks);

            // Act
            var result = await taskService.GetAllAsync(userId, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Title).Should().Contain(new[] { "Task 1", "Task 2" });
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFound_When_Task_Does_Not_Exist()
        {
            // Arrange
            var userId = TaskTestDataFactory.CreateUserId();
            var taskId = Guid.NewGuid();

            var request = TaskTestDataFactory.CreateUpdateTaskRequest();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = () =>
                taskService.UpdateAsync(userId, taskId, request, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>();

            _repositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Task_When_Task_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var task = TaskTestDataFactory.CreateTask(userId);

            var request = TaskTestDataFactory.CreateUpdateTaskRequest(
                "New Title",
                "New Desc",
                TaskStatus.Done,
                TaskPriority.High,
                DateTime.UtcNow.AddDays(3)
            );
            

            _repositoryMock
                .Setup(r => r.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            _repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await taskService.UpdateAsync(userId, taskId, request, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _repositoryMock.Verify(
                r => r.UpdateAsync(
                    It.Is<TaskItem>(t => t.Title == "New Title"),
                    It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_Task_Does_Not_Exist()
        {
            // Arrange
            var userId = TaskTestDataFactory.CreateUserId();
            var taskId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await taskService.DeleteAsync(userId, taskId, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_Task_When_Task_Exists()
        {
            // Arrange
            var userId = TaskTestDataFactory.CreateUserId();
            var taskId = Guid.NewGuid();

            var task = TaskTestDataFactory.CreateTask(userId);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            _repositoryMock
                .Setup(r => r.DeleteAsync(task, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await taskService.DeleteAsync(userId, taskId, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _repositoryMock.Verify(
                r => r.DeleteAsync(task, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }



    }
}
