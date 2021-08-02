using System.Collections.Generic;
using System.ComponentModel;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DashboardWidget
    {
        /// <summary>
        /// Gets or Ranges
        /// </summary>
        /// 
        [Description("Widget Date Ranges")]
        public Dictionary<string, string> Ranges { get; set; }

        /// <summary>
        /// Gets or sets Current Range
        /// </summary>        
        /// 
        [Description("Selected Date Range")]
        public string CurrentRange { get; set; }

        /// <summary>
        /// Gets or sets Detail
        /// </summary>
        /// 
        [Description("Widget Detail")]
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        /// 
        [Description("Widget Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Data
        /// </summary>
        /// 
        [Description("Widget Requested Datas")]
        public WidgetData Data { get; set; }
    }

    public class WidgetData
    {
        /// <summary>
        /// Gets or sets Core Data
        /// </summary>
        /// 
        [Description("Widget Requested Data List")]
        public List<WidgetDataCore> CoreData { get; set; }

        /// <summary>
        /// Gets or sets Extra
        /// </summary>
        /// 
        [Description("Widget Requested Summary Data List")]
        public List<WidgetDataCore> Extra { get; set; }
    }

    /// <summary>
    /// Test
    /// </summary>
    /// 
    public class WidgetDataCore
    {
        /// <summary>
        /// Gets or sets Label
        /// </summary>       
        /// 
        [Description("Widget Label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets Count
        /// </summary>
        /// 
        [Description("Widget Value")]
        public Dictionary<string, object> Count { get; set; }
    }
}
