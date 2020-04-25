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

namespace AdobeSign.Activities.Users
{
    [LocalizedDisplayName(nameof(Resources.UpdateUserState_DisplayName))]
    [LocalizedDescription(nameof(Resources.UpdateUserState_Description))]
    public class UpdateUserState : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserState_UserId_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserState_UserId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> UserId { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserState_Response_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserState_Response_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Response { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserState_Comments_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserState_Comments_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Comments { get; set; }

        [LocalizedDisplayName(nameof(Resources.UpdateUserState_State_DisplayName))]
        [LocalizedDescription(nameof(Resources.UpdateUserState_State_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        [TypeConverter(typeof(EnumNameConverter<UserState>))]
        public UserState State { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public UpdateUserState()
        {
            Constraints.Add(ActivityConstraints.HasParentType<UpdateUserState, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (UserId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(UserId)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var userId = UserId.Get(context);
            var comments = Comments.Get(context);
            var userState = Enum.GetName(typeof(UserState), (int)State);
            bool response = false;
            var inputjson = "{\"state\": \"" + userState + "\",\"comment\": \"" + comments + "\"}";
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string updateuserstate = await adobeObject.UpdateUserState(inputjson, userId);
            response = true;
            // Outputs
            return (ctx) => {
                Response.Set(ctx, response);
            };
        }

        #endregion
    }
}

