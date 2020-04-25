using System;
using System.Activities;
using System.IO;
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
    [LocalizedDisplayName(nameof(Resources.DownloadAgreement_DisplayName))]
    [LocalizedDescription(nameof(Resources.DownloadAgreement_Description))]
    public class DownloadAgreement : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.DownloadAgreement_AgreementId_DisplayName))]
        [LocalizedDescription(nameof(Resources.DownloadAgreement_AgreementId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> AgreementId { get; set; }

        [LocalizedDisplayName(nameof(Resources.DownloadAgreement_FileName_DisplayName))]
        [LocalizedDescription(nameof(Resources.DownloadAgreement_FileName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FileName { get; set; }

        [LocalizedDisplayName(nameof(Resources.DownloadAgreement_FilePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.DownloadAgreement_FilePath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FilePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.DownloadAgreement_Response_DisplayName))]
        [LocalizedDescription(nameof(Resources.DownloadAgreement_Response_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Response { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public DownloadAgreement()
        {
            Constraints.Add(ActivityConstraints.HasParentType<DownloadAgreement, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (AgreementId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(AgreementId)));
            if (FileName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FileName)));
            if (FilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FilePath)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var agreementId = AgreementId.Get(context);
            var fileName = FileName.Get(context);
            var filePath = FilePath.Get(context);
            bool response = false;
            string filesavepath = Path.Combine(filePath, fileName);
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            byte[] doc = await adobeObject.GetAgreementCombinedDocument(agreementId);
            try
            {
                File.WriteAllBytes(filesavepath, doc);
                response = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to save file at given path.");
            }
            // Outputs
            return (ctx) => {
                Response.Set(ctx, response);
            };
        }

        #endregion
    }
}

