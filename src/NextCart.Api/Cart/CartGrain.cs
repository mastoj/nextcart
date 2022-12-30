using Proto;
using Proto.Cluster;

namespace NextCart.Api.Cart;

public class CartGrain : CartGrainBase
{
    private readonly ClusterIdentity _clusterIdentity;

    public CartGrain(IContext context, ClusterIdentity clusterIdentity) : base(context)
    {
        _clusterIdentity = clusterIdentity;
    }

    public override Task<Cart2> Create(CreateCart2 request)
    {
        return Task.FromResult(new Cart2
        {
            Id = request.Id,
        });
    }
}