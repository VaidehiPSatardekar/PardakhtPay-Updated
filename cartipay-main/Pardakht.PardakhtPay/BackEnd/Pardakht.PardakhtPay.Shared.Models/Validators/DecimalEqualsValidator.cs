namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DecimalEqualsValidator : BaseDecimalValidator
    {
        protected override bool OnValidate()
        {
            return ActualValue == ExpectedValue;
        }
    }
}
