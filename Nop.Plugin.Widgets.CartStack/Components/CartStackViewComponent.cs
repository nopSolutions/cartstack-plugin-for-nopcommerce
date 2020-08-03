﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Widgets.CartStack.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.CartStack.Components
{
    /// <summary>
    /// Represents the view component to place a widget into pages
    /// </summary>
    [ViewComponent(Name = CartStackDefaults.VIEW_COMPONENT)]
    public class CartStackViewComponent : NopViewComponent
    {
        #region Fields

        private readonly CartStackService _cartStackService;
        private readonly CartStackSettings _cartStackSettings;

        #endregion

        #region Ctor

        public CartStackViewComponent(CartStackService cartStackService,
            CartStackSettings cartStackSettings)
        {
            _cartStackService = cartStackService;
            _cartStackSettings = cartStackSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var script = widgetZone != _cartStackSettings.WidgetZone
                ? string.Empty
                : _cartStackService.PrepareTrackingCode();

            return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
        }

        #endregion
    }
}