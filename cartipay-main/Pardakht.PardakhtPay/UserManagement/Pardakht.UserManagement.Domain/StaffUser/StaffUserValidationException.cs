using System;

namespace Pardakht.UserManagement.Domain.StaffUser
{
    public class StaffUserValidationException : Exception
    {
        public StaffUserValidationException(string message) : base(message)
        {

        }
    }
}
