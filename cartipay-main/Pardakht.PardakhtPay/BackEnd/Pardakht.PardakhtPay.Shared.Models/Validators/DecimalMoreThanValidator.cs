namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DecimalMoreThanValidator : BaseDecimalValidator
    {
        protected override bool OnValidate()
        {
            return ActualValue > ExpectedValue;
        }
    }
}
