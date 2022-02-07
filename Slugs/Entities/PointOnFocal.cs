using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PointOnFocal : PointBase
    {
        public override ElementKind ElementKind => ElementKind.PointOnFocal;
        public override IElement EmptyElement => Empty;
        public static readonly PointOnFocal Empty = new PointOnFocal();
        private PointOnFocal() : base(true) { }

        public int FocalKey { get; set; }
        public float T { get; set; }

        public Focal Focal => Pad.FocalAt(FocalKey);
        public Trait Trait => Focal.Trait;
        public int TraitKey => Focal.TraitKey;

        public PointOnFocal(PadKind padKind, int focalKey, float t) : base(padKind)
        {
            FocalKey = focalKey;
            T = t;
        }

        public override bool CanMergeWith(IPoint point)
        {
	        return point.TargetPoint.ElementKind == ElementKind.PointOnFocal;
        }

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { this };
        public override SKPoint Position
        {
            get
            {
                var focal = Pad.FocalAt(FocalKey);
                return focal.IsEmpty ? SKPoint.Empty : focal.PointAlongLine(T);
            }
            set
            {
                var focal = Pad.FocalAt(FocalKey);
                T = focal.TFromPoint(value, false).Item1;
            }
        }

        public Trait GetTrait() => Pad.TraitAt(GetFocal().TraitKey);
        public Focal GetFocal() => Pad.FocalAt(FocalKey);
        public float GetT() => T;
        public IPoint GetStartPoint() => Pad.FocalAt(FocalKey).StartPoint;
        public IPoint GetEndPoint() => Pad.FocalAt(FocalKey).EndPoint;

        public bool ReplaceWith(IPoint to)
        {
            var result = false;
            if (Key != -1)
            {
                Pad.SetPointAt(Key, to);
                result = true;
            }
            return result;
        }

        public void Update(int focalKey, float t)
        {
            FocalKey = focalKey;
            T = t;
        }

        public static bool operator ==(PointOnFocal left, PointOnFocal right) =>
            left.Key == right.Key && left.FocalKey == right.FocalKey;
        public static bool operator !=(PointOnFocal left, PointOnFocal right) =>
            left.Key != right.Key || left.FocalKey != right.FocalKey;

        public override bool Equals(object obj) => obj is PointOnFocal value && this == value;
        public override int GetHashCode() => Key * 23 + FocalKey * 31;
    }
}
