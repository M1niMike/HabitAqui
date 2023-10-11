using System;
namespace TP2324.Models
{
	public class Client
    {
        public int Id { get; set; } // Primary key
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int CitizenCard { get; set; }
        public int Nif { get; set; }
    }
}

