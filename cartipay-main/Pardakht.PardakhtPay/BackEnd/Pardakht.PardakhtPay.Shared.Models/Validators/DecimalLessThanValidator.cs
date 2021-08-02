namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class DecimalLessThanValidator : BaseDecimalValidator
    {
        protected override bool OnValidate()
        {
            return ActualValue < ExpectedValue;
        }
    }
}
