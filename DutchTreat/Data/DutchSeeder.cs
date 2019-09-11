using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<StoreUser> _manager;

        public DutchSeeder(DutchContext ctx, IHostingEnvironment hosting, UserManager<StoreUser> manager)
        {
            _ctx = ctx;
            _hosting = hosting;
            _manager = manager;
        }

        public IHostingEnvironment Hosting { get; }

        public async Task SeedAsync()
        {
            //if (_hosting.IsDevelopment())
            //{
            //    _ctx.Database.EnsureDeleted(); //kill the database??  Here we goooooooo!
            //}

            _ctx.Database.EnsureCreated(); //Check that the database actually exists

            StoreUser user = await _manager.FindByEmailAsync("jace@jacetech.com");

            if (user == null)
            {
                user = new StoreUser()
                {
                    FirstName = "Jason",
                    LastName = "Thomson",
                    Email = "Jace@jacetech.com",
                    UserName = "jace2019"
                };

                var result = await _manager.CreateAsync(user, "P@ssw0rd!");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Unable to create user in seeder.");
                }

            }

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
                    order.User = user;
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
