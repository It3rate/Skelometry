using SkiaSharp;
using Slugs.Agent;

namespace Slugs.Pads
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public readonly struct PointRef : IEquatable<PointRef>
	{
		public static PointRef Empty = new PointRef(-1, -1, -1);
		public bool IsEmpty => PointIndex == -1;

        public readonly int PadIndex;
		public readonly int DataMapIndex;
		public readonly int PointIndex;
		public readonly float T; // maybe t isn't needed, and pointrefs can only represent measured things, not extrapolated.

        // pointIndex, endIndex, t -- then everything becomes a place on a line. Perhaps also offset.
        // Look at 'start index' being referenced from the end (like complex segments)
        // The output can be two lines as well, esp symmetrical along axis - maybe this is the negative/complex version (see top and bottom of transparent sheet)

        // OR... this only maps points, everything on a line is a line (DataMap) and a slug. Points on a line are always segments, where you can ignore one side (0-7 vs 7).
        // probably datamaps should be just segments rather than polylines.
        // Addition is relating one line's slug with another line
        // Multiplication is locking the absolute size (eg after translating units like cm/inch) of one line/slug to another.
        // conflicts cause things to move up a dimension, as curves of various types, or even triangles (no curve). This is based on properties of the segments.

        public PointRef(int padIndex, int dataMapIndex, int pointIndex, float t = 0)
		{
			PadIndex = padIndex;
			DataMapIndex = dataMapIndex;
			PointIndex = pointIndex;
			T = t;
		}

		public SKPoint SKPoint
		{
			get => SlugAgent.ActiveAgent[this];
			set => SlugAgent.ActiveAgent[this] = value;
        }

		public static bool operator ==(PointRef left, PointRef right) => left.PadIndex == right.PadIndex && left.DataMapIndex == right.DataMapIndex && left.PointIndex == right.PointIndex && left.T == right.T;
		public static bool operator !=(PointRef left, PointRef right) => left.PadIndex != right.PadIndex || left.DataMapIndex != right.DataMapIndex || left.PointIndex != right.PointIndex || left.T != right.T;
		public override bool Equals(object obj) => obj is PointRef value && this == value;
		public bool Equals(PointRef value) => this == value;
		public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 29 + DataMapIndex.GetHashCode() * 37 + PointIndex.GetHashCode() + T.GetHashCode() * 41;
	}

	public readonly struct VirtualPointRef : IEquatable<VirtualPointRef>
	{
		public static PointRef Empty = new PointRef(-1, -1, -1);

		public readonly PointRef StartRef;
		public readonly PointRef EndRef;
		public readonly float T;
		public readonly float Offset;

		public SKPoint StartPoint => StartRef.SKPoint;
		public SKPoint EndPoint => EndRef.SKPoint;

		public VirtualPointRef(PointRef startRef, PointRef endRef, float t, float offset = 0)
		{
			StartRef = startRef;
			EndRef = endRef;
			T = t;
			Offset = offset;
		}

		public bool IsEmpty => StartRef.IsEmpty;

		public static bool operator ==(VirtualPointRef left, VirtualPointRef right) =>
			left.StartRef == right.StartRef && left.EndRef == right.EndRef && left.T == right.T && left.Offset == right.Offset;

		public static bool operator !=(VirtualPointRef left, VirtualPointRef right) =>
			left.StartRef != right.StartRef || left.EndRef != right.EndRef || left.T != right.T || left.Offset != right.Offset;

		public override bool Equals(object obj) => obj is VirtualPointRef value && this == value;

		public bool Equals(VirtualPointRef value) =>
			StartRef.Equals(value.StartRef) && EndRef.Equals(value.EndRef) && T.Equals(value.T) && Offset.Equals(value.Offset);

		public override int GetHashCode() =>
			17 * 23 + StartRef.GetHashCode() * 29 + EndRef.GetHashCode() * 31 + T.GetHashCode() * 37 + Offset.GetHashCode();
	}
}