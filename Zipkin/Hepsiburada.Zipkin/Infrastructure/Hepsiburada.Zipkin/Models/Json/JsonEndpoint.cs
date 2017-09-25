using System;
using Hepsiburada.Zipkin.Models.Endpoints;
using Newtonsoft.Json;

namespace Hepsiburada.Zipkin.Models.Json
{
    internal class JsonEndpoint
    {
        private readonly Endpoint endpoint;

        [JsonProperty("ipv4")]
        public string IPv4 => endpoint.IPAddress.ToString();

        [JsonProperty("port")]
        public ushort Port => endpoint.Port;

        [JsonProperty("serviceName")]
        public string ServiceName => endpoint.ServiceName;

        public JsonEndpoint(Endpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            this.endpoint = endpoint;
        }
    }
}