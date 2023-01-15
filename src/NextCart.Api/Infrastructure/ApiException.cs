using System.Runtime.Serialization;
using NextCart.Contracts.Cart.Proto;

namespace NextCart.Api.Infrastructure
{
    [Serializable]
    internal class ApiException : Exception
    {
        public ErrorDto errorDto;

        public ApiException()
        {
        }

        public ApiException(ErrorDto errorDto)
        {
            this.errorDto = errorDto;
        }
    }
}