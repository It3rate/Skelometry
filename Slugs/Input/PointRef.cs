using SkiaSharp;
using Slugs.Agent;
using Slugs.Input;
using Slugs.Slugs;

namespace Slugs.Pads
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public readonly struct PointRef : IPointRef, IEquatable<PointRef>
	{
		public static PointRef Empty = new PointRef(-1, -1, -1);
		public bool IsEmpty => FocalIndex == -1;

		public int PadIndex { get; }
		public int EntityIndex { get; }
		public int FocalIndex { get; }

        // pointIndex, endIndex, t -- then everything becomes a place on a line. Perhaps also offset.
        // Look at 'start index' being referenced from the end (like complex segments)
        // The output can be two lines as well, esp symmetrical along axis - maybe this is the negative/complex version (see top and bottom of transparent sheet)

        // OR... this only maps points, everything on a line is a line (DataMap) and a slug. Points on a line are always segments, where you can ignore one side (0-7 vs 7).
        // probably datamaps should be just segments rather than polylines.
        // Addition is relating one line's slug with another line
        // Multiplication is locking the absolute size (eg after translating units like cm/inch) of one line/slug to another.
        // conflicts cause things to move up a dimension, as curves of various types, or even triangles (no curve). This is based on properties of the segments.

        public PointRef(int padIndex, int dataMapIndex, int pointIndex)
		{
			PadIndex = padIndex;
			EntityIndex = dataMapIndex;
			FocalIndex = pointIndex;
		}

        public SKPoint SKPoint
		{
			get => SlugAgent.ActiveAgent[this];
			set => SlugAgent.ActiveAgent[this] = value;
        }

		public void UpdateWith(IPointRef newPoint)
		{
			SlugAgent.ActiveAgent.UpdatePointRef(this, newPoint);
		}

        public static bool operator ==(PointRef left, PointRef right) => left.PadIndex == right.PadIndex && left.EntityIndex == right.EntityIndex && left.FocalIndex == right.FocalIndex;
		public static bool operator !=(PointRef left, PointRef right) => left.PadIndex != right.PadIndex || left.EntityIndex != right.EntityIndex || left.FocalIndex != right.FocalIndex;
		public override bool Equals(object obj) => obj is PointRef value && this == value;
		public bool Equals(PointRef value) => this == value;
		public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 29 + EntityIndex.GetHashCode() * 37 + FocalIndex.GetHashCode();
	}

	public struct VirtualPoint : IPointRef, IEquatable<VirtualPoint>
	{
		public bool IsEmpty => Seg.IsEmpty;

        private PointRef _pointRef;
        public readonly SegRef Seg;
		public float T;
		public readonly float Offset;

		public int PadIndex => _pointRef.PadIndex;
		public int EntityIndex => _pointRef.EntityIndex;
        public int FocalIndex => _pointRef.FocalIndex;

		public SKPoint StartPoint => Seg.StartPoint;
		public SKPoint EndPoint => Seg.EndPoint;

		public VirtualPoint(SegRef seg, float t, float offset = 0)
		{
			_pointRef = PointRef.Empty;
			Seg = seg;
			T = t;
			Offset = offset;
		}

		public SKPoint SKPoint
		{
			get => Seg.PointAlongLine(T);
			set => T = Seg.TFromPoint(value);
		}

		public static bool operator ==(VirtualPoint left, VirtualPoint right) =>
			left._pointRef == right._pointRef && left.Seg == right.Seg && left.T == right.T && left.Offset == right.Offset;

		public static bool operator !=(VirtualPoint left, VirtualPoint right) =>
			left._pointRef == right._pointRef && left.Seg != right.Seg || left.T != right.T || left.Offset != right.Offset;

		public override bool Equals(object obj) => obj is VirtualPoint value && this == value;

		public bool Equals(VirtualPoint value) =>
			_pointRef.Equals(value._pointRef) && Seg.Equals(value.Seg) && T.Equals(value.T) && Offset.Equals(value.Offset);

		public override int GetHashCode() =>
			Seg.GetHashCode() * 39 + Seg.GetHashCode()* 29 + T.GetHashCode() * 37 + Offset.GetHashCode();
	}
}