using System.Windows.Forms;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Commands;
using Slugs.Entities;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum EditKind
    {
	    Create,
	    Delete,
	    Merge,
	    Move,
	    Extend,
	    Duplicate,
	    Connect,
	    TempGroup,
	    Group,
	    Ungroup,
    }

    public enum SelectionKind
    {
        None,
	    BeginPoint,
	    BeginElement,
	    CurrentPoint,
	    CurrentElement,
	    SelectedPoint,
	    SelectedElement,
	    HighlightPoint,
	    HighlightElement,
	    ClipboardPoint,
	    ClipboardElement,

	    LastCommand,
	    ToolKind,
    }
    public enum UIMode
    {
	    Inspect,
	    Edit,
	    Interact,
	    Animate,
    }
}
