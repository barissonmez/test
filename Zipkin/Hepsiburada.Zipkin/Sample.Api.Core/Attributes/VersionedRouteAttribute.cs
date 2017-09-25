using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using Sample.Api.Core.Models;

namespace Sample.Api.Core.Attributes
{
    public class VersionedRouteAttribute : RouteFactoryAttribute
    {
        private int _AllowedVersion;

        public VersionedRouteAttribute(string template, int allowedVersion)
            : base(template)
        {
            _AllowedVersion = allowedVersion;
        }

        public override IDictionary<String, Object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary();
                constraints.Add("version", new VersionConstraint(_AllowedVersion));

                return constraints;
            }
        }
    }
}
