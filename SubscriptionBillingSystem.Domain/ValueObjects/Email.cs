using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace SubscriptionBillingSystem.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; }

        public Email(string value)
        {            
            if (string.IsNullOrWhiteSpace(value))
                throw EmailErrors.Required();

            if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw EmailErrors.InvalidFormat();

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {            
            yield return Value;
        }
    }
}
