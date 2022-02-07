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

	public class RefPoint : PointBase
	{
		public override ElementKind ElementKind => ElementKind.RefPoint;
		public override IElement EmptyElement => Empty;
		public static readonly RefPoint Empty = new RefPoint();

		public RefPoint() : base(true)
		{
			Position = SKPoint.Empty;
		}

		public int TargetKey { get; private set; }
		public override IPoint TargetPoint => Pad.ResolvedPointFor(this);
		public ElementKind TargetKind => TargetPoint.ElementKind;

        public override List<IPoint> Points => IsEmpty ? new List<IPoint>() : new List<IPoint> { Pad.ResolvedPointFor(this)};
		public override SKPoint Position
		{
			get => Pad.ResolvedPointFor(this).Position;
			set
			{
                var tp = Pad.ResolvedPointFor(this);
                if (!tp.IsEmpty)
                {
	                tp.Position = value;
                }
			}
		}

		public RefPoint(PadKind padKind, int targetKey) : base(padKind)
		{
			TargetKey = targetKey;
		}
		public override bool CanMergeWith(IPoint point)
		{
			return TargetPoint.CanMergeWith(point.TargetPoint);
		}

        //   public bool ReplaceWith(IPoint pt)
        //{
        // Key = pt.Key;
        // SelectionKind = PointKind.Reference;
        // return true;
        //   }

        //public void CopyValuesFrom(IPoint from) // probably don't do things this way, replace IElement values instead
        //{
        // PadKind = from.PadKind;
        // if (from.SelectionKind == SelectionKind.Terminal)
        // {
        //  Position = from.Position;
        // }
        //       else
        // {
        //  TargetKey = from.Key;
        //  //_point = from.Position;
        // }
        //}
        public static bool operator ==(RefPoint left, IPoint right) => left.Key == right.Key && (right is RefPoint value && left.TargetKey == value.TargetKey);
        public static bool operator !=(RefPoint left, IPoint right) => left.Key != right.Key || (right is RefPoint value && left.TargetKey != value.TargetKey);
        public override bool Equals(object obj) => obj is RefPoint value && this == value;
        public override int GetHashCode() => PadKind.GetHashCode() * 31 + Key.GetHashCode() * 37 + TargetKey.GetHashCode() * 39;
    }
}
