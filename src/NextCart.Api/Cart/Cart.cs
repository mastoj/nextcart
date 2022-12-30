namespace NextCart.Api.Cart;

public record Product(
    string ProductId,
    string Name,
    string Url,
    string MainImage,
    float Amount,
    string Currency,
    int Quantity);

public interface CartEvent { }

// public record CreateCart(Guid CartId);
public record AddItem(Product Product);

public record CartCreated(string CartId) : CartEvent;
public record ItemAdded(Product product, float newTotal) : CartEvent;

public record Cart(
    string Id,
    IEnumerable<Product>? Items = null,
    float Total = 0,
    int Version = 1)
{
    public static Cart Create(CartCreated created) => new(created.CartId);
}