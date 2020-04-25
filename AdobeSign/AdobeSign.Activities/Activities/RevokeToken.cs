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

namespace AdobeSign.Activities.Tokens
{
    [LocalizedDisplayName(nameof(Resources.RevokeToken_DisplayName))]
    [LocalizedDescription(nameof(Resources.RevokeToken_Description))]
    public class RevokeToken : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.RevokeToken_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.RevokeToken_Status_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Status { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public RevokeToken()
        {
            Constraints.Add(ActivityConstraints.HasParentType<RevokeToken, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Object Container: Use objectContainer.Get<T>() to retrieve objects from the scope
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            string fileName = "adobesigntokendetails.txt";
            string myTempFile = Path.Combine(Path.GetTempPath(), fileName);
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            bool reovkedstatus = await adobeObject.RevokedAccessToken(tokenDetails.access_token);
            if (reovkedstatus)
            {
                if (File.Exists(myTempFile))
                {
                    File.Delete(myTempFile);
                }
            }
            // Outputs
            return (ctx) => {
                Status.Set(ctx, reovkedstatus);
            };
        }

        #endregion
    }
}

