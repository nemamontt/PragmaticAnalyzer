using System.ComponentModel;

namespace PragmaticAnalyzer.Enums
{
    public enum ViolatorPotential
    {
        [Description("Нарушитель, обладающий базовыми возможностями")]
        Basic = 1,

        [Description("Нарушитель, обладающий базовыми повышенными возможностями")]
        BasicEnhanced = 2,

        [Description("Нарушитель, обладающий средними возможностями")]
        Average = 3,

        [Description("Нарушитель, обладающий высокими возможностями")]
        High = 4,
    }
}
