using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum ChannelEncoding
    {
        [Display(Name = "Float64 GZip")]
        FLOAT64_GZIP = 1,

        [Display(Name = "Text JSON GZip")]
        TEXT_JSON_GZIP = 2
    }
}
