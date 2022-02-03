﻿using SkiaSharp;
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
        private TerminalPoint() : base(true) { Position = SKPoint.Empty; }

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { this };
        public override SKPoint Position { get; set; }

        public TerminalPoint(PadKind padKind, SKPoint point) : base(padKind)
        {
            Position = point;
        }

        public bool ReplaceWith(IPoint pt)
        {
            Pad.SetPointAt(Key, pt);
            return true;
        }

        public void CopyValuesFrom(IPoint from)
        {
	        Position = from.Position;
        }
        public static bool operator ==(TerminalPoint left, TerminalPoint right) => left.Key == right.Key && left.Position == right.Position;
        public static bool operator !=(TerminalPoint left, TerminalPoint right) => left.Key != right.Key || left.Position != right.Position;
        public override bool Equals(object obj) => obj is TerminalPoint value && this == value;
        public override int GetHashCode() => Key * 17 + Position.GetHashCode();
    }
}