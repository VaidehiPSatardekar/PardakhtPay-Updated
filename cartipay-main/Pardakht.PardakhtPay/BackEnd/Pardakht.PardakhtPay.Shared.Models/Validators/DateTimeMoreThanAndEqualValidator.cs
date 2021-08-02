namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DateTimeMoreThanAndEqualValidator : BaseDateTimeValidator
    {
        protected override bool OnValidate()
        {
            if (ActualValue == null || ExpectedValue == null)
            {
                return false;
            }
            return ActualDayCount >= ExpectedDayCount;
        }
    }
}
