using NextCart.Domain.Cart;

namespace NextCart.Service.Cart;

public static class DomainMapper
{
    public static CreateCart ToDomain(this Contracts.Cart.Proto.CreateCart dto) => new(Guid.Parse(dto.Id));
    public static AddItem ToDomain(this Contracts.Cart.Proto.AddItem dto, Func<string, Product> GetProduct) => new(GetProduct(dto.ProductId));
    public static Domain.Cart.Cart ToDomain(this Contracts.Cart.Proto.CartDto dto) => new(Guid.Parse(dto.Id), dto.Items?.Select(x => x.ToDomain()), dto.Total, dto.Version);
    public static Item ToDomain(this Contracts.Cart.Proto.ItemDto dto) => new(dto.Product.ToDomain(), dto.ItemTotal, dto.Quantity);
    public static Product ToDomain(this Contracts.Cart.Proto.ProductDto dto) => new(dto.ProductId, dto.Name, dto.Url, dto.MainImage, dto.Amount, dto.Currency);
    public static RemoveItem ToDomain(this Contracts.Cart.Proto.RemoveItem dto) => new(dto.ProductId);
    public static IncreaseItemQuantity ToDomain(this Contracts.Cart.Proto.IncreaseQuantity dto) => new(dto.ProductId);
    public static DecreaseItemQuantity ToDomain(this Contracts.Cart.Proto.DecreaseQuantity dto) => new(dto.ProductId);
    public static ClearCart ToDomain(this Contracts.Cart.Proto.Clear _) => new();

    public static Contracts.Cart.Proto.CartDto ToDto(this Domain.Cart.Cart cart)
    {
        var dto = new Contracts.Cart.Proto.CartDto()
        {
            Id = cart.Id.ToString(),
            // Items = cart.Items?.Select(x => x.ToDto()) ?? new List<Contracts.Cart.Proto.ItemDto> { },
            Total = cart.Total,
            Version = cart.Version
        };
        dto.Items.AddRange(cart.Items?.Select(x => x.ToDto()) ?? new List<Contracts.Cart.Proto.ItemDto> { });
        return dto;
    }

    public static Contracts.Cart.Proto.ItemDto ToDto(this Item item) => new()
    {
        Product = item.Product.ToDto(),
        ItemTotal = item.ItemTotal,
        Quantity = item.Quantity
    };

    public static Contracts.Cart.Proto.ProductDto ToDto(this Product product) => new()
    {
        ProductId = product.ProductId,
        Name = product.Name,
        Url = product.Url,
        MainImage = product.MainImage,
        Amount = product.Amount,
        Currency = product.Currency
    };
}
