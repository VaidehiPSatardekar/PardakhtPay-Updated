namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Language
{
    public class LocaleInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public void Clone(LocaleInfo copyFrom)
        {
            Id = copyFrom.Id;
            Name = copyFrom.Name;
            Code = copyFrom.Code;
        }
    }
}
