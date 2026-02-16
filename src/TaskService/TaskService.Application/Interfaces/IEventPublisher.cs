using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T message);
    }
}
