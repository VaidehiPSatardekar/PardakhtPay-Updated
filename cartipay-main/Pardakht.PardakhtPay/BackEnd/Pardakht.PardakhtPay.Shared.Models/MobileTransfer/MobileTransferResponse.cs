using System;

namespace Pardakht.PardakhtPay.Shared.Models.MobileTransfer
{
    public class MobileTransferResponse
    {
        public MobileTransferResult Result { get; set; }

        public MobileTransferError Error { get; set; }

        public bool IsSuccess
        {
            get
            {
                return Error == null && Result != null;
            }
        }

        public void CheckError()
        {
            if(Error != null)
            {
                throw new Exception(Error.Desc);
            }

            if(Result == null)
            {
                throw new Exception("Result is null");
            }
        }
    }

    public class MobileTransferResult
    {
        public int Id { get; set; }

        public string Msg { get; set; }
    }

    public class MobileTransferError
    {
        public int Code { get; set; }

        public string Desc { get; set; }

        public string UniqueId { get; set; }

        public bool IsProcessed { get; set; }
    }
}
