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

namespace AdobeSign.Activities.LibraryDocuments
{
    [LocalizedDisplayName(nameof(Resources.UpdateLibraryDocumentState_DisplayName))]
    [LocalizedDescription(nameof(Resources.UpdateLibraryDocumentState_Description))]
    public class UpdateLibraryDocumentState : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateLibraryDocumentState_LibraryDocumentId_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateLibraryDocumentState_LibraryDocumentId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> LibraryDocumentId { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateLibraryDocumentState_State_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateLibraryDocumentState_State_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        [TypeConverter(typeof(EnumNameConverter<LibraryDocsState>))]
        public LibraryDocsState State { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateLibraryDocumentState_Response_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateLibraryDocumentState_Response_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Response { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public UpdateLibraryDocumentState()
        {
            Constraints.Add(ActivityConstraints.HasParentType<UpdateLibraryDocumentState, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (LibraryDocumentId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(LibraryDocumentId)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var libraryDocumentId = LibraryDocumentId.Get(context);
            var LibraruDocState = Enum.GetName(typeof(LibraryDocsState), (int)State);
            bool response = false;
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string inputjson = "{\"state\": \"" + LibraruDocState + "\"}";
            string StateReposnse = await adobeObject.UpdateLibrarDocState(inputjson, libraryDocumentId);
            response = true;
            // Outputs
            return (ctx) => {
                Response.Set(ctx, response);
            };
        }

        #endregion
    }
}

