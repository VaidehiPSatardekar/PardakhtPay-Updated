namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public abstract class BaseValidator<T> : IValidator<T>
    {
        public IValidator Next { get; set; }

        public T ExpectedValue { get; set; }

        public T ActualValue { get; set; }

        public bool Validate()
        {
            var valid = OnValidate();

            if(Next == null || !valid)
            {
                return valid;
            }

            return valid && Next.Validate();
        }

        protected abstract bool OnValidate();
    }

    public abstract class BaseDecimalValidator : BaseValidator<decimal>
    {

    }
}
