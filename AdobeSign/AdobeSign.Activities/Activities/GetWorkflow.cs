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

namespace AdobeSign.Activities.Workflows
{
    [LocalizedDisplayName(nameof(Resources.GetWorkflow_DisplayName))]
    [LocalizedDescription(nameof(Resources.GetWorkflow_Description))]
    public class GetWorkflow : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetWorkflow_WorkflowId_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetWorkflow_WorkflowId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> WorkflowId { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetWorkflow_WorkflowDescription_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetWorkflow_WorkflowDescription_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> WorkflowDescription { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public GetWorkflow()
        {
            Constraints.Add(ActivityConstraints.HasParentType<GetWorkflow, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (WorkflowId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(WorkflowId)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var workflowId = WorkflowId.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string WorkFlow = await adobeObject.GetWorkflow(workflowId);

            // Outputs
            return (ctx) => {
                WorkflowDescription.Set(ctx, WorkFlow);
            };
        }

        #endregion
    }
}

