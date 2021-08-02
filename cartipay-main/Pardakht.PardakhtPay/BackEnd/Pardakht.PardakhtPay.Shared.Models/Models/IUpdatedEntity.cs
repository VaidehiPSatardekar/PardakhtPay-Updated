using System;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    /// <summary>
    /// Defines an interface which contains Update date information
    /// </summary>
    public interface IUpdatedEntity
    {
        DateTime? UpdatedDate { get; set; }
    }
}
