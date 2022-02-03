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
	    public readonly List<SKPoint> SetPositions = new List<SKPoint>();

        public Group(PadKind padKind) : base(padKind)
	    {
	    }

        public int Count => _elementKeys.Count;
	    public ElementKind Kind => _elementKeys.Count == 0 ? ElementKind.None : _elementKeys.Count > 1 ? ElementKind.Multiple : FirstElement.ElementKind;
	    public IElement FirstElement => ElementAt(0);
	    public IElement LastElement => ElementAt(_elementKeys.Count - 1);
        public IElement ElementAt(int index) => index >= 0 && index < _elementKeys.Count ? Pad.ElementAt(_elementKeys[index]) : TerminalPoint.Empty;
        public void Add(int elementKey)
        {
	        Add(Pad.ElementAt(elementKey));
        }
        public void Add(IElement element)
        {
	        _elementKeys.Add(element.Key);
	        SetPositions.AddRange(element.SKPoints);
        }
        public void Add(params int[] elementKeys)
        {
	        foreach (var key in elementKeys)
	        {
		        Add(key);
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
	    public void Clear()
	    {
		    _elementKeys.Clear();
		    SetPositions.Clear();
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
			    foreach (var element in _elementKeys)
			    {
				    yield return ElementAt(Key);
			    }
		    }
	    }
	    public IEnumerable<int> ElementKeys
	    {
		    get
		    {
			    foreach (var key in _elementKeys)
			    {
				    yield return key;
			    }
		    }
	    }
    }
}
