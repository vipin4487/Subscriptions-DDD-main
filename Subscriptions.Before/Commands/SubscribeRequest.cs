using System;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.EventBus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Subscriptions.Before.Data;
using Subscriptions.Before.Domain;
using Subscriptions.Before.IntegrationEvents;
using Subscriptions.Before.Services;

namespace Subscriptions.Before.Commands
{
    public class SubscribeRequest : IRequest
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
    }
    public class SubscribeRequestHandler : IRequestHandler<SubscribeRequest>
    {
        private readonly SubscriptionContext _subscriptionContext;
        private readonly IEmailSender _emailSender;
        private readonly ISubscriptionAmountCalculator _subscriptionAmountCalculator;
        private readonly IEventBus _eventBus;
        public SubscribeRequestHandler(SubscriptionContext subscriptionContext,
            IEmailSender emailSender, ISubscriptionAmountCalculator subscriptionAmountCalculator, IEventBus eventBus)
        {
            _subscriptionContext = subscriptionContext;
            _emailSender = emailSender;
            _subscriptionAmountCalculator = subscriptionAmountCalculator;
            _eventBus = eventBus;
        }
        public async Task<Unit> Handle(SubscribeRequest request, CancellationToken cancellationToken)
        {
            var customer = await _subscriptionContext
                .Customers
                .Include(x => x.Subscriptions)
                .FirstAsync(x => x.Id == request.CustomerId, cancellationToken: cancellationToken);

            var product = await _subscriptionContext.Products.FindAsync(request.ProductId);
            customer.AddSubscription(product, _subscriptionAmountCalculator);

            await _subscriptionContext.SaveChangesAsync(cancellationToken);
            ProductPriceChangedIntegrationEvent @event = new ProductPriceChangedIntegrationEvent(1,10,5 );
            _eventBus.Publish(@event);
            //await _emailSender.SendEmailAsync("Congratulations! You subscribed to a cool product");
            return Unit.Value;
        }  
    }
}