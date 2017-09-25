using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Sample.Api.Core.Attributes;
using Sample.Api.Core.Models;
using Sample.Api2.ActionFilters;

namespace Sample.Api2.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductController : ApiController
    {
       
        [HttpGet]
        [VersionedRoute("{id}", 1)]
        [ZipkinTrace]
        public async Task<IHttpActionResult> Get(int id)
        {
            return Ok(await Task.Factory.StartNew(() => ProductList.Items.FirstOrDefault(p => p.Id == id)));
        }
    }
}