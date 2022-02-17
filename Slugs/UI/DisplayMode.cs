namespace Slugs.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Flags]
    public enum DisplayMode
    {
	    None = 0,
	    ShowTicks = 0x1,
	    ShowLengths = 0x2,
	    ShowSlugValues = 0x4,
	    ShowConstraints = 0x8,
	    //XX = 0x10,
	    //XX = 0x20,
	    //XX = 0x40,
	    //XX = 0x80,
        ShowAllValues = ShowLengths | ShowSlugValues,

        Any = 0xFFFF,
    }
    public static class DisplayModeExtensions
    {
	    public static bool IsNone(this DisplayMode kind) => kind == DisplayMode.None;
	    public static bool IsNumeric(this DisplayMode kind) => (kind & DisplayMode.ShowAllValues) > 0;
    }
}
