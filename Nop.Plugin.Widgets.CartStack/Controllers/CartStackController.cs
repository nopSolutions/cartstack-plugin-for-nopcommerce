using System.Threading.Tasks;
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

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CartStackSettings>(storeId);

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
                model.TrackingCode_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TrackingCode, storeId);
                model.SiteId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.SiteId, storeId);
                model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.ApiKey, storeId);
                model.UseServerSideApi_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseServerSideApi, storeId);
            }

            //display a warning when the 'thank you' page is disabled
            if (_orderSettings.DisableOrderCompletedPage)
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Widgets.CartStack.ThankYouPage.Warning");
                _notificationService.WarningNotification(string.Format(locale, Url.Action("Order", "Setting")), false);
            }

            return View("~/Plugins/Widgets.CartStack/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CartStackSettings>(storeId);

            settings.TrackingCode = model.TrackingCode;
            settings.SiteId = model.SiteId;
            settings.ApiKey = model.ApiKey;
            settings.UseServerSideApi = model.UseServerSideApi;

            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.TrackingCode, model.TrackingCode_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SiteId, model.SiteId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ApiKey, model.ApiKey_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.UseServerSideApi, model.UseServerSideApi_OverrideForStore, storeId, false);

            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}