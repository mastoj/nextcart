using Marten;
using Proto;
using Proto.Cluster;
using NextCart.Api.Infrastructure;

namespace NextCart.Api.Cart;

public class CartGrain : CartGrainBase
{
    private readonly IDocumentStore _documentStore;
    private CartDto _cart;

    public CartGrain(IContext context, IDocumentStore documentStore) : base(context)
    {
        Console.WriteLine("====> CartGrain");
        _cart = new CartDto();
        _documentStore = documentStore;
    }

    public CartGrain(IContext context) : base(context)
    {
        Console.WriteLine("====> CartGrain");
        _cart = new CartDto();
    }

    public override Task OnStarted()
    {
        Console.WriteLine("====> OnStarted");
        return base.OnStarted();
    }

    public override Task<CartDto> Create(CreateCart request)
    {
        Console.WriteLine("====> CreateCart");
        var result = _documentStore.Add<Cart>(request.Id, () => CartService.Handle(new CreateCart { Id = request.Id }), Context.CancellationToken).Result;
        _cart = ToCartDto(result!);
        _cart = new CartDto { Id = request.Id };
        Console.WriteLine("====> CreatedCart");
        return Task.FromResult(_cart);
    }

    private static CartDto ToCartDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
        };
    }
}