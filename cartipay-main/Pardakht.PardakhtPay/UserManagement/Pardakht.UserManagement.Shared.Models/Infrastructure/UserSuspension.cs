using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class UserSuspension : EntityBase
    {
        public string Reason { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public StaffUser StaffUser { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public StaffUser CreatedBy { get; set; }
    }
}
