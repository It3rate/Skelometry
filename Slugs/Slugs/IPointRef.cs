using SkiaSharp;

namespace Slugs.Slugs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPointRef
    {
	    int PadIndex { get; }
	    int EntityKey { get; }
	    int FocalKey { get; }
	    SKPoint SKPoint { get; set; }
	    bool IsEmpty { get; }

	    bool ReplaceWith(IPointRef ptRef);
    }
}
