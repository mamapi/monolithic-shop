﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Data;

namespace MicroShop.Controllers
{
    public class AdminController: Controller
    {
        [HttpGet]
        [Route("/initdb")]
        public bool Init()
        {
            var mockedListOfProducts = new List<Product>
            {
                new Product(10.0m, "Bike", 1),
                new Product(5.0m, "Scooter", 2),
                new Product(4.99m, "Ball", 3),
                new Product(89.99m, "Helmet", 4)
            };

            var mockedListOfWarehouse = new List<ProductWarehouse>
            {
                new ProductWarehouse(mockedListOfProducts[0], 10),
                new ProductWarehouse(mockedListOfProducts[1], 10),
                new ProductWarehouse(mockedListOfProducts[2], 10),
                new ProductWarehouse(mockedListOfProducts[3], 10)
            };

            using (var context = new MainDatabaseContext())
            {
                foreach (var mockedListOfProduct in mockedListOfProducts)
                {
                    context.Products.Add(mockedListOfProduct);
                }

                foreach (var mockedWar in mockedListOfWarehouse)
                {
                    context.ProductsWarehouse.Add(mockedWar);
                }

                context.SaveChanges();

                return true;
            }
        }
    }
}
