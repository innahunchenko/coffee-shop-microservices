namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class Address
    {
        public FirstName FirstName { get; private set; } = default!;
        public LastName LastName { get; private set; } = default!;
        public EmailAddress? EmailAddress { get; private set; } = default!;
        public AddressLine AddressLine { get; private set; } = default!;
        public Country Country { get; private set; } = default!;
        public State State { get; private set; } = default!;
        public ZipCode ZipCode { get; private set; } = default!;

        public static Address From(
            FirstName firstName, LastName lastName, EmailAddress? emailAddress,
            AddressLine addressLine, Country country, State state, ZipCode zipCode)
        {
            return new Address
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                AddressLine = addressLine,
                Country = country,
                State = state,
                ZipCode = zipCode
            };
        }
    }
}
