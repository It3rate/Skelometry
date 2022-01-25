using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TerminalPoint : PointBase
    {
        public override ElementKind ElementKind => ElementKind.Terminal;
        public override IElement EmptyElement => Empty;
        public static TerminalPoint Empty { get; } = new TerminalPoint();
        private TerminalPoint() : base(true) { SKPoint = SKPoint.Empty; }

        public override IPoint[] Points => IsEmpty ? new IPoint[] { } : new IPoint[] { this };
        public override SKPoint SKPoint { get; set; }

        public TerminalPoint(PadKind padKind, SKPoint point) : base(padKind)
        {
            SKPoint = point;
        }

        public bool ReplaceWith(IPoint pt)
        {
            Pad.SetPointAt(Key, pt);
            return true;
        }

        public void CopyValuesFrom(IPoint from)
        {
	        SKPoint = from.SKPoint;
        }
        //public static bool operator ==(TerminalPoint left, IPoint right) => left.Key == right.Key;
        //public static bool operator !=(TerminalPoint left, IPoint right) => left.Key != right.Key;
        //public override bool Equals(object obj) => obj is TerminalPoint value && this == value;
        //public bool Equals(IPoint value) => this == value;
        //public override int GetHashCode() => Key.GetHashCode();
    }
}
