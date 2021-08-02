using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Stage
{
    public class AppSettingModel
    {
        [Required]
        public string KeyName { get; set; }
        [Required]
        public string KeyValue { get; set; }
    }

    public class MachineModel
    {
        [Required]
        public string Name { get; set; }
    }
}
