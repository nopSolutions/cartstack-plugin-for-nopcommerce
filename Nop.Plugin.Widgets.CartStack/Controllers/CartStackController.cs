using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.CartStack.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.CartStack.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CartStackController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public CartStackController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            OrderSettings orderSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _orderSettings = orderSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //prepare the model
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<CartStackSettings>(storeId);
            var model = new ConfigurationModel
            {
                TrackingCode = settings.TrackingCode,
                SiteId = settings.SiteId,
                ApiKey = settings.ApiKey,
                UseServerSideApi = settings.UseServerSideApi,
                ActiveStoreScopeConfiguration = storeId
            };
            if (storeId > 0)
            {
                model.TrackingCode_OverrideForStore = _settingService.SettingExists(settings, setting => setting.TrackingCode, storeId);
                model.SiteId_OverrideForStore = _settingService.SettingExists(settings, setting => setting.SiteId, storeId);
                model.ApiKey_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ApiKey, storeId);
                model.UseServerSideApi_OverrideForStore = _settingService.SettingExists(settings, setting => setting.UseServerSideApi, storeId);
            }

            //display a warning when the 'thank you' page is disabled
            if (_orderSettings.DisableOrderCompletedPage)
            {
                _notificationService.WarningNotification(string.Format(_localizationService
                    .GetResource("Plugins.Widgets.CartStack.ThankYouPage.Warning"), Url.Action("Order", "Setting")), false);
            }

            return View("~/Plugins/Widgets.CartStack/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<CartStackSettings>(storeId);
            settings.TrackingCode = model.TrackingCode;
            settings.SiteId = model.SiteId;
            settings.ApiKey = model.ApiKey;
            settings.UseServerSideApi = model.UseServerSideApi;
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.TrackingCode, model.TrackingCode_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.SiteId, model.SiteId_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.ApiKey, model.ApiKey_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.UseServerSideApi, model.UseServerSideApi_OverrideForStore, storeId, false);
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}