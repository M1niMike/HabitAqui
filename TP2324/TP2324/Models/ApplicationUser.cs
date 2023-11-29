using Microsoft.AspNetCore.Identity;

namespace TP2324.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int NIF { get; set; }
    }
}
