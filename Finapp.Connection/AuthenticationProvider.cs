using System.Threading.Tasks;
using Finapp.Connection.Exception;
using Finapp.Dto.Authentication;
using RestSharp;
using RestSharp.Authenticators;

namespace Finapp.Connection
{
    public static class AuthenticationProvider
    {
        public static async Task AuthenticateAsync(string username, string password)
        {
            var restRequest = new RestRequest("api/authenticate")
            {
                Method = Method.POST
            };
            var authenticationRequestDto = new AuthenticationRequestDto
            {
                Username = username,
                Password = password
            };
            restRequest.AddJsonBody(authenticationRequestDto);
            var response = await RestClientProvider.RestClient.ExecuteAsync<AuthenticationResponseDto>(restRequest);

            if (response.IsSuccessful == false)
            {
                throw new CannotLogInException(response);
            }

            if (string.IsNullOrEmpty(response.Data.Token))
            {
                throw new TokenNotReceivedException();
            }

            AuthClient = new RestClient(Consts.ApiBaseUrl)
            {
                Authenticator = new JwtAuthenticator(response.Data.Token)
            };
        }

        public static RestClient AuthClient { get; private set; }
    }
}