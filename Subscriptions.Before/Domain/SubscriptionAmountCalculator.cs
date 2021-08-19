using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subscriptions.Before.Domain
{
    public class SubscriptionAmountCalculator : ISubscriptionAmountCalculator
    {
        public decimal CalculateSubscriptionAmount(Customer customer, Product product)
        {
            var subscriptionAmount = product.Amount;
            if (customer.MoneySpent >= 100)
            {
                subscriptionAmount *= 0.8M;
            }
            else if (customer.MoneySpent >= 1000)
            {
                subscriptionAmount *= 0.5M;
            }

            return subscriptionAmount;
        }
    }
}
