using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

using Axis.Share.Soap.ActionService;
using Axis.Share.Soap.EventService;

namespace Axis.Share.Soap
{
    public static class ClientFactory
    {
        /// <summary>
        ///     Creates an action service client with the specified address and the provided credentials.
        /// </summary>
        public static ActionClient CreateActionServiceClient(string address, string userName, string password)
        {
            var encoding  = new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8);
            var transport = new HttpTransportBindingElement { AuthenticationScheme = AuthenticationSchemes.Digest };
            var binding   = new CustomBinding(encoding, transport);
            var endpoint  = new EndpointAddress($"http://{address}/vapix/services");

            var client = new ActionClient(binding, endpoint);

            //client.ClientCredentials.HttpDigest.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            client.ClientCredentials.HttpDigest.ClientCredential.UserName = userName;
            client.ClientCredentials.HttpDigest.ClientCredential.Password = password;
            client.ClientCredentials.HttpDigest.ClientCredential.Domain   = "localhost";
            client.ClientCredentials.UserName.UserName                    = userName;
            client.ClientCredentials.UserName.Password                    = password;

            return client;
        }

        /// <summary>
        ///     Creates an event service client with the specified address and the provided credentials.
        /// </summary>
        public static EventClient CreateEventServiceClient(string address, string userName, string password)
        {
            var encoding  = new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8);
            var transport = new HttpTransportBindingElement { AuthenticationScheme = AuthenticationSchemes.Digest };
            var binding   = new CustomBinding(encoding, transport);
            var endpoint  = new EndpointAddress($"http://{address}/vapix/services");

            var client = new EventClient(binding, endpoint);

            // client.ClientCredentials.HttpDigest.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            client.ClientCredentials.HttpDigest.ClientCredential.UserName = userName;
            client.ClientCredentials.HttpDigest.ClientCredential.Password = password;
            client.ClientCredentials.HttpDigest.ClientCredential.Domain   = "localhost";
            client.ClientCredentials.UserName.UserName                    = userName;
            client.ClientCredentials.UserName.Password                    = password;

            return client;
        }
    }
}
