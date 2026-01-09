using Microsoft.AspNetCore.Identity;
using ShoppingCart.EntityCore.Entities;

namespace ShoppingCart.EntityCore.Data
{
    public class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            //ensure db exists
            context.Database.EnsureCreated();

            // Seed Users
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<User>();

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "user@gmail.com",
                    PasswordHash = hasher.HashPassword(null, "User123")
                };

                context.Users.Add(user);
            }

            // Seed Products
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Id = Guid.NewGuid().ToString(), Code = "P001", Name = "Product A", Price = 1000, Balance = 100 },
                    new Product { Id = Guid.NewGuid().ToString(), Code = "P002", Name = "Product B", Price = 2000, Balance = 100 },
                    new Product { Id = Guid.NewGuid().ToString(), Code = "P003", Name = "Product C", Price = 3000, Balance = 100 }
                );
            }

            context.SaveChanges();
        }

    }
}
