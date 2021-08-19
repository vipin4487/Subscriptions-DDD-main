namespace Subscriptions.Before.Domain
{
    public interface ISubscriptionAmountCalculator
    {
        public decimal CalculateSubscriptionAmount(Customer customer, Product product);
    }
}