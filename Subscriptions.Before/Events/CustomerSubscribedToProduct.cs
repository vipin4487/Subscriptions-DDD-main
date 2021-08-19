using MediatR;
using Subscriptions.Before.Services;
using Subscriptions.Before.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Before.Events
{
    public class CustomerSubscribedToProduct : IDomainEvent
    {
        public Guid ProductID { get; set; }
        public Guid CustomerId { get; set; }

        public class CustomerSubscribedToProductHandler : INotificationHandler<CustomerSubscribedToProduct>
        {
            private readonly IEmailSender _emailSender;
            public CustomerSubscribedToProductHandler(IEmailSender emailSender)
            {
                _emailSender = emailSender;
            }
            public Task Handle(CustomerSubscribedToProduct notification, CancellationToken cancellationToken)
            {
                _emailSender.SendEmailAsync(notification.CustomerId +  " Congratulations! You subscribed to a cool product");
                return Task.CompletedTask;
            }
        }
    }
}
