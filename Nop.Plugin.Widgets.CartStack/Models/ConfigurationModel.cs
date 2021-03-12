using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.CartStack.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CartStack.Fields.TrackingCode")]
        public string TrackingCode { get; set; }
        public bool TrackingCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CartStack.Fields.SiteId")]
        public string SiteId { get; set; }
        public bool SiteId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CartStack.Fields.ApiKey")]
        [DataType(DataType.Password)]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CartStack.Fields.UseServerSideApi")]
        public bool UseServerSideApi { get; set; }
        public bool UseServerSideApi_OverrideForStore { get; set; }

        #endregion
    }
}