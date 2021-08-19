using System;
using System.Collections.Generic;

namespace Subscriptions.Before.SharedKernel
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        //public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public IReadOnlyCollection<IDomainEvent> DomainEvents
        {
            get
            {
                return _domainEvents.AsReadOnly();
            }
        }

        public void AddDomainEvent(IDomainEvent @event)
        {
            _domainEvents.Add(@event);
        }
        public void RemoveDomainEvent(IDomainEvent @event)
        {
            _domainEvents?.Remove(@event);
        }
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}