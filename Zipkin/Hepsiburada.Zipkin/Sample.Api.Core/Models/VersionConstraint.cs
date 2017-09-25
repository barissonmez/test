using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace Sample.Api.Core.Models
{
    public class VersionConstraint : IHttpRouteConstraint
    {
        private const int DEFAULT_VERSION = 1;
        private int _AllowedVersion;

        public VersionConstraint(int allowedVersion)
        {
            _AllowedVersion = allowedVersion;
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                int version = GetVersionFromRequestHeader(request) ?? DEFAULT_VERSION;
                if (version == _AllowedVersion)
                {
                    return true;
                }
            }

            return false;
        }

        private int? GetVersionFromRequestHeader(HttpRequestMessage request)
        {
            IEnumerable<string> headerValues;
            string val = string.Empty;

            if (request.Headers.TryGetValues("api-version", out headerValues) && headerValues.Count() == 1)
            {
                val = headerValues.First();
            }

            int versionFromHeader;

            if (!string.IsNullOrEmpty(val) && int.TryParse(val, out versionFromHeader))
            {
                return versionFromHeader;
            }

            return null;
        }
    }
}
