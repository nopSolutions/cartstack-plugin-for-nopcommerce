using FluentValidation;
using Nop.Plugin.Widgets.CartStack.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.CartStack.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.TrackingCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.CartStack.Fields.TrackingCode.Required"));

            RuleFor(model => model.SiteId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.CartStack.Fields.SiteId.Required"))
                .When(model => model.UseServerSideApi);

            RuleFor(model => model.ApiKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Widgets.CartStack.Fields.ApiKey.Required"))
                .When(model => model.UseServerSideApi);
        }

        #endregion
    }
}