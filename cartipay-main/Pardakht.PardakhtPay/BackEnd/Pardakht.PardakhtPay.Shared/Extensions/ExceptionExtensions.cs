using System;
using System.Text;

namespace Pardakht.PardakhtPay.Shared.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetExceptionDetails(this Exception ex)
        {
            StringBuilder str = new StringBuilder();

            Exception innerException = ex;

            while(innerException != null)
            {
                str.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }

            return str.ToString();
        }

        public static string GetExceptionDetailsWithStackTrace(this Exception ex)
        {
            StringBuilder str = new StringBuilder();

            Exception innerException = ex;

            while (innerException != null)
            {
                str.AppendLine(string.Format("{0} # {1}", innerException.Message, innerException.StackTrace));
                innerException = innerException.InnerException;
            }

            return str.ToString();
        }
    }
}
