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
    [LocalizedDisplayName(nameof(Resources.SendAgreement_DisplayName))]
    [LocalizedDescription(nameof(Resources.SendAgreement_Description))]
    public class SendAgreement : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendAgreement_TransientDocumentId_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendAgreement_TransientDocumentId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> TransientDocumentId { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendAgreement_Name_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendAgreement_Name_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Name { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendAgreement_EmailId_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendAgreement_EmailId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<String[]> EmailId { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendAgreement_SignatureType_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendAgreement_SignatureType_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        [TypeConverter(typeof(EnumNameConverter<SignatureType>))]
        public SignatureType SignatureType { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendAgreement_Role_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendAgreement_Role_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        [TypeConverter(typeof(EnumNameConverter<Role>))]
        public Role Role { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendAgreement_Id_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendAgreement_Id_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Id { get; set; }

        [LocalizedDisplayName(nameof(Resources.SendAgreement_State_DisplayName))]
        [LocalizedDescription(nameof(Resources.SendAgreement_State_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        [TypeConverter(typeof(EnumNameConverter<State>))]
        public State State { get; set; }
        private static RestAPI API;
        #endregion


        #region Constructors

        public SendAgreement()
        {
            Constraints.Add(ActivityConstraints.HasParentType<SendAgreement, AdobeSignScope>(string.Format(Resources.ValidationScope_Error, Resources.AdobeSignScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (TransientDocumentId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(TransientDocumentId)));
            if (Name == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Name)));
            if (EmailId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(EmailId)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var transientDocumentId = TransientDocumentId.Get(context);
            var name = Name.Get(context);
            var emailIds = EmailId.Get(context);
            var signatureType = Enum.GetName(typeof(SignatureType), (int)SignatureType);
            var role = Enum.GetName(typeof(Role), (int)Role);
            var state = Enum.GetName(typeof(State), (int)State);
            int order = 1;
            var objectContainer = context.GetFromContext<IObjectContainer>(AdobeSignScope.ParentContainerPropertyTag);
            var applicationDetails = objectContainer.Get<ApplicationDetails>();
            var tokenDetails = objectContainer.Get<AccessToken>();
            API = new RestAPI(tokenDetails.token_type, applicationDetails.APIUrl, tokenDetails.access_token, applicationDetails.ClientID, applicationDetails.ClientSecret, tokenDetails.refresh_token, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            string emailidsjson = "";
            foreach (var emailId in emailIds)
            {
                if (emailidsjson != "")
                    emailidsjson = emailidsjson + "," + "{\"email\":\"" + emailId + "\"}";
                else
                    emailidsjson = "{\"email\":\"" + emailId + "\"}";
            }
            string inputjson = "{\"fileInfos\":[{\"transientDocumentId\":\"" + transientDocumentId + "\"}],\"name\":\"" + name + "\",\"participantSetsInfo\":[{\"memberInfos\":[" + emailidsjson + "],\"order\":\"" + order + "\",\"role\":\"" + role + "\"}],\"signatureType\":\"" + signatureType + "\",\"state\":\"" + state + "\"}";
            AgreementCreationResponse AgreementCreationResponse = await adobeObject.CreateAgreement(inputjson);

            // Outputs
            return (ctx) => {
                Id.Set(ctx, AgreementCreationResponse.id);
            };
        }

        #endregion
    }
}

