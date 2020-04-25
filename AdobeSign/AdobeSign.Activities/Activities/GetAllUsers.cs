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

namespace AdobeSign.Activities.Users
{
    [LocalizedDisplayName(nameof(Resources.GetAllUsers_DisplayName))]
    [LocalizedDescription(nameof(Resources.GetAllUsers_Description))]
    public class GetAllUsers : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetAllUsers_UsersInformation_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetAllUsers_UsersInformation_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> UsersInformation { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public GetAllUsers()
        {
            Constraints.Add(ActivityConstraints.HasParentType<GetAllUsers, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string Responsejson = await adobeObject.GetAllUsers();

            // Outputs
            return (ctx) => {
                UsersInformation.Set(ctx, Responsejson);
            };
        }

        #endregion
    }
}

