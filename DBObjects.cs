namespace Barclays.GenAIHackathon.OpenAIWrapper
{
    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; } = new List<Column>();
    }

    public class Column
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsForeignKey { get; set; }
        public string ForeignKeyTable { get; set; }
    }
}
