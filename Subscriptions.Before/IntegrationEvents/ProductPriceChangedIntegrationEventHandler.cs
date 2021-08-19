using AzureServiceBus.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subscriptions.Before.IntegrationEvents
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        public ProductPriceChangedIntegrationEventHandler()
        {

        }
        public Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
