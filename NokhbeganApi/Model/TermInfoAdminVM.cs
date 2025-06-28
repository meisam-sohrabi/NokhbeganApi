namespace NokhbeganApi.Model
{
    public class TermInfoAdminVM
    {
        public string BookName { get; set; }
        public int CurrentLevel { get; set; }
        public int HistoryOfTerm { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
    }
}
