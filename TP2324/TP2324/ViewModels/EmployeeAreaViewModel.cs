using TP2324.Models;

namespace TP2324.ViewModels
{
    public class EmployeeAreaViewModel
    {
        public Home? homes { get; set; }

        //public EmployeeAreaViewModel()
        //{
        //    homes = new Home();
        //}

        public decimal? Price { get; set; }
        public int numWc { get; set; }
        public string Address { get; set; }
        public float SquareFootage { get; set; }
        public int numParks { get; set; }
        public bool Wifi { get; set; }
        public string Description { get; set; }
        public int minimumPeriod { get; set; }
        public int? ratings { get; set; }
        public bool available { get; set; }
        public string? ImgUrl { get; set; }
        public int categoryId { get; set; }
        public int typeResidenceId { get; set; }
        public int districtId { get; set; }


    }
}
