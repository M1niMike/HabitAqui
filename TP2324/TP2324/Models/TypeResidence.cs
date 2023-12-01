namespace TP2324.Models
{
    public class TypeResidence
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
        //public bool? Available { get; set; }

        public List<Home> Homes { get; set; }
    }
}
