using System.Collections.Generic;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
        public EmailAccount From { get; set; }
        public EmailAccount To { get; set; }
        public EmailAccount ReplyTo { get; set; }
        public List<EmailAttachment> Attachments { get; set; }
    }

    public class EmailAccount
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] File { get; set; }
    }
}
