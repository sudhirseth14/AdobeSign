using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using AdobeSign.Activities.Properties;
using AdobeSign.Contracts;
using AdobeSign.Services;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace AdobeSign.Activities.Tokens
{
    [LocalizedDisplayName(nameof(Resources.GetRefreshToken_DisplayName))]
    [LocalizedDescription(nameof(Resources.GetRefreshToken_Description))]
    public class GetRefreshToken : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetRefreshToken_APIAccessPoint_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetRefreshToken_APIAccessPoint_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> APIAccessPoint { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetRefreshToken_AuthorizationCode_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetRefreshToken_AuthorizationCode_Description))]
        [LocalizedCategory(nameof(Resources.Authentication_Category))]
        public InArgument<string> AuthorizationCode { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetRefreshToken_ClientId_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetRefreshToken_ClientId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ClientId { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetRefreshToken_ClientSecret_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetRefreshToken_ClientSecret_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ClientSecret { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetRefreshToken_RedirectUri_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetRefreshToken_RedirectUri_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> RedirectUri { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetRefreshToken_RefreshToken_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetRefreshToken_RefreshToken_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> RefreshToken { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public GetRefreshToken()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (APIAccessPoint == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(APIAccessPoint)));
            if (AuthorizationCode == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(AuthorizationCode)));
            if (ClientId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ClientId)));
            if (ClientSecret == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ClientSecret)));
            if (RedirectUri == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(RedirectUri)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var apiAccessPoint = APIAccessPoint.Get(context);
            var authorizationCode = AuthorizationCode.Get(context);
            var clientId = ClientId.Get(context);
            var clientSecret = ClientSecret.Get(context);
            var redirectUri = RedirectUri.Get(context);

            AdobeObject adobeObject = new AdobeObject(API);
            AccessToken tokenDetails = new AccessToken();
            tokenDetails = await adobeObject.GetAccessToken(apiAccessPoint, authorizationCode, clientId, clientSecret, redirectUri);
            tokenDetails.TokenExpireDate = DateTime.UtcNow.AddSeconds(tokenDetails.expires_in - 600);
            adobeObject.WriteTokenDetailstoTempFile(tokenDetails);
            // Outputs
            return (ctx) => {
                RefreshToken.Set(ctx, tokenDetails.refresh_token);
            };
        }

        #endregion
    }
}

