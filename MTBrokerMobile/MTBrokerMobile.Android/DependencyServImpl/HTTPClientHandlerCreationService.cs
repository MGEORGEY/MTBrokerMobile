using Android.Net;
using Javax.Net.Ssl;
using MTBrokerMobile.DependencyServiceInterfaces;
using MTBrokerMobile.Droid.DependencyServImpl;
using System.Net.Http;
using Xamarin.Android.Net;

[assembly: Xamarin.Forms.Dependency(typeof(HTTPClientHandlerCreationService))]
namespace MTBrokerMobile.Droid.DependencyServImpl
{
    public class HTTPClientHandlerCreationService : IHttpClientHandlerCreationService
    {
        HttpClientHandler IHttpClientHandlerCreationService.GetInsecureHandler()
        {
            return new IgnoreSSLClientHandler();
        }
    }

    internal class IgnoreSSLClientHandler : AndroidClientHandler
    {
        protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
        {
            return SSLCertificateSocketFactory.GetInsecure(1000, null);
        }

        protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
        {
            return new IgnoreSSLHostnameVerifier();
        }
    }

    internal class IgnoreSSLHostnameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {
            return true;
        }
    }
}