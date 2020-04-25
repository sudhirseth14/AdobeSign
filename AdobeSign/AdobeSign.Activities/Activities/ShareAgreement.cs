using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using AdobeSign.Activities.Properties;
using AdobeSign.Activities.Scope;
using AdobeSign.Contracts;
using AdobeSign.Models;
using AdobeSign.Services;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace AdobeSign.Activities.Agreements
{
    [LocalizedDisplayName(nameof(Resources.ShareAgreement_DisplayName))]
    [LocalizedDescription(nameof(Resources.ShareAgreement_Description))]
    public class ShareAgreement : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ShareAgreement_AgreementId_DisplayName))]
        [LocalizedDescription(nameof(Resources.ShareAgreement_AgreementId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> AgreementId { get; set; }

        [LocalizedDisplayName(nameof(Resources.ShareAgreement_EmailIds_DisplayName))]
        [LocalizedDescription(nameof(Resources.ShareAgreement_EmailIds_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<String[]> EmailIds { get; set; }

        [LocalizedDisplayName(nameof(Resources.ShareAgreement_Message_DisplayName))]
        [LocalizedDescription(nameof(Resources.ShareAgreement_Message_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Message { get; set; }

        [LocalizedDisplayName(nameof(Resources.ShareAgreement_ShareCreationResponseList_DisplayName))]
        [LocalizedDescription(nameof(Resources.ShareAgreement_ShareCreationResponseList_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> ShareCreationResponseList { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public ShareAgreement()
        {
            Constraints.Add(ActivityConstraints.HasParentType<ShareAgreement, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (AgreementId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(AgreementId)));
            if (EmailIds == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(EmailIds)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var agreementId = AgreementId.Get(context);
            var emailIds = EmailIds.Get(context);
            var message = Message.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string emailidsjson = "";
            foreach (var emailId in emailIds)
            {
                if (emailidsjson != "")
                    emailidsjson = emailidsjson + "," + "{\"email\":\"" + emailId + "\",\"message\": \"" + message + "\"}";
                else
                    emailidsjson = "{\"email\":\"" + emailId + "\",\"message\": \"" + message + "\"}";
            }
            string inputjson = "{\"shareCreationInfo\": [" + emailidsjson + "]}";
            string UserAgreements = await adobeObject.ShareAgreement(inputjson, agreementId);


            // Outputs
            return (ctx) => {
                ShareCreationResponseList.Set(ctx, UserAgreements);
            };
        }

        #endregion
    }
}

