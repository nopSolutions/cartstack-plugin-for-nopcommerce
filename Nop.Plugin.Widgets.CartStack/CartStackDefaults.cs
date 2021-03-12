using Nop.Core;

namespace Nop.Plugin.Widgets.CartStack
{
    /// <summary>
    /// Represents plugin default vaues and constants
    /// </summary>
    public class CartStackDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Widgets.CartStack";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopcommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the server side API URL
        /// </summary>
        public static string ServerSideApiUrl => "https://api.cartstack.com/ss/v1/";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Widgets.CartStack.Configure";

        /// <summary>
        /// Gets the name of the view component to place a widget into pages
        /// </summary>
        public const string VIEW_COMPONENT = "CartStack";
    }
}