using System.ComponentModel.DataAnnotations;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    // copied from communications repo
    public class EmailServiceRequest
    {
        [Required]
        public Email FromEmail { get; set; }
        [Required]
        public Email ToEmail { get; set; }
        public Email ReplyTo { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string Subject { get; set; }
        public bool? IsBodyHtml { get; set; }
    }

    public class Email
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name Should be minimum 3 characters and a maximum of 100 characters")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }

    public class EmailServiceResponse
    {
        public bool IsSent { get; set; }
    }
}
