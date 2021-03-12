using System;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result
        /// </returns>
        private async Task<TResult> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                //check whether the plugin is active
                if (!await IsPluginActiveAsync())
                    return default;

                //invoke function
                return await function();
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
                var customer = await _workContext.GetCurrentCustomerAsync();
                var error = $"{CartStackDefaults.SystemName} error: {Environment.NewLine}{exception.Message}";
                await _logger.ErrorAsync(error, exception, customer);

                return default;
            }
        }

        /// <summary>
        /// Check whether the plugin is active for the current customer and the current store
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the check result
        /// </returns>
        private async Task<bool> IsPluginActiveAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            return await _widgetPluginManager.IsPluginActiveAsync(CartStackDefaults.SystemName, customer, store.Id);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tracking code
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tracking code
        /// </returns>
        public async Task<string> PrepareTrackingCodeAsync()
        {
            return await HandleFunctionAsync(() =>
            {
                return Task.FromResult(_cartStackSettings.TrackingCode);
            });
        }

        /// <summary>
        /// Confirm that order has taken place
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ConfirmTrackingAsync(Order order)
        {
            await HandleFunctionAsync(async () =>
            {
                //check whether the purchase was initiated by the customer
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (order?.CustomerId != customer.Id)
                    return false;

                //whether server side API is enabled
                if (!_cartStackSettings.UseServerSideApi || string.IsNullOrEmpty(_cartStackSettings.SiteId) || string.IsNullOrEmpty(_cartStackSettings.ApiKey))
                    return false;

                //confirm tracking
                var address = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                var email = address?.Email;
                if (string.IsNullOrEmpty(email))
                    email = customer.Email;
                await _cartStackHttpClient.ConfirmTrackingAsync(email, order.OrderTotal);

                return true;
            });
        }

        #endregion
    }
}