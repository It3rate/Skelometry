using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BondPoint : PointBase, ITValue
    {
        public override ElementKind ElementKind => ElementKind.BondPoint;
        public override IElement EmptyElement => Empty;
        public static readonly BondPoint Empty = new BondPoint();
        private BondPoint() : base(true) { }

        public int FocalKey { get; set; }
        public override List<int> AllKeys => new List<int>() { Key, FocalKey };
        private float _t;
        public float T
        {
	        get => _t;
	        set => _t = IsEmpty ? _t : value;
        }

        public Focal Focal => Pad.FocalAt(FocalKey);
        public Trait Trait => Focal.Trait;
        public int TraitKey => Focal.TraitKey; 
        public Entity Entity => Focal.Entity;
        public int EntityKey => Focal.EntityKey;

        public BondPoint(PadKind padKind, int focalKey, float t) : base(padKind)
        {
            FocalKey = focalKey;
            T = t;
            AllKeys.Add(FocalKey);
        }

        public override bool CanMergeWith(IPoint point)
        {
	        var targetKind = point.TargetPoint.ElementKind;
	        return targetKind == ElementKind.BondPoint || targetKind == ElementKind.FocalPoint;
        }
        public override int MergeInto(IPoint point)
        {
	        var result = Key;
	        if (point.ElementKind == ElementKind.BondPoint)
	        {
		        Pad.SetPointAt(Key, point);
		        result = point.Key;
		        T = Focal.TFromPoint(point.Position).Item1;
	        }
            else if (point is FocalPoint fp)
	        {
		        T = Focal.TFromPoint(fp.Position).Item1;
            }
	        //SetOtherT(T); // this locks the t ratio
	        return result;
        }

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { this };
        public override SKPoint Position
        {
            get
            {
                var focal = Focal;
                return focal.IsEmpty ? SKPoint.Empty : focal.PointAlongLine(T);
            }
            set
            {
	            if (!IsLocked)
	            {
		            var focal = Focal;
		            T = focal.TFromPoint(value, false).Item1;
		            //SetOtherT(T); // this locks the t ratio
	            }
            }
        }

        public Trait GetTrait() => Pad.TraitAt(GetFocal().TraitKey);
        public Focal GetFocal() => Pad.FocalAt(FocalKey);
        public float GetT() => T;
        public IPoint GetStartPoint() => Pad.FocalAt(FocalKey).StartFocalPoint;
        public IPoint GetEndPoint() => Pad.FocalAt(FocalKey).EndFocalPoint;

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

        //public BondPoint OtherPoint { get; set; } // could just store other point...
        private BondPoint OtherPoint
        {
            get
            {
                var result = BondPoint.Empty;
                foreach (var singleBond in Focal.AllBonds)
                {
                    var other = singleBond.OtherPoint(Key);
                    if (!other.IsEmpty)
                    {
                        result = other;
                        break;
                    }
                }
                return result;
            }
        }

        private bool SetOtherT(float val)
        {
	        var result = false;
	        var other = OtherPoint;
	        if (!other.IsEmpty)
	        {
		        other.T = val;
		        result = true;
	        }
	        return result;
        }

        public static bool operator ==(BondPoint left, BondPoint right) =>
            left.Key == right.Key && left.FocalKey == right.FocalKey;
        public static bool operator !=(BondPoint left, BondPoint right) =>
            left.Key != right.Key || left.FocalKey != right.FocalKey;

        public override bool Equals(object obj) => obj is BondPoint value && this == value;
        public override int GetHashCode() => Key * 23 + FocalKey * 31;
    }
}
