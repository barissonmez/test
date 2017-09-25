using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sample.Api.Core.Models
{
    public static class ProductList
    {
        public static List<Product> Items
        {
            get
            {
                using (WebClient wc = new WebClient())
                {
                    string json = wc.DownloadString(
                        "https://raw.githubusercontent.com/reactjs/redux/master/examples/shopping-cart/src/api/products.json");

                    return JsonConvert.DeserializeObject<List<Product>>(json);
                }
            }


        }
    }
}
