namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public class StringEqualsValidator : BaseStringValidator
    {
        protected override bool OnValidate()
        {
            if(string.IsNullOrEmpty(ActualValue) || string.IsNullOrEmpty(ExpectedValue))
            {
                return ActualValue == ExpectedValue;
            }

            return ActualValue.ToLowerInvariant() == ExpectedValue.ToLowerInvariant();
        }
    }
}
