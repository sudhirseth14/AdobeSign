using System.ComponentModel;

namespace AdobeSign.Enums
{
    class Enums
    {
    }

    public enum ReminderFrequency
    {

        [Description("DAILY UNTIL SIGNED")]
        DAILY_UNTIL_SIGNED,
        [Description("WEEKLY UNTIL SIGNED")]
        WEEKLY_UNTIL_SIGNED,
        [Description("ONCE")]
        ONCE,
        [Description("NEVER")]
        NEVER,
    }

    public enum LibraryDocsState
    {
        [Description("ACTIVE")]
        ACTIVE,
        [Description("REMOVED")]
        REMOVED,
        [Description("AUTHORING")]
        AUTHORING
    }

    public enum AgreementStates
    {
        [Description("IN_PROCESS")]
        IN_PROCESS,
        [Description("CANCELLED")]
        CANCELLED,
        [Description("AUTHORING")]
        AUTHORING
    }
    public enum VisibilityStatus
    {

        [Description("SHOW")]
        SHOW,
        [Description("HIDE")]
        HIDE
    }
    public enum UserState
    {

        [Description("ACTIVE")]
        ACTIVE,
        [Description("INACTIVE")]
        INACTIVE
    }

    public enum SignatureType
    {
        [Description("ESIGN")]
        ESIGN,
        [Description("WRITTEN")]
        WRITTEN
    }

    public enum Role
    {
        [Description("SIGNER")]
        SIGNER,
        [Description("APPROVER")]
        APPROVER,
        [Description("ACCEPTOR")]
        ACCEPTOR,
        [Description("CERTIFIED RECIPIENT")]
        CERTIFIED_RECIPIENT,
        [Description("FORM FILLER")]
        FORM_FILLER,
        [Description("DELEGATE TO SIGNER")]
        DELEGATE_TO_SIGNER,
        [Description("DELEGATE TO APPROVER")]
        DELEGATE_TO_APPROVER,
        [Description("DELEGATE TO ACCEPTOR")]
        DELEGATE_TO_ACCEPTOR,
        [Description("DELEGATE TO CERTIFIED RECIPIENT")]
        DELEGATE_TO_CERTIFIED_RECIPIENT,
        [Description("DELEGATE TO FORM FILLER")]
        DELEGATE_TO_FORM_FILLER,
        [Description("SHARE")]
        SHARE
    }

    public enum State
    {
        [Description("IN PROCESS")]
        IN_PROCESS,
        [Description("AUTHORING")]
        AUTHORING,
        [Description("DRAFT")]
        DRAFT
    }
}
