using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Ectm.Enums
{
    public enum BaselineStatistic
    {
        [Display(Name = "Window Mean")]
        MEAN = 1,

        [Display(Name = "Value At Peak Sample")]
        AT_PEAK_SAMPLE = 2
    }
}
