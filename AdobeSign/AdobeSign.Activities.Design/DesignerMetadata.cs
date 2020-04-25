using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using AdobeSign.Activities.Account;
using AdobeSign.Activities.Agreements;
using AdobeSign.Activities.Design.Designers;
using AdobeSign.Activities.Design.Properties;
using AdobeSign.Activities.Groups;
using AdobeSign.Activities.LibraryDocuments;
using AdobeSign.Activities.MessageTemplates;
using AdobeSign.Activities.Scope;
using AdobeSign.Activities.Tokens;
using AdobeSign.Activities.TransientDocuments;
using AdobeSign.Activities.Users;
using AdobeSign.Activities.Workflows;

namespace AdobeSign.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(GetRefreshToken), new DesignerAttribute(typeof(GetRefreshTokenDesigner)));
            builder.AddCustomAttributes(typeof(GetRefreshToken), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(RevokeToken), new DesignerAttribute(typeof(RevokeTokenDesigner)));
            builder.AddCustomAttributes(typeof(RevokeToken), new HelpKeywordAttribute(""));

            
            builder.AddCustomAttributes(typeof(AdobeSignScope), new DesignerAttribute(typeof(AdobeSignScopeDesigner)));
            builder.AddCustomAttributes(typeof(AdobeSignScope), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(AddDocument), new DesignerAttribute(typeof(AddDocumentDesigner)));
            builder.AddCustomAttributes(typeof(AddDocument), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SendAgreement), new DesignerAttribute(typeof(SendAgreementDesigner)));
            builder.AddCustomAttributes(typeof(SendAgreement), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetAgreements), new DesignerAttribute(typeof(GetAgreementsDesigner)));
            builder.AddCustomAttributes(typeof(GetAgreements), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(ShareAgreement), new DesignerAttribute(typeof(ShareAgreementDesigner)));
            builder.AddCustomAttributes(typeof(ShareAgreement), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SendReminder), new DesignerAttribute(typeof(SendReminderDesigner)));
            builder.AddCustomAttributes(typeof(SendReminder), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetSigningURL), new DesignerAttribute(typeof(GetSigningURLDesigner)));
            builder.AddCustomAttributes(typeof(GetSigningURL), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DownloadAgreement), new DesignerAttribute(typeof(DownloadAgreementDesigner)));
            builder.AddCustomAttributes(typeof(DownloadAgreement), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(UpdateAgreementVisibility), new DesignerAttribute(typeof(UpdateAgreementVisibilityDesigner)));
            builder.AddCustomAttributes(typeof(UpdateAgreementVisibility), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(UpdateAgreementState), new DesignerAttribute(typeof(UpdateAgreementStateDesigner)));
            builder.AddCustomAttributes(typeof(UpdateAgreementState), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DeleteAgreement), new DesignerAttribute(typeof(DeleteAgreementDesigner)));
            builder.AddCustomAttributes(typeof(DeleteAgreement), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetAllUsers), new DesignerAttribute(typeof(GetAllUsersDesigner)));
            builder.AddCustomAttributes(typeof(GetAllUsers), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetUser), new DesignerAttribute(typeof(GetUserDesigner)));
            builder.AddCustomAttributes(typeof(GetUser), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetGroupsOfUser), new DesignerAttribute(typeof(GetGroupsOfUserDesigner)));
            builder.AddCustomAttributes(typeof(GetGroupsOfUser), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(UpdateUserDetails), new DesignerAttribute(typeof(UpdateUserDetailsDesigner)));
            builder.AddCustomAttributes(typeof(UpdateUserDetails), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(UpdateUserState), new DesignerAttribute(typeof(UpdateUserStateDesigner)));
            builder.AddCustomAttributes(typeof(UpdateUserState), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetAllGroups), new DesignerAttribute(typeof(GetAllGroupsDesigner)));
            builder.AddCustomAttributes(typeof(GetAllGroups), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetUsersOfGroup), new DesignerAttribute(typeof(GetUsersOfGroupDesigner)));
            builder.AddCustomAttributes(typeof(GetUsersOfGroup), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetMessageTemplates), new DesignerAttribute(typeof(GetMessageTemplatesDesigner)));
            builder.AddCustomAttributes(typeof(GetMessageTemplates), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetAccountInformation), new DesignerAttribute(typeof(GetAccountInformationDesigner)));
            builder.AddCustomAttributes(typeof(GetAccountInformation), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetAllWorkflows), new DesignerAttribute(typeof(GetAllWorkflowsDesigner)));
            builder.AddCustomAttributes(typeof(GetAllWorkflows), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetWorkflow), new DesignerAttribute(typeof(GetWorkflowDesigner)));
            builder.AddCustomAttributes(typeof(GetWorkflow), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetAllLibraryDocuments), new DesignerAttribute(typeof(GetAllLibraryDocumentsDesigner)));
            builder.AddCustomAttributes(typeof(GetAllLibraryDocuments), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetLibraryDocument), new DesignerAttribute(typeof(GetLibraryDocumentDesigner)));
            builder.AddCustomAttributes(typeof(GetLibraryDocument), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(UpdateLibraryDocumentVisibility), new DesignerAttribute(typeof(UpdateLibraryDocumentVisibilityDesigner)));
            builder.AddCustomAttributes(typeof(UpdateLibraryDocumentVisibility), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(UpdateLibraryDocumentState), new DesignerAttribute(typeof(UpdateLibraryDocumentStateDesigner)));
            builder.AddCustomAttributes(typeof(UpdateLibraryDocumentState), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetAgreement), new DesignerAttribute(typeof(GetAgreementDesigner)));
            builder.AddCustomAttributes(typeof(GetAgreement), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GetGroup), new DesignerAttribute(typeof(GetGroupDesigner)));
            builder.AddCustomAttributes(typeof(GetGroup), new HelpKeywordAttribute(""));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
