using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.CartStack
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class CartStackSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a tracking code
        /// </summary>
        public string TrackingCode { get; set; }

        /// <summary>
        /// Gets or sets a site ID
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Gets or sets an API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use server side API
        /// </summary>
        public bool UseServerSideApi { get; set; }

        /// <summary>
        /// Gets or sets a widget zone name to place a widget
        /// </summary>
        public string WidgetZone { get; set; }
    }
}