namespace NextCart.Service.Cart;

public static class DomainMapper
{
    public static CreateCart ToDomain(this Contracts.CreateCart dto) => new(Guid.Parse(dto.Id));
    public static AddItem ToDomain(this Contracts.AddItem dto, Func<string, Product> GetProduct) => new(GetProduct(dto.ProductId));
    public static Cart ToDomain(this Contracts.CartDto dto) => new(Guid.Parse(dto.Id), dto.Items?.Select(x => x.ToDomain()), dto.Total, dto.Version);
    public static Item ToDomain(this Contracts.ItemDto dto) => new(dto.Product.ToDomain(), dto.ItemTotal, dto.Quantity);
    public static Product ToDomain(this Contracts.ProductDto dto) => new(dto.ProductId, dto.Name, dto.Url, dto.MainImage, dto.Amount, dto.Currency);
    public static RemoveItem ToDomain(this Contracts.RemoveItem dto) => new(dto.ProductId);
    public static IncreaseItemQuantity ToDomain(this Contracts.IncreaseQuantity dto) => new(dto.ProductId);
    public static DecreaseItemQuantity ToDomain(this Contracts.DecreaseQuantity dto) => new(dto.ProductId);
    public static ClearCart ToDomain(this Contracts.Clear _) => new();

    public static Contracts.CartDto ToDto(this Cart cart)
    {
        var dto = new Contracts.CartDto()
        {
            Id = cart.Id.ToString(),
            // Items = cart.Items?.Select(x => x.ToDto()) ?? new List<Contracts.ItemDto> { },
            Total = cart.Total,
            Version = cart.Version
        };
        dto.Items.AddRange(cart.Items?.Select(x => x.ToDto()) ?? new List<Contracts.ItemDto> { });
        return dto;
    }

    public static Contracts.ItemDto ToDto(this Item item) => new()
    {
        Product = item.Product.ToDto(),
        ItemTotal = item.ItemTotal,
        Quantity = item.Quantity
    };

    public static Contracts.ProductDto ToDto(this Product product) => new()
    {
        ProductId = product.ProductId,
        Name = product.Name,
        Url = product.Url,
        MainImage = product.MainImage,
        Amount = product.Amount,
        Currency = product.Currency
    };
}
