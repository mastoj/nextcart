namespace NextCart.Service.Cart;

public static class DomainMapper
{
    public static CreateCart ToDomain(this Proto.CreateCart dto) => new(Guid.Parse(dto.Id));
    public static AddItem ToDomain(this Proto.AddItem dto, Func<string, Product> GetProduct) => new(GetProduct(dto.ProductId));
    public static Cart ToDomain(this Proto.CartDto dto) => new(Guid.Parse(dto.Id), dto.Items?.Select(x => x.ToDomain()), dto.Total, dto.Version);
    public static Item ToDomain(this Proto.ItemDto dto) => new(dto.Product.ToDomain(), dto.ItemTotal, dto.Quantity);
    public static Product ToDomain(this Proto.ProductDto dto) => new(dto.ProductId, dto.Name, dto.Url, dto.MainImage, dto.Amount, dto.Currency);
    public static RemoveItem ToDomain(this Proto.RemoveItem dto) => new(dto.ProductId);
    public static IncreaseItemQuantity ToDomain(this Proto.IncreaseQuantity dto) => new(dto.ProductId);
    public static DecreaseItemQuantity ToDomain(this Proto.DecreaseQuantity dto) => new(dto.ProductId);
    public static ClearCart ToDomain(this Proto.Clear _) => new();

    public static Proto.CartDto ToDto(this Cart cart)
    {
        var dto = new Proto.CartDto()
        {
            Id = cart.Id.ToString(),
            // Items = cart.Items?.Select(x => x.ToDto()) ?? new List<Proto.ItemDto> { },
            Total = cart.Total,
            Version = cart.Version
        };
        dto.Items.AddRange(cart.Items?.Select(x => x.ToDto()) ?? new List<Proto.ItemDto> { });
        return dto;
    }

    public static Proto.ItemDto ToDto(this Item item) => new()
    {
        Product = item.Product.ToDto(),
        ItemTotal = item.ItemTotal,
        Quantity = item.Quantity
    };

    public static Proto.ProductDto ToDto(this Product product) => new()
    {
        ProductId = product.ProductId,
        Name = product.Name,
        Url = product.Url,
        MainImage = product.MainImage,
        Amount = product.Amount,
        Currency = product.Currency
    };
}
