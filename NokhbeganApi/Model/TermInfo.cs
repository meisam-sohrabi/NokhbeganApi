namespace NokhbeganApi.Model
{
    public class TermInfo
    {
        public Guid TermId { get; set; }
        public string BookName { get; set; }
        public int CurrentLevel { get; set; }
        public int HistoryOfTerm { get; set; }
        public int Price { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
    }
}
