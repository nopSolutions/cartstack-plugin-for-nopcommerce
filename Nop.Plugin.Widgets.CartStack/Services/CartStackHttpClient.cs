using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Plugin.Widgets.CartStack.Services
{
    /// <summary>
    /// Represents the plugin HTTP client implementation
    /// </summary>
    public class CartStackHttpClient
    {
        #region Fields

        private readonly CartStackSettings _cartStackSettings;
        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public CartStackHttpClient(CartStackSettings cartStackSettings,
            HttpClient httpClient)
        {
            //configure client
            httpClient.Timeout = TimeSpan.FromSeconds(20);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, CartStackDefaults.UserAgent);

            _cartStackSettings = cartStackSettings;
            _httpClient = httpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Confirm that order has taken place
        /// </summary>
        /// <param name="email">Customer rmail</param>
        /// <param name="orderTotal">Placed order total</param>
        /// <returns>The asynchronous task whose result determines that request is completed</returns>
        public async Task ConfirmTrackingAsync(string email, decimal orderTotal)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    ["key"] = _cartStackSettings.ApiKey,
                    ["siteid"] = _cartStackSettings.SiteId,
                    ["email"] = email,
                    ["total"] = orderTotal.ToString("0.00", CultureInfo.InvariantCulture)
                };
                var url = QueryHelpers.AddQueryString(CartStackDefaults.ServerSideApiUrl, parameters);
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeAnonymousType(content ?? string.Empty, new { resp = 0, err_txt = string.Empty });

                //100 is success
                if (result.resp != 100)
                    throw new NopException($"Server side error. {result.resp} - {result.err_txt}");
            }
            catch (AggregateException exception)
            {
                //rethrow actual exception
                throw exception.InnerException;
            }
        }

        #endregion
    }
}