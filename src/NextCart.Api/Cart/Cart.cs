namespace NextCart.Api.Cart;

public interface CartEvent { }

public record CreateCart(Guid CartId);

public record CartCreated(Guid CartId) : CartEvent;

public record Cart(Guid Id, int Version = 1)
{
    public static Cart Create(CartCreated created) => new(created.CartId);
}