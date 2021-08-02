using System;

namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public abstract class BaseDateTimeValidator : BaseValidator<DateTime?>
    {
        public int ExpectedDayCount { get; set; }

        protected int ActualDayCount
        {
            get
            {
                if(ActualValue == null || ExpectedValue == null)
                {
                    return int.MaxValue;
                }

                return ExpectedValue.Value.Date.Subtract(ActualValue.Value.Date).Days;
            }
        }
    }
}
