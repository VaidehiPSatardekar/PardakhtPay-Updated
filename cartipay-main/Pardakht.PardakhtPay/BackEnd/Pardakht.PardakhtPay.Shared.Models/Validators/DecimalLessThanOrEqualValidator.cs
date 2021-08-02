namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DecimalLessThanOrEqualValidator : BaseDecimalValidator
    {
        protected override bool OnValidate()
        {
            return ActualValue <= ExpectedValue;
        }
    }
}
