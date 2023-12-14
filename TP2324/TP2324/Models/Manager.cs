using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace TP2324.Models
{
	public class Manager
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public bool Available { get; set; }

        [ForeignKey("company")]
        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public List<Renting> Rentings { get; set; } = new List<Renting>();
    }
}

