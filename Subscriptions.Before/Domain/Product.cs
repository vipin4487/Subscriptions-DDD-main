using Subscriptions.Before.SharedKernel;
using System;

namespace Subscriptions.Before.Domain
{
    public class Product: Entity
    {
        public string Name { get; set;}
        public decimal Amount { get; set; }
        public BillingPeriod BillingPeriod { get; set; }

        public DateTime CalculateCurrentPeriodEndDate()
        {
            return this.BillingPeriod switch
            {
                BillingPeriod.Weekly => DateTime.Now.AddDays(7),
                BillingPeriod.Monthly => DateTime.Now.AddMonths(1),
                _ => throw new InvalidOperationException()
            };
        }
    }

    public enum BillingPeriod
    {
        Weekly,
        Monthly
    }
}