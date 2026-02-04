namespace EnsinoApp.Models.Entities
{
    public class Supervisao
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public int IdCampus { get; set; }

        public Campus Campus { get; set; } = null!;
        public ICollection<Usuario> Lideres { get; set; } = new List<Usuario>();
    }
}