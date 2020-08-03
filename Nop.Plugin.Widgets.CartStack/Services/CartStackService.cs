using System;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Logging;

namespace Nop.Plugin.Widgets.CartStack.Services
{
    /// <summary>
    /// Represents the plugin service implementation
    /// </summary>
    public class CartStackService
    {
        #region Fields

        private readonly CartStackHttpClient _cartStackHttpClient;
        private readonly CartStackSettings _cartStackSettings;
        private readonly IAddressService _addressService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CartStackService(CartStackHttpClient cartStackHttpClient,
            CartStackSettings cartStackSettings,
            IAddressService addressService,
            ILogger logger,
            IStoreContext storeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _cartStackHttpClient = cartStackHttpClient;
            _cartStackSettings = cartStackSettings;
            _addressService = addressService;
            _logger = logger;
            _storeContext = storeContext;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <returns>Result</returns>
        private TResult HandleFunction<TResult>(Func<TResult> function)
        {
            try
            {
                //check whether the plugin is active
                if (!PluginActive())
                    return default;

                //invoke function
                return function();
            }
            catch (Exception exception)
            {
                //get a short error message
                var detailedException = exception;
                do
                {
                    detailedException = detailedException.InnerException;
                } while (detailedException?.InnerException != null);


                //log errors
                var error = $"{CartStackDefaults.SystemName} error: {Environment.NewLine}{exception.Message}";
                _logger.Error(error, exception, _workContext.CurrentCustomer);

                return default;
            }
        }

        /// <summary>
        /// Check whether the plugin is active for the current customer and the current store
        /// </summary>
        /// <returns>Result</returns>
        private bool PluginActive()
        {
            return _widgetPluginManager.IsPluginActive(CartStackDefaults.SystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tracking code
        /// </summary>
        /// <returns>Tracking code</returns>
        public string PrepareTrackingCode()
        {
            return HandleFunction(() =>
            {
                return _cartStackSettings.TrackingCode;
            });
        }

        /// <summary>
        /// Confirm that order has taken place
        /// </summary>
        /// <param name="order">Order</param>
        public void ConfirmTracking(Order order)
        {
            HandleFunction(() =>
            {
                //check whether the purchase was initiated by the customer
                if (order?.CustomerId != _workContext.CurrentCustomer.Id)
                    return false;

                //whether server side API is enabled
                if (!_cartStackSettings.UseServerSideApi || string.IsNullOrEmpty(_cartStackSettings.SiteId) || string.IsNullOrEmpty(_cartStackSettings.ApiKey))
                    return false;

                //confirm tracking
                var email = _addressService.GetAddressById(order.BillingAddressId)?.Email;
                if (string.IsNullOrEmpty(email))
                    email = _workContext.CurrentCustomer.Email;
                _cartStackHttpClient.ConfirmTrackingAsync(email, order.OrderTotal).Wait();

                return true;
            });
        }

        #endregion
    }
}