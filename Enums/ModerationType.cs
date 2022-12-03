using System.ComponentModel;

namespace Blog_MVC.Enums
{
    public enum ModerationType
    {
        [Description("Political Harassment")]
        Political,
        [Description("Religous Persicution")]
        Religion,
        [Description("Foul Language")]
        Language,
        [Description("Drug use or distribution")]
        Drugs,
        [Description("Threatening another")]
        Threatening,
        [Description("Sexual discussions or comments")]
        Sexual,
        [Description("Personal comments")]
        Shaming
    }
}
