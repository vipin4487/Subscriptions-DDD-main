using System;
using Subscriptions.Before.SharedKernel;

namespace Subscriptions.Before.Domain
{
    public class Subscription : Entity
    {
        public SubscriptionStatus Status { get;private set; }
        public Customer Customer { get; private set; }
        public Product Product { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime CurrentPeriodEndDate { get; private set; }
        private Subscription()
        {

        }
        public Subscription(Customer customer, Product product, decimal amount) : this()
        {
            Id = Guid.NewGuid();
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            Product = product ?? throw new ArgumentNullException(nameof(product));
            Amount = amount >= 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount));
            Status = SubscriptionStatus.Active;
            CurrentPeriodEndDate = product.CalculateCurrentPeriodEndDate();
        }
    }
}