namespace TP2324.Models
{
    public class Home
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public float Price { get; set; }
        public int NumRooms { get; set; }
        public int NumWC { get; set; }
        public string Address { get; set; }
        public float SquareFootage { get; set; }
        public int NumParks { get; set; }
        public bool Wifi { get; set; }
        public string Description { get; set; }
    }
}
