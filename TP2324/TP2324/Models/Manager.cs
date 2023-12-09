using System;
namespace TP2324.Models
{
	public class Manager
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }


        //Relacionamentos
        public string? companyId { get; set; }
        public Company company { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

