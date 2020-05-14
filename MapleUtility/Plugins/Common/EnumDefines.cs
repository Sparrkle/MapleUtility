using System.ComponentModel;

namespace MapleUtility.Plugins.Common
{
    public enum ProgressType
    {
        [Description("Start Progress")]
        START,
        [Description("Update Progress")]
        UPDATE,
        [Description("End Progress")]
        END
    }
}
