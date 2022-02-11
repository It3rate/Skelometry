namespace Slugs.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Flags]
    public enum UIMode
    {
	    None = 0,
	    CreateEntity = 0x1,
	    CreateTrait = 0x2,
	    CreateFocal = 0x4,
	    CreateDoubleBond = 0x8,
	    CreateBond = 0x10,

        //Inspect = 0x10,
        //Edit = 0x20,
        //Interact = 0x40,
        //Animate = 0x80,
        //XXX = 0x100,
        //XXX = 0x200,

        Any = 0xFFFF,

	    Create = CreateEntity | CreateTrait | CreateFocal | CreateBond,
    }
    public static class UIModeExtensions
    {
	    public static bool IsNone(this UIMode kind) => kind == UIMode.None;
	    public static bool IsCreate(this UIMode kind) => (int)(kind & UIMode.Create) > 0;// UIMode.Create.HasFlag(kind);
	    //public static bool IsTerminal(this UIMode kind) => kind == ElementKind.Terminal;
	    //public static bool IsCompatible(this UIMode self, ElementKind other) => (int)(self & other) != 0;
    }
}
