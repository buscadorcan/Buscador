namespace SharedApp.Data
{
    public class TypeSearch
    {
        public List<Item> ListTypeSearch { get; }

        public TypeSearch()
        {
            ListTypeSearch = new List<Item>
            {
                new Item(0, "Buscar exacta"),
                new Item(1, "Buscar palabra"),
                new Item(2, "Buscar frase"),
                new Item(3, "Buscar por palabras"),
                new Item(4, "Buscar con sinonimos"),
                new Item(5, "Buscar vectorizacion")
            };
        }
    }

    public record Item(int Id, string Name);
}