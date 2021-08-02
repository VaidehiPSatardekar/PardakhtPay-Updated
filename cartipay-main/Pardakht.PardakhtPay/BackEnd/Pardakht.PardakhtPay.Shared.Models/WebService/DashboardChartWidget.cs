using System.Collections.Generic;
using System.ComponentModel;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DashboardChartWidget
    {/// <summary>
     /// Gets or sets Ranges
     /// </summary>
     /// 
        [Description("Widget Ranges")]
        public Dictionary<string, string> Ranges { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        /// 
        [Description("Widget Tile")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Data
        /// </summary>
        /// 
        [Description("Widget Chart List")]
        public Dictionary<string, List<WidgetChartSeries>> MainChart { get; set; }

        /// <summary>
        /// Gets or sets Current Range
        /// </summary>        
        /// 
        [Description("Requested Range")]
        public string CurrentRange { get; set; }

        /// <summary>
        /// Gets or sets xAxis
        /// </summary>
        /// 
        [Description("show or hide the x axis")]
        public bool XAxis { get; set; }

        /// <summary>
        /// Gets or sets yAxis
        /// </summary>
        /// 
        [Description("show or hide the y axis")]
        public bool YAxis { get; set; }

        /// <summary>
        /// Gets or sets gradient
        /// </summary>
        /// 
        [Description("fill elements with a gradient instead of a solid color")]
        public bool Gradient { get; set; }

        /// <summary>
        /// Gets or sets legend
        /// </summary>
        /// 
        [Description("show or hide the legend")]
        public bool Legend { get; set; }

        /// <summary>
        /// Gets or sets showXAxisLabel
        /// </summary>
        /// 
        [Description("show or hide the x axis label")]
        public bool ShowXAxisLabel { get; set; }

        /// <summary>
        /// Gets or sets xAxisLabel
        /// </summary>
        /// 
        [Description("the x axis label text")]
        public string XAxisLabel { get; set; }

        /// <summary>
        /// Gets or sets showYAxisLabe
        /// </summary>
        /// 
        [Description("show or hide the y axis label")]
        public bool ShowYAxisLabel { get; set; }

        /// <summary>
        /// Gets or sets yAxisLabel
        /// </summary>
        /// 
        [Description("the y axis label text")]
        public string YAxisLabel { get; set; }

        /// <summary>
        /// Gets or sets domain
        /// </summary>
        /// 
        [Description("the color scheme of the chart")]
        public Scheme Scheme { get; set; }

        /// <summary>
        /// Gets or sets ExplodeSlices
        /// </summary>
        /// 
        [Description("make the radius of each slice proportional to it's value")]
        public bool ExplodeSlices { get; set; }

        /// <summary>
        /// Gets or sets Labels
        /// </summary>
        /// 
        [Description("show or hide the labels")]
        public bool Labels { get; set; }

        /// <summary>
        /// Gets or sets Doughnut
        /// </summary>
        /// 
        [Description("should doughnut instead of pie slices")]
        public bool Doughnut { get; set; }

        /// <summary>
        /// Gets or sets FooterLeft
        /// </summary>
        /// 
        [Description("left footer information")]
        public FooterForPie FooterLeft { get; set; }

        /// <summary>
        /// Gets or sets FooterRight
        /// </summary>
        /// 
        [Description("right footer information")]
        public FooterForPie FooterRight { get; set; }

        public List<CustomColor> CustomColors { get; set; }

        [Description("Key and Value of the Chart Data")]
        public IEnumerable<object> NonChartData { get; set; }
    }

    public class CustomColor
    {
        [Description("Label of the Line")]
        public string Name { get; set; }
        [Description("Color value (green, red etc.)")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Gets or sets chart data
    /// </summary>
    public class WidgetChartData
    {
        /// <summary>
        /// Gets or Ranges
        /// </summary>
        /// 
        [Description("List of the Widget Chart Series")]
        public Dictionary<string, List<WidgetChartSeries>> ChartRanges { get; set; }
    }

    /// <summary>
    /// Gets or sets chart series
    /// </summary>
    public class WidgetChartSeries
    {
        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Ranges
        /// </summary>
        public List<WidgetChartSeriesData> Series { get; set; }
    }

    public class WidgetChartSeriesData
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        // this can hold any extra info for a series eg a count if value is used for a sum
        public decimal Extra { get; set; }
    }

    /// <summary>
    /// Gets or sets chart series data
    /// </summary>
    /// 
    public class Scheme
    {
        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public List<string> Domain { get; set; }
    }

    /// <summary>
    /// Gets or sets chart series data
    /// </summary>
    public class FooterForPie
    {
        /// <summary>
        /// Gets or sets Title
        /// </summary>
        /// 
        [Description("Title of the Data")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Count
        /// </summary>
        /// 
        [Description("Value of the Key")]
        public Dictionary<string, string> Count { get; set; }
    }
}
