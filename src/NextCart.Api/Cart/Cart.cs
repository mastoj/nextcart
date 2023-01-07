namespace NextCart.Api.Cart;

public record Product(
    string ProductId,
    string Name,
    string Url,
    string MainImage,
    float Amount,
    string Currency);

public record Item(Product Product, float ItemTotal, int Quantity);
public interface CartEvent { }

// public record CreateCart(Guid CartId);
public record CreateCart(string Id);
public record AddItem(Product Product);
public record IncreaseItemQuantity(string ProductId);
public record RemoveItem(string ProductId);
public record DecreaseItemQuantity(string ProductId);
public record ClearCart();

public record CartCreated(string CartId) : CartEvent;
public record ItemAdded(Item Item, float NewTotal) : CartEvent;
public record ItemQuantityIncreased(Item Item, float NewTotal) : CartEvent;
public record ItemQuantityDecreased(Item Item, float NewTotal) : CartEvent;
public record ItemRemoved(string ProductId, float NewTotal) : CartEvent;
public record CartCleared() : CartEvent;

public record Cart(
    string Id,
    IEnumerable<Item>? Items = null,
    float Total = 0,
    int Version = 1)
{
    public static Cart Create(CartCreated created) => new(created.CartId);
    public Cart Apply(ItemAdded added) => this with
    {
        Items = Items?.Append(added.Item) ?? new[] { added.Item },
        Total = added.NewTotal
    };
    public Cart Apply(ItemQuantityIncreased increased) => this with
    {
        Items = Items?.Select(x => x.Product.ProductId == increased.Item.Product.ProductId ? increased.Item : x),
        Total = increased.NewTotal
    };
    public Cart Apply(ItemQuantityDecreased decreased) => this with
    {
        Items = Items?.Select(x => x.Product.ProductId == decreased.Item.Product.ProductId ? decreased.Item : x),
        Total = decreased.NewTotal
    };
    public Cart Apply(ItemRemoved removed) => this with
    {
        Items = Items?.Where(x => x.Product.ProductId != removed.ProductId),
        Total = removed.NewTotal
    };
    public Cart Apply(CartCleared _) => this with
    {
        Items = new List<Item> { },
        Total = 0
    };
}