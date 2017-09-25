using System.Net;

namespace Hepsiburada.Zipkin.Models.Endpoints
{
    public class Endpoint
    {
        public IPAddress IPAddress { get; set; }

        public ushort Port { get; set; }

        public string ServiceName { get; set; }
    }
}
