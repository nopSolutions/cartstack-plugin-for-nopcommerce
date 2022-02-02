using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.CartStack
{
    /// <summary>
    /// Represents the plugin implementation
    /// </summary>
    public class CartStackPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly CartStackSettings _cartStackSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly WidgetSettings _widgetSettings;


        #endregion

        #region Ctor

        public CartStackPlugin(CartStackSettings cartStackSettings,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory,
            WidgetSettings widgetSettings)
        {
            _cartStackSettings = cartStackSettings;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(CartStackDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { _cartStackSettings.WidgetZone });
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == null)
                throw new ArgumentNullException(nameof(widgetZone));

            return CartStackDefaults.VIEW_COMPONENT;
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new CartStackSettings
            {
                WidgetZone = PublicWidgetZones.HeadHtmlTag
            });

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.CartStack.ThankYouPage.Warning"] = "It looks like you have <a href=\"{0}\" target=\"_blank\">DisableOrderCompletedPage</a> setting enabled, so this can lead to incorrect tracking of purchases. You should uncheck this setting, if you don't, CartStack won't know to stop the reminder email from being sent.",
                ["Plugins.Widgets.CartStack.Fields.TrackingCode"] = "Tracking Code",
                ["Plugins.Widgets.CartStack.Fields.TrackingCode.Hint"] = "Find your unique Tracking Code snippet on the Code Installation page of your account and then copy it into this field.",
                ["Plugins.Widgets.CartStack.Fields.TrackingCode.Required"] = "Tracking code is required",
                ["Plugins.Widgets.CartStack.Fields.SiteId"] = "Site ID",
                ["Plugins.Widgets.CartStack.Fields.SiteId.Hint"] = "Find your Site ID on the Code Installation page of your account and then copy it into this field.",
                ["Plugins.Widgets.CartStack.Fields.SiteId.Required"] = "Site ID is required for server side integration",
                ["Plugins.Widgets.CartStack.Fields.ApiKey"] = "API Key",
                ["Plugins.Widgets.CartStack.Fields.ApiKey.Hint"] = "You need to generate a unique API key on the Code Installation page of your account and then copy it into this field.",
                ["Plugins.Widgets.CartStack.Fields.ApiKey.Required"] = "API Key is required for server side integration",
                ["Plugins.Widgets.CartStack.Fields.UseServerSideApi"] = "Use server side integration",
                ["Plugins.Widgets.CartStack.Fields.UseServerSideApi.Hint"] = "Quite simply, the server side integration is more accurate than the client side confirmation tracking. Since the client side tracking relies on the end user's browser settings, a small percentage of conversions may be missed. The server side integration should be 100% accurate."
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(CartStackDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
            await _settingService.DeleteSettingAsync<CartStackSettings>();

            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.CartStack");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion
    }
}