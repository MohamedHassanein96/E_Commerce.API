using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var hasher = new PasswordHasher<IdentityUser>();
            var user = new IdentityUser(); // أو ApplicationUser لو عندك كلاس مخصص

            string hashedPassword = hasher.HashPassword(user, "Tarek@4697");

            Console.WriteLine(hashedPassword);

        }
    }
}
