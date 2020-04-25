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

namespace AdobeSign.Activities.Agreements
{
    [LocalizedDisplayName(nameof(Resources.SendReminder_DisplayName))]
    [LocalizedDescription(nameof(Resources.SendReminder_Description))]
    public class SendReminder : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendReminder_AgreementId_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendReminder_AgreementId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> AgreementId { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendReminder_ParticipantIds_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendReminder_ParticipantIds_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<String[]> ParticipantIds { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendReminder_Frequency_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendReminder_Frequency_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        [TypeConverter(typeof(EnumNameConverter<ReminderFrequency>))]
        public ReminderFrequency Frequency { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendReminder_ReminderCreationResult_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendReminder_ReminderCreationResult_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> ReminderCreationResult { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public SendReminder()
        {
            Constraints.Add(ActivityConstraints.HasParentType<SendReminder, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (AgreementId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(AgreementId)));
            if (ParticipantIds == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ParticipantIds)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var agreementId = AgreementId.Get(context);
            var participantIds = ParticipantIds.Get(context);
            var reminderfrequency = Enum.GetName(typeof(ReminderFrequency), (int)Frequency);
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string participantids = "";
            foreach (var participantId in participantIds)
            {
                if (participantids != "")
                    participantids = participantids + "," + "\"" + participantId + "\"";
                else
                    participantids = "\"" + participantId + "\"";
            }
            string inputjson = "{\"recipientParticipantIds\": [" + participantids + "],\"status\":\"ACTIVE\",\"frequency\":\"" + reminderfrequency + "\"}";
            string sendreminderreponse = await adobeObject.SendReminder(inputjson, agreementId);

            // Outputs
            return (ctx) => {
                ReminderCreationResult.Set(ctx, sendreminderreponse);
            };
        }

        #endregion
    }
}

