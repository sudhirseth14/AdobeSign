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
    [LocalizedDisplayName(nameof(Resources.UpdateUserDetails_DisplayName))]
    [LocalizedDescription(nameof(Resources.UpdateUserDetails_Description))]
    public class UpdateUserDetails : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserDetails_UserId_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserDetails_UserId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> UserId { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserDetails_Reponse_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserDetails_Reponse_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Response { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserDetails_IsAccountAdmin_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserDetails_IsAccountAdmin_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> IsAccountAdmin { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserDetails_Phone_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserDetails_Phone_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Phone { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserDetails_Title_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserDetails_Title_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Title { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public UpdateUserDetails()
        {
            Constraints.Add(ActivityConstraints.HasParentType<UpdateUserDetails, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (UserId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(UserId)));
            if (IsAccountAdmin == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(IsAccountAdmin)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var userId = UserId.Get(context);
            var isAccountAdmin = IsAccountAdmin.Get(context);
            var phone = Phone.Get(context);
            var title = Title.Get(context);
            bool response = false;
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            UserInfo userInfo = await adobeObject.GetUserInfo(userId);
                if (!String.IsNullOrEmpty(phone))
                    userInfo.phone = phone;
                else
                    phone = userInfo.phone;

                if (!String.IsNullOrEmpty(title))
                    userInfo.title = title;
                else
                    title = userInfo.title;
            userInfo.isAccountAdmin = isAccountAdmin;
            string updateresponse = await adobeObject.UpdateUserDetails(userInfo, userId);
            response = true;

            // Outputs
            return (ctx) => {
                Response.Set(ctx, response);
            };
        }

        #endregion
    }
}

