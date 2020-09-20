using Finapp.Dto.Authentication;
using RestSharp;

namespace Finapp.Connection.Exception
{
    public class CannotLogInException : AuthenticationException
    {
        public IRestResponse<AuthenticationResponseDto> Response { get; }

        public CannotLogInException(IRestResponse<AuthenticationResponseDto> response)
        {
            Response = response;
        }
    }
}