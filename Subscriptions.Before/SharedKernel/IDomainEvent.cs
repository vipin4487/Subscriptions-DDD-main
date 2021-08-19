using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
namespace Subscriptions.Before.SharedKernel
{
    public interface IDomainEvent : INotification
    {
    }
}
