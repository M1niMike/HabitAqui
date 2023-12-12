using System;
namespace TP2324.Models
{
    public class Employee
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public bool? Available { get; set; }




        //RELACIONAMENTOS

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

