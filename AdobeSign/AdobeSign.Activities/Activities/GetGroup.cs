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

namespace AdobeSign.Activities.Groups
{
    [LocalizedDisplayName(nameof(Resources.GetGroup_DisplayName))]
    [LocalizedDescription(nameof(Resources.GetGroup_Description))]
    public class GetGroup : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetGroup_GroupId_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetGroup_GroupId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> GroupId { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetGroup_DetailedGroupInformation_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetGroup_DetailedGroupInformation_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> DetailedGroupInformation { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public GetGroup()
        {
            Constraints.Add(ActivityConstraints.HasParentType<GetGroup, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (GroupId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(GroupId)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var groupId = GroupId.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string GroupInfo = await adobeObject.GetGroupInfo(groupId);

            // Outputs
            return (ctx) => {
                DetailedGroupInformation.Set(ctx, GroupInfo);
            };
        }

        #endregion
    }
}

