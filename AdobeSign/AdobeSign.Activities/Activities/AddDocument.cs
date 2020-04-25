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

namespace AdobeSign.Activities.TransientDocuments
{
    [LocalizedDisplayName(nameof(Resources.AddDocument_DisplayName))]
    [LocalizedDescription(nameof(Resources.AddDocument_Description))]
    public class AddDocument : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.AddDocument_FilePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.AddDocument_FilePath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FilePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.AddDocument_TransientDocumentId_DisplayName))]
        [LocalizedDescription(nameof(Resources.AddDocument_TransientDocumentId_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> TransientDocumentId { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public AddDocument()
        {
            Constraints.Add(ActivityConstraints.HasParentType<AddDocument, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (FilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FilePath)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var filePath = FilePath.Get(context);
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            TransientDocumentResponse TransientDocumentResponse = await adobeObject.AddDocument(Path.GetFileName(filePath), System.IO.File.ReadAllBytes(filePath));
            // Outputs
            return (ctx) => {
                TransientDocumentId.Set(ctx, TransientDocumentResponse.transientDocumentId);
            };
        }

        #endregion
    }
}

