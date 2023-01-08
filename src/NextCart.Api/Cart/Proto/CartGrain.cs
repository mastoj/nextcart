using Marten;
using Proto;
using Proto.Cluster;
using NextCart.Api.Infrastructure;

namespace NextCart.Api.Cart.Proto;

public class CartGrain : CartGrainBase
{
    private readonly IDocumentStore _documentStore;
    private CartDto? _cart;
    private readonly Guid _cartId;

    private static Product GetProduct(string productId)
    {
        return new(productId, "Product Name", "https://www.google.com", "https://www.google.com", 100, "USD");
    }

    public CartGrain(IContext context, IDocumentStore documentStore) : base(context)
    {
        _cart = new CartDto();
        _cartId = Guid.Parse(context.ClusterIdentity()!.Identity);
        _documentStore = documentStore;
    }

    public override Task OnStarted()
    {
        using var dbSession = _documentStore.LightweightSession();
        var cart = dbSession.Events.AggregateStream<Cart>(_cartId);
        _cart = cart?.ToDto();
        Console.WriteLine("==> OnStarted: " + _cart);
        return base.OnStarted();
    }

    public override Task<CartResponse> Create(CreateCart request)
    {
        Console.WriteLine("====> Grain: Create cart" + request);
        var result = _documentStore.Add<Cart>(_cartId, () => CartService.Handle(request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        Console.WriteLine("====> CreatedCart" + _cart);
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> Get(GetCart request)
    {
        return Task.FromResult(new CartResponse { Cart = _cart });
    }

    public override Task<CartResponse> AddItem(AddItem request)
    {
        Console.WriteLine("====> AddItem: " + _cart.Version);
        // var result = _documentStore.Update<Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain(GetProduct)), Context.CancellationToken).Result;
        var result = _documentStore.GetAndUpdate<Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain(GetProduct)), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> RemoveItem(RemoveItem request)
    {
        var result = _documentStore.Update<Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> IncreaseQuantity(IncreaseQuantity request)
    {
        var result = _documentStore.Update<Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> DecreaseQuantity(DecreaseQuantity request)
    {
        var result = _documentStore.Update<Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> Clear(Clear request)
    {
        var result = _documentStore.Update<Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }
}