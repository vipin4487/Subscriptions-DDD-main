using AzureServiceBus.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subscriptions.Before.IntegrationEvents
{
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public int ProductId { get; private set; }
        public decimal NewPrice { get; private set; }
        public decimal OldPrice { get; private set; }

        public ProductPriceChangedIntegrationEvent(int productId, decimal newPrice,
            decimal oldPrice)
        {
            ProductId = productId;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }
    }
}
