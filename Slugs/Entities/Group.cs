using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Group : ElementBase
    {
	    public override ElementKind ElementKind => Entities.ElementKind.Group;
	    public override IElement EmptyElement => Empty;
	    public static readonly Group Empty = new Group();
        public Group() : base(true) { }

	    private readonly List<int> _elementKeys = new List<int>();
	    public readonly Dictionary<int, List<SKPoint>> InitialPositions = new Dictionary<int, List<SKPoint>>();
	    public List<SKPoint> InitialPositionFor(int key) => InitialPositions[key];

        public Group(PadKind padKind) : base(padKind)
	    {
	    }
        public int Count => _elementKeys.Count;
	    public ElementKind Kind => _elementKeys.Count == 0 ? ElementKind.None : _elementKeys.Count > 1 ? ElementKind.Multiple : FirstElement.ElementKind;
	    public IElement FirstElement => ElementByIndex(0);
	    public IElement LastElement => ElementByIndex(_elementKeys.Count - 1);
	    public IElement ElementByIndex(int index) => index >= 0 && index < _elementKeys.Count ? Pad.ElementAt(_elementKeys[index]) : TerminalPoint.Empty;
	    public IElement ElementAt(int key) => Pad.ElementAt(key);

        public void Add(IElement element)
        {
	        Add(element.Key);
        }
        public void Add(params int[] elementKeys)
        {
	        foreach (var key in elementKeys)
	        {
		        if (!ElementKeys.Contains(key))
		        {
			        _elementKeys.Add(key);
			        var pts = new List<SKPoint>();
			        pts.AddRange(Pad.ElementAt(key).SKPoints);
			        InitialPositions[key] = pts;
		        }
	        }
        }
        public void Add(IEnumerable<int> elementKeys)
        {
	        foreach (var key in elementKeys)
	        {
		        Add(key);
	        }
        }
        public void AddRange(IEnumerable<IElement> elements)
        {
	        foreach (var element in elements)
	        {
		        Add(element);
	        }
        }
        public void AddRange(IEnumerable<int> keys)
        {
            Add(keys);
        }
        public void Clear()
	    {
		    _elementKeys.Clear();
		    InitialPositions.Clear();
	    }
	    public bool ContainsElement(IElement element) => _elementKeys.Contains(element.Key);

	    public override List<IPoint> Points
	    {
		    get
		    {
			    var pts = new List<IPoint>();
			    foreach (var element in Elements)
			    {
				    pts.AddRange(element.Points);
			    }
			    return pts;
		    }
	    }

        public IEnumerable<IElement> Elements
	    {
		    get
		    {
			    foreach (var key in _elementKeys)
			    {
				    yield return ElementAt(key);
			    }
		    }
        }
        public IEnumerable<int> ElementKeys => _elementKeys;
        public int[] ElementKeysCopy => _elementKeys.ToArray();
    }
}
