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

        public static CartEvent[] Handle(Cart cart, AddItem command)
        {
            return new[]
            {
                new ItemAdded(command.Product, cart.Total + command.Product.Amount * command.Product.Quantity)
            };
        }
    }
}