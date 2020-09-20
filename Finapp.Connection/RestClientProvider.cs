using RestSharp;

namespace Finapp.Connection
{
    public class RestClientProvider
    {
        public static readonly RestClient RestClient = new RestClient(Consts.ApiBaseUrl);
    }
}