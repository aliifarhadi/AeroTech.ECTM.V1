using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum SnapshotContextStatus
    {
        [Display(Name = "Context Verified")]
        VERIFIED = 1,

        [Display(Name = "Context Unverified - Channel Coherence")]
        CONTEXT_UNVERIFIED_CHANNEL_COHERENCE = 2
    }
}
