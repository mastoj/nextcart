using System.Runtime.Serialization;

namespace NextCart.Domain.Cart
{
    [Serializable]
    public class DuplicateCartException : Exception
    {
        private Guid cartId;

        public DuplicateCartException()
        {
        }

        public DuplicateCartException(Guid cartId)
        {
            this.cartId = cartId;
        }

        public DuplicateCartException(string? message) : base(message)
        {
        }

        public DuplicateCartException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DuplicateCartException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}