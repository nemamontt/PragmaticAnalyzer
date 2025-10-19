using System.ComponentModel;

namespace PragmaticAnalyzer.Enums
{
    public enum ViolatorSource
    {
        [Description("Внутренний нарушитель")]
        Internal = 1,

        [Description("Внешний нарушитель")]
        External = 2
    }
}
