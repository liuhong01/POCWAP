//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api.Controllers
{
    public class ProductsController : ApiController
    {
        public static List<Product> products;

        static ProductsController()
        {
            products = new List<Product>();
            products.Add(new Product { ProductId = 0, ProductName = "XBOX", ProductPrice = 100, ExpiryDate = DateTime.Now.AddDays(2), NumberOfUnits = 100 });
            products.Add(new Product { ProductId = 1, ProductName = "Kinect", ProductPrice = 200, ExpiryDate = DateTime.Now.AddDays(4), NumberOfUnits = 200 });
            products.Add(new Product { ProductId = 1, ProductName = "Surface", ProductPrice = 300, ExpiryDate = DateTime.Now.AddDays(10), NumberOfUnits = 500 });
        }        

        [HttpGet]
        public List<Product> GetProductList()
        {
           return products;
        }

        [HttpPut]
        public void UpdateProduct(Product product)
        {
            if (product == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.ProductEmpty);
            }

            var prod = (from p in products where p.ProductId == product.ProductId select p).FirstOrDefault();

            if (prod != null)
            {
                prod.ProductName = product.ProductName;
                prod.ProductPrice = product.ProductPrice;
                prod.ExpiryDate = product.ExpiryDate;
                prod.NumberOfUnits = product.NumberOfUnits;
            }
        }

        [HttpPost]
        public void AddProduct(Product product)
        {
            if (product == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.ProductEmpty);
            }

            products.Add(new Product
            {
                ProductId = products.Count,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ExpiryDate = product.ExpiryDate,
                NumberOfUnits = product.NumberOfUnits
            });
        }     
    }
}
