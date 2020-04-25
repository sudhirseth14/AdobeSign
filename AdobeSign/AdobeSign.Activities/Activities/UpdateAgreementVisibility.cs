using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AdobeSign.Activities.Properties;
using AdobeSign.Activities.Scope;
using AdobeSign.Contracts;
using AdobeSign.Enums;
using AdobeSign.Models;
using AdobeSign.Services;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace AdobeSign.Activities.Agreements
{
    [LocalizedDisplayName(nameof(Resources.UpdateAgreementVisibility_DisplayName))]
    [LocalizedDescription(nameof(Resources.UpdateAgreementVisibility_Description))]
    public class UpdateAgreementVisibility : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateAgreementVisibility_AgreementId_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateAgreementVisibility_AgreementId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> AgreementId { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateAgreementVisibility_VisibilityStatus_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateAgreementVisibility_VisibilityStatus_Description))]
        [TypeConverter(typeof(EnumNameConverter<VisibilityStatus>))]
        public VisibilityStatus VisibilityStatus { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateAgreementVisibility_Response_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateAgreementVisibility_Response_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Response { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public UpdateAgreementVisibility()
        {
            Constraints.Add(ActivityConstraints.HasParentType<UpdateAgreementVisibility, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (AgreementId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(AgreementId)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var agreementId = AgreementId.Get(context);
            bool response = false;
            var visibilityStatus = Enum.GetName(typeof(VisibilityStatus), (int)VisibilityStatus);
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string inputjson = "{\"visibility\": \"" + visibilityStatus + "\"}";
            string VisibilityStatusReposnse = await adobeObject.UpdateAgreementVisibility(inputjson, agreementId);
            response = true;
            // Outputs
            return (ctx) => {
                Response.Set(ctx, response);
            };
        }

        #endregion
    }
}

