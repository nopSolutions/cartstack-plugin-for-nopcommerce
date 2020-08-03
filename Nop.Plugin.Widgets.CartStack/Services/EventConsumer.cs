using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Plugin.Widgets.CartStack.Services
{
    /// <summary>
    /// Represents the plugin event consumer
    /// </summary>
    public class EventConsumer : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly CartStackService _cartStackService;

        #endregion

        #region Ctor

        public EventConsumer(CartStackService cartStackService)
        {
            _cartStackService = cartStackService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            _cartStackService.ConfirmTracking(eventMessage?.Order);
        }

        #endregion
    }
}