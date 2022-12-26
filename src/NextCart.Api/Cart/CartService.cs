namespace NextCart.Api.Cart
{
    public class CartService
    {
        public static CartEvent[] Handle(CreateCart command)
        {
            return new[]
            {
                new CartCreated(command.CartId)
            };
        }
    }
}