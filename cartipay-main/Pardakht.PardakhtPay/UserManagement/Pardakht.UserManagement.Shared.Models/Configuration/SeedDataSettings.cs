namespace Pardakht.UserManagement.Shared.Models.Configuration
{
    public class SeedDataSettings
    {
        public bool Enabled { get; set; }
        public bool SeedUserData { get; set; }
        public bool SeedTenantData { get; set; }
        public bool SeedProviderData { get; set; }
        public bool SeedDomainData { get; set; }
        public bool ClearExistingData { get; set; }
        public bool DropAndRecreateMainDb { get; set; }
        public bool SeedRoleData { get; set; }
        public bool SeedTicketData { get; set; }
        public bool SeedTicketCategoryData { get; set; }
        public bool SeedProductData { get; set; }
        public bool SeedLanguageData { get; set; }
        public bool SeedRegionData { get; set; }
        public bool SeedPageThemeData { get; set; }
        public bool SeedPageData { get; set; }
        public bool SeedDictionaryData { get; set; }
        public bool CreateDatabaseOnly { get; set; }
        public bool DropAndRecreateDb { get; set; }
    }
}
