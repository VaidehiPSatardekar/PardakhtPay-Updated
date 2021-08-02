namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class SeedDataSettings
    {
        public bool Enabled { get; set; }
        public bool CreateDatabaseOnly { get; set; }
        public bool ClearExistingData { get; set; }
        public bool DropAndRecreateMainDb { get; set; }
    }
}
