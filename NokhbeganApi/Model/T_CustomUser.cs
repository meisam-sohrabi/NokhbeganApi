using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NokhbeganApi.Model
{
    public class T_CustomUser : IdentityUser
    {
        [StringLength(25)]
        public string FirstName { get; set; }

        [StringLength(25)]
        public string LastName { get; set; }

        [StringLength(10)]
        public string? NationalId { get; set; }

        public StudentStatus? Status { get; set; }

        [StringLength(8)]

        public string? ReferralCode { get; set; }

        public string? ImageUrl { get; set; }

        public string? InvitedById { get; set; }

        public DateTime? InvitedDate { get; set; }

        [ForeignKey("InvitedById")]
        public T_CustomUser? InvitedBy { get; set; }

        public ICollection<T_CustomUser> InvitedUsers { get; set; } = new List<T_CustomUser>();
        public ICollection<T_StudentTerm> StudentTerms { get; set; } = new List<T_StudentTerm>();
        public ICollection<T_Notification> Notifications { get; set; } = new List<T_Notification>();
        public ICollection<T_Payment> Payments { get; set; } = new List<T_Payment>();
    }

    public enum StudentStatus
    { 
        PENDING,
        VERIFIED,
        REJECTED
    }

}


