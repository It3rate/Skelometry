using SkiaSharp;
using Slugs.Agent;
using Slugs.Input;

namespace Slugs.Pads
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public interface IPointRef
	{
		int PadIndex { get; }
        int DataMapIndex { get; }
		int PointIndex { get; }
        SKPoint SKPoint { get; set; }
        bool IsEmpty { get; }
	}
	public readonly struct PointRef : IPointRef, IEquatable<PointRef>
	{
		public static PointRef Empty = new PointRef(-1, -1, -1);
		public bool IsEmpty => PointIndex == -1;

		public int PadIndex { get; }
		public int DataMapIndex { get; }
		public int PointIndex { get; }

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
			DataMapIndex = dataMapIndex;
			PointIndex = pointIndex;
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

        public static bool operator ==(PointRef left, PointRef right) => left.PadIndex == right.PadIndex && left.DataMapIndex == right.DataMapIndex && left.PointIndex == right.PointIndex;
		public static bool operator !=(PointRef left, PointRef right) => left.PadIndex != right.PadIndex || left.DataMapIndex != right.DataMapIndex || left.PointIndex != right.PointIndex;
		public override bool Equals(object obj) => obj is PointRef value && this == value;
		public bool Equals(PointRef value) => this == value;
		public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 29 + DataMapIndex.GetHashCode() * 37 + PointIndex.GetHashCode();
	}

	public struct VirtualPoint : IPointRef, IEquatable<VirtualPoint>
	{
		public bool IsEmpty => Segment.IsEmpty;

        private PointRef _pointRef;
        public readonly SegmentRef Segment;
		public float T;
		public readonly float Offset;

		public int PadIndex => _pointRef.PadIndex;
		public int DataMapIndex => _pointRef.DataMapIndex;
        public int PointIndex => _pointRef.PointIndex;

		public SKPoint StartPoint => Segment.StartPoint;
		public SKPoint EndPoint => Segment.EndPoint;

		public VirtualPoint(SegmentRef segment, float t, float offset = 0)
		{
			_pointRef = PointRef.Empty;
			Segment = segment;
			T = t;
			Offset = offset;
		}

		public SKPoint SKPoint
		{
			get => Segment.PointAlongLine(T);
			set => T = Segment.TFromPoint(value);
		}

		public static bool operator ==(VirtualPoint left, VirtualPoint right) =>
			left._pointRef == right._pointRef && left.Segment == right.Segment && left.T == right.T && left.Offset == right.Offset;

		public static bool operator !=(VirtualPoint left, VirtualPoint right) =>
			left._pointRef == right._pointRef && left.Segment != right.Segment || left.T != right.T || left.Offset != right.Offset;

		public override bool Equals(object obj) => obj is VirtualPoint value && this == value;

		public bool Equals(VirtualPoint value) =>
			_pointRef.Equals(value._pointRef) && Segment.Equals(value.Segment) && T.Equals(value.T) && Offset.Equals(value.Offset);

		public override int GetHashCode() =>
			Segment.GetHashCode() * 39 + Segment.GetHashCode()* 29 + T.GetHashCode() * 37 + Offset.GetHashCode();
	}
}