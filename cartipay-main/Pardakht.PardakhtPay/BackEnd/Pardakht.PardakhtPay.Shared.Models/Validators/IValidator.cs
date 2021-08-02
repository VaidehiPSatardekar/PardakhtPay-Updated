namespace Pardakht.PardakhtPay.Shared.Models.Validators
{
    public interface IValidator
    {
        IValidator Next { get; set; }

        bool Validate();
    }

    public interface IValidator<T> : IValidator
    {
        T ExpectedValue { get; set; }

        T ActualValue { get; set; }
    }
}
