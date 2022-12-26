namespace NextCart.Api.Cart;

public interface CartEvent { }

public record CreateCart(Guid CartId);

public record CartCreated(Guid CartId) : CartEvent;

public record Cart(Guid CartId, int Version = 1)
{
    public static Cart Create(Guid CartId) => new(CartId);
}