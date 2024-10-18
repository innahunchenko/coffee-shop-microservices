using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderObjects
{
    public class UserId : ValueOf<Guid, UserId>
    {
        protected override void Validate()
        {
            if (Value == Guid.Empty)
            {
                // some default guid for unknown user
            }
        }
    }
}
