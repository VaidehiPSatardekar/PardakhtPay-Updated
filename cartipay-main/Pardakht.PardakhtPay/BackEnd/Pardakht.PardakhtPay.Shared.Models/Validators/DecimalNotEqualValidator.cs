namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DecimalNotEqualValidator : BaseDecimalValidator
    {
        protected override bool OnValidate()
        {
            return ActualValue != ExpectedValue;
        }
    }
}
