namespace TP2324.Models
{
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }

        public List<Home> Homes { get; set; }
    }
}
