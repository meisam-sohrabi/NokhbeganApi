using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class CreateNotificationVM
    {
        public string Title { get; set; }
        [StringLength(500)]
        public string Message { get; set; }
    }
}
