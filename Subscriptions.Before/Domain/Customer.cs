using System;
using System.Collections.Generic;
using Subscriptions.Before.Events;
using Subscriptions.Before.SharedKernel;

namespace Subscriptions.Before.Domain
{
    public class Customer: Entity
    {
        public string Email { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal MoneySpent { get; set; }

        private readonly List<Subscription> _subscriptions = new List<Subscription>();
        public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

        public void AddSubscription(Product product, ISubscriptionAmountCalculator subscriptionAmountCalculator)
        {            
            var subscription = new Subscription(
                this,
                product,
                subscriptionAmountCalculator.CalculateSubscriptionAmount(this, product)
            );
            _subscriptions.Add(subscription);
            MoneySpent += subscription.Amount;


            AddDomainEvent(new CustomerSubscribedToProduct
            {
                CustomerId = Id,
                ProductID = product.Id
            });
        }
    }
}