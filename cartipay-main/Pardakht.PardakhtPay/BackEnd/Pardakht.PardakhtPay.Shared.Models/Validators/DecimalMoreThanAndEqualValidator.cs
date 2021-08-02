namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DecimalMoreThanAndEqualValidator : BaseDecimalValidator
    {
        protected override bool OnValidate()
        {
            return ActualValue >= ExpectedValue;
        }
    }
}
