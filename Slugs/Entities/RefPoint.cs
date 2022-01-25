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
		public static RefPoint Empty = new RefPoint();
		private RefPoint() : base(true) { SKPoint = SKPoint.Empty; }

		public int TargetKey { get; private set; }

		public override IPoint[] Points => IsEmpty ? new IPoint[] { } : new IPoint[] {Pad.TerminalPointAt(TargetKey)};
		public override SKPoint SKPoint
		{
			get => Pad.TerminalPointAt(Key).SKPoint;
			set => Pad.TerminalPointAt(Key).SKPoint = value;
		}

		public RefPoint(PadKind padKind, int targetKey) : base(padKind)
		{
			TargetKey = targetKey;
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
		//  SKPoint = from.SKPoint;
		// }
		//       else
		// {
		//  TargetKey = from.Key;
		//  //_point = from.SKPoint;
		// }
		//}
		//public static bool operator ==(RefPoint left, IPoint right) => left.Key == right.Key;
		//public static bool operator !=(RefPoint left, IPoint right) => left.Key != right.Key;
		//public override bool Equals(object obj) => obj is RefPoint value && this == value;
		//public bool Equals(IPoint value) => this == value;
		//public override int GetHashCode() => Key.GetHashCode();
	}
}
