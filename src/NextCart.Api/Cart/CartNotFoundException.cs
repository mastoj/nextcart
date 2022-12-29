using System.Runtime.Serialization;

namespace NextCart.Api.Cart
{
    [Serializable]
    internal class CartNotFoundException : Exception
    {
        private Guid id;

        public CartNotFoundException()
        {
        }

        public CartNotFoundException(Guid id)
        {
            this.id = id;
        }

        public CartNotFoundException(string? message) : base(message)
        {
        }

        public CartNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected CartNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}