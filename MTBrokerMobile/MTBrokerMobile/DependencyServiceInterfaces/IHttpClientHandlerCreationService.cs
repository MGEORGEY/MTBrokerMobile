using System.Net.Http;

namespace MTBrokerMobile.DependencyServiceInterfaces
{
    public interface IHttpClientHandlerCreationService
    {
        HttpClientHandler GetInsecureHandler();
    }
}
