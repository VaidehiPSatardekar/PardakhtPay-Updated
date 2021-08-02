namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DateTimeMoreThanValidator : BaseDateTimeValidator
    {
        protected override bool OnValidate()
        {
            if (ActualValue == null || ExpectedValue == null)
            {
                return false;
            }
            return ActualDayCount > ExpectedDayCount;
        }
    }
}
