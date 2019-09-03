using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchSeeder
    {
        private readonly DutchContext _ctx;
        private readonly IHostingEnvironment _hosting;

        public DutchSeeder(DutchContext ctx, IHostingEnvironment hosting)
        {
            _ctx = ctx;
            _hosting = hosting;
        }

        public IHostingEnvironment Hosting { get; }

        public void Seed()
        {
            //if (_hosting.IsDevelopment())
            //{
            //    _ctx.Database.EnsureDeleted(); //kill the database??  Here we goooooooo!
            //}

            _ctx.Database.EnsureCreated(); //Check that the database actually exists

            if (!_ctx.Products.Any()) //Make sure there are in fact existing products
            {
                //Create sample data
                var filepath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(filepath);
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);

                _ctx.Products.AddRange(products);

                // add orders
                var order = _ctx.Orders.Where(o => o.Id == 1).FirstOrDefault();
                if (order != null)
                {
                    order.Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Product = products.First(),
                            Quantity = 5,
                            UnitPrice =  products.First().Price
                        }
                    };
                   
                }
                else
                {
                    _ctx.Add(new Order()
                    {
                        OrderDate = DateTime.UtcNow,
                        OrderNumber = "TestJace",
                        Items = new List<OrderItem>()
                           {
                                new OrderItem()
                                {
                                    Product = products.First(),
                                    Quantity = 5,
                                    UnitPrice =  products.First().Price
                                }
                            }
                    });

                    
                }

                _ctx.SaveChanges();
            }
        }
    }
}
