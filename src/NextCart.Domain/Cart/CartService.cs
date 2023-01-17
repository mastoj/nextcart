namespace NextCart.Domain.Cart
{
    public class CartService
    {
        public static CartEvent[] Handle(CreateCart command)
        {
            return new[]
            {
                new CartCreated(command.Id)
            };
        }

        public static CartEvent[] Handle(Cart cart, AddItem command)
        {
            if (cart.Items?.Any(x => x.Product.ProductId == command.Product.ProductId) == true)
                return Handle(cart, new IncreaseItemQuantity(command.Product.ProductId));
            return new[]
            {
                new ItemAdded(new Item(command.Product, command.Product.Amount, 1), cart.Total + command.Product.Amount)
            };
        }

        public static CartEvent[] Handle(Cart cart, IncreaseItemQuantity command)
        {
            var item = cart.Items?.First(x => x.Product.ProductId == command.ProductId);
            if (item == null) throw NextCartExceptions.InvalidProductId(command.ProductId, cart.Id);
            var newItem = item with { Quantity = item.Quantity + 1, ItemTotal = item.ItemTotal + item.Product.Amount };
            var newTotal = cart.Total + item.Product.Amount;
            return new[]
            {
                new ItemQuantityIncreased(newItem, newTotal)
            };
        }

        public static CartEvent[] Handle(Cart cart, RemoveItem command)
        {
            var item = cart.Items?.First(x => x.Product.ProductId == command.ProductId);
            if (item == null) throw NextCartExceptions.InvalidProductId(command.ProductId, cart.Id);
            var newTotal = cart.Total - item.ItemTotal;
            return new[]
            {
                new ItemRemoved(command.ProductId, newTotal)
            };
        }

        public static CartEvent[] Handle(Cart cart, DecreaseItemQuantity command)
        {
            var item = cart.Items?.First(x => x.Product.ProductId == command.ProductId);
            if (item == null) throw NextCartExceptions.InvalidProductId(command.ProductId, cart.Id);
            if (item.Quantity == 1) return Handle(cart, new RemoveItem(command.ProductId));
            var newItem = item with { Quantity = item.Quantity - 1, ItemTotal = item.ItemTotal - item.Product.Amount };
            var newTotal = cart.Total - item.Product.Amount;
            return new[]
            {
                new ItemQuantityDecreased(newItem, newTotal)
            };
        }

        public static CartEvent[] Handle(Cart cart, ClearCart command)
        {
            return new[]
            {
                new CartCleared()
            };
        }
    }
}