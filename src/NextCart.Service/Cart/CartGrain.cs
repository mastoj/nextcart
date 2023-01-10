using Marten;
using Proto;
using Proto.Cluster;
using NextCart.Service.Infrastructure;
using NextCart.Contracts.Cart.Proto;
using NextCart.Domain.Cart;

namespace NextCart.Service.Cart;

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

    public override Task OnReceive()
    {
        switch (Context.Message)
        {
            case ReceiveTimeout _:
                Console.WriteLine("Actor timed out: " + _cartId);
                Context.Poison(Context.Self);
                break;
        }
        return base.OnReceive();
    }

    public override Task OnStarted()
    {
        try
        {
            Console.WriteLine("====> OnStarted: " + _cartId);
            using var dbSession = _documentStore.LightweightSession();
            var cart = dbSession.Events.AggregateStream<Domain.Cart.Cart>(_cartId);
            _cart = cart?.ToDto();
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            Console.WriteLine("==> Exception: " + ex);
            throw;
        }
        return base.OnStarted();
    }

    public override Task<CartResponse> Create(Contracts.Cart.Proto.CreateCart request)
    {
        var result = _documentStore.Add<Domain.Cart.Cart>(_cartId, () => CartService.Handle(request.ToDomain()), this.Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> Get(GetCart request)
    {
        return Task.FromResult(new CartResponse { Cart = _cart });
    }

    public override Task<CartResponse> AddItem(Contracts.Cart.Proto.AddItem request)
    {
        Console.WriteLine("====> AddItem: " + _cart.Version);
        // var result = _documentStore.Update<Domain.Cart.Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain(GetProduct)), Context.CancellationToken).Result;
        var result = _documentStore.GetAndUpdate<Domain.Cart.Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain(GetProduct)), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> RemoveItem(Contracts.Cart.Proto.RemoveItem request)
    {
        var result = _documentStore.Update<Domain.Cart.Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Domain.Cart.Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> IncreaseQuantity(Contracts.Cart.Proto.IncreaseQuantity request)
    {
        var result = _documentStore.Update<Domain.Cart.Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Domain.Cart.Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> DecreaseQuantity(Contracts.Cart.Proto.DecreaseQuantity request)
    {
        var result = _documentStore.Update<Domain.Cart.Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Domain.Cart.Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }

    public override Task<CartResponse> Clear(Contracts.Cart.Proto.Clear request)
    {
        var result = _documentStore.Update<Domain.Cart.Cart>(_cartId, _cart.Version, CartService.Handle(_cart.ToDomain(), request.ToDomain()), Context.CancellationToken).Result;
        // var result = _documentStore.GetAndUpdate<Domain.Cart.Cart>(_cartId, _cart.Version, (cart) => CartService.Handle(cart, request.ToDomain()), Context.CancellationToken).Result;
        _cart = result!.ToDto();
        var response = new CartResponse { Cart = _cart };
        return Task.FromResult(response);
    }
}