using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate
{
    public enum InvoiceStatus
    {
        Pending,
        Paid
    }
}
