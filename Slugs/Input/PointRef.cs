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

		public readonly int PadIndex;
		public readonly int InfoSetIndex;
		public readonly int PointIndex;
		// pointIndex, endIndex, t -- then everything becomes a place on a line. Perhaps also offset.
		// Look at 'start index' being referenced from the end (like complex segments)
		// The output can be two lines as well, esp symmetrical along axis - maybe this is the negative/complex version (see top and bottom of transparent sheet)

		public PointRef(int padIndex, int infoSetIndex, int pointIndex, int endIndex = -1, float t = 0, float offset = 0)
		{
			PadIndex = padIndex;
			InfoSetIndex = infoSetIndex;
			PointIndex = pointIndex;
		}

		public SKPoint Point
		{
			get => SlugAgent.ActiveAgent[this];
			set => SlugAgent.ActiveAgent[this] = value;
        } 

		public bool IsEmpty => PadIndex == -1 && InfoSetIndex == -1 && PointIndex == -1;

		public static bool operator ==(PointRef left, PointRef right) => left.PadIndex == right.PadIndex && left.InfoSetIndex == right.InfoSetIndex && left.PointIndex == right.PointIndex;
		public static bool operator !=(PointRef left, PointRef right) => left.PadIndex != right.PadIndex || left.InfoSetIndex != right.InfoSetIndex || left.PointIndex != right.PointIndex;
		public override bool Equals(object obj) => obj is PointRef value && this == value;
		public bool Equals(PointRef value) => this.PadIndex.Equals(value.PadIndex) && this.InfoSetIndex.Equals(value.InfoSetIndex) && this.PointIndex.Equals(value.PointIndex);
		public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 29 + InfoSetIndex.GetHashCode() * 37 + PointIndex.GetHashCode();
	}

	public readonly struct VirtualPointRef : IEquatable<VirtualPointRef>
	{
		public static PointRef Empty = new PointRef(-1, -1, -1);

		public readonly PointRef StartRef;
		public readonly PointRef EndRef;
		public readonly float T;
		public readonly float Offset;

		public SKPoint StartPoint => StartRef.Point; // todo: add t and offset to virtual point
		public SKPoint EndPoint => EndRef.Point;

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