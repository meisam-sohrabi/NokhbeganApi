namespace NokhbeganApi.Model
{
    public class ResponseVM
    {
        public string Message { get; set; } = null!;

        public int StatusCode { get; set; }
        public bool isSuccess { get; set; }

        public object? Data { get; set; }
        public IEnumerable<string>? Error { get; set; }
        public string? Role { get; set; }
    }
}
