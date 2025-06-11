using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class ShowNotificationVM
    {
        public Guid ID_Notification { get; set; }
        public string Title { get; set; }
        [StringLength(500)]
        public string Message { get; set; }
        public bool IsRead { get; set; }

    }
}
