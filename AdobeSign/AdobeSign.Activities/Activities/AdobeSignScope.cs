using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Activities.Statements;
using System.ComponentModel;
using AdobeSign.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using AdobeSign.Services;
using AdobeSign.Contracts;
using AdobeSign.Models;
using System.IO;

namespace AdobeSign.Activities.Scope
{
    [LocalizedDisplayName(nameof(Resources.AdobeSignScope_DisplayName))]
    [LocalizedDescription(nameof(Resources.AdobeSignScope_Description))]
    public class AdobeSignScope : ContinuableAsyncNativeActivity
    {
        #region Properties

        [Browsable(false)]
        public ActivityAction<IObjectContainer​> Body { get; set; }

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.AdobeSignScope_APIAccessPoint_DisplayName))]
        [LocalizedDescription(nameof(Resources.AdobeSignScope_APIAccessPoint_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> APIAccessPoint { get; set; }

        [LocalizedDisplayName(nameof(Resources.AdobeSignScope_RefreshToken_DisplayName))]
        [LocalizedDescription(nameof(Resources.AdobeSignScope_RefreshToken_Description))]
        [LocalizedCategory(nameof(Resources.Authentication_Category))]
        public InArgument<string> RefreshToken { get; set; }

        [LocalizedDisplayName(nameof(Resources.AdobeSignScope_ClientId_DisplayName))]
        [LocalizedDescription(nameof(Resources.AdobeSignScope_ClientId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ClientId { get; set; }

        [LocalizedDisplayName(nameof(Resources.AdobeSignScope_ClientSecret_DisplayName))]
        [LocalizedDescription(nameof(Resources.AdobeSignScope_ClientSecret_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ClientSecret { get; set; }

        [LocalizedDisplayName(nameof(Resources.AdobeSignScope_RedirectUri_DisplayName))]
        [LocalizedDescription(nameof(Resources.AdobeSignScope_RedirectUri_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> RedirectUri { get; set; }

        // A tag used to identify the scope in the activity context
        internal static string ParentContainerPropertyTag => "ScopeActivity";

        // Object Container: Add strongly-typed objects here and they will be available in the scope's child activities.
        private readonly IObjectContainer _objectContainer;
        private static RestAPI API;
        #endregion


        #region Constructors

        public AdobeSignScope(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;

            Body = new ActivityAction<IObjectContainer>
            {
                Argument = new DelegateInArgument<IObjectContainer> (ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = Resources.Do }
            };
        }

        public AdobeSignScope() : this(new ObjectContainer())
        {

        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            if (APIAccessPoint == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(APIAccessPoint)));
            if (RefreshToken == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(RefreshToken)));
            if (ClientId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ClientId)));
            if (ClientSecret == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ClientSecret)));
            if (RedirectUri == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(RedirectUri)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<NativeActivityContext>> ExecuteAsync(NativeActivityContext  context, CancellationToken cancellationToken)
        {
            // Inputs
            var apiAccessPoint = APIAccessPoint.Get(context);
            var refreshToken = RefreshToken.Get(context);
            var clientId = ClientId.Get(context);
            var clientSecret = ClientSecret.Get(context);
            var redirectUri = RedirectUri.Get(context);
            string fileName = "adobesigntokendetails.txt";
            string myTempFile = Path.Combine(Path.GetTempPath(), fileName);
            AccessToken tokenDetails = new AccessToken();
            ApplicationDetails applicationDetails = new ApplicationDetails(apiAccessPoint, clientId, clientSecret, redirectUri);
            AdobeObject adobeObject = new AdobeObject(API);
            if (!File.Exists(myTempFile))
            {
                tokenDetails = await adobeObject.GetAccessTokenByRefreshToken(apiAccessPoint, refreshToken, clientId, clientSecret, null);
                tokenDetails.TokenExpireDate = DateTime.UtcNow.AddSeconds(tokenDetails.expires_in - 600);
                tokenDetails.refresh_token = refreshToken;
                adobeObject.WriteTokenDetailstoTempFile(tokenDetails);
                _objectContainer.Add(tokenDetails);
            }
            else
            {
                string jsonDetails = File.ReadAllText(myTempFile);
                tokenDetails = await adobeObject.ConvertJsonToAccessToken(jsonDetails);
                if (tokenDetails.TokenExpireDate.HasValue && tokenDetails.TokenExpireDate < DateTime.UtcNow)
                {
                    if (refreshToken == null)
                        throw new Exception("Refresh token is null.");
                    tokenDetails = await adobeObject.GetAccessTokenByRefreshToken(apiAccessPoint, refreshToken, clientId, clientSecret, null);
                    tokenDetails.TokenExpireDate = DateTime.UtcNow.AddSeconds(tokenDetails.expires_in - 600);
                    tokenDetails.refresh_token = refreshToken;
                    adobeObject.WriteTokenDetailstoTempFile(tokenDetails);
                }
                _objectContainer.Add(tokenDetails);
            }

            //send details to child activities
            _objectContainer.Add(applicationDetails);
            return (ctx) => {
                // Schedule child activities
                if (Body != null)
				    ctx.ScheduleAction<IObjectContainer>(Body, _objectContainer, OnCompleted, OnFaulted);

                // Outputs
            };
        }

        #endregion


        #region Events

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            faultContext.CancelChildren();
            Cleanup();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Cleanup();
        }

        #endregion


        #region Helpers
        
        private void Cleanup()
        {
            var disposableObjects = _objectContainer.Where(o => o is IDisposable);
            foreach (var obj in disposableObjects)
            {
                if (obj is IDisposable dispObject)
                    dispObject.Dispose();
            }
            _objectContainer.Clear();
        }

        #endregion
    }
}

