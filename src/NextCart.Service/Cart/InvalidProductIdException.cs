using System.Runtime.Serialization;

namespace NextCart.Service.Cart
{
    public enum ErrorCode
    {
        InvalidProductId = 1001
    }

    [Serializable]
    public class NextCartExceptions : Exception
    {
        public ErrorCode ErrorCode { get; private set; }
        public NextCartExceptions(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public static InvalidProductIdException InvalidProductId(string productId, Guid cartId) => new(productId, cartId, ErrorCode.InvalidProductId);
    }

    [Serializable]
    public class InvalidProductIdException : NextCartExceptions
    {
        public InvalidProductIdException(string productId, Guid cartId, ErrorCode errorCode) : base(errorCode, $"The product id {productId} doesn't exist in cart {cartId}")
        {
        }
    }
}