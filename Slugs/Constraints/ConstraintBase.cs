using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class ConstraintBase : IConstraint
    {
	    public IElement StartElement { get; }
	    public virtual bool HasElement(int key) => AffectedKeys.Contains(key);
	    public List<int> AffectedKeys { get; } = new List<int>();

	    protected ConstraintBase(IElement startElement)
	    {
		    StartElement = startElement;
		    AffectedKeys.AddRange(StartElement.AllKeys);
        }

        public virtual void OnAddConstraint() { }
	    public virtual void OnRemoveConstraint() { }
	    public virtual void OnElementChanged(IElement changedElement) { }

    }

    public abstract class TwoElementConstraintBase : ConstraintBase, ITwoElementConstraint
    {
	    public IElement EndElement { get; }
        public IElement OtherElement(int originalKey) => originalKey == StartElement.Key ? EndElement : StartElement;

	    protected TwoElementConstraintBase(IElement startElement, IElement endElement) : base(startElement)
	    {
		    EndElement = endElement;
		    AffectedKeys.AddRange(EndElement.AllKeys);
        }

        public virtual void OnStartChanged() { }
	    public virtual void OnEndChanged() { }

	    public override void OnElementChanged(IElement changedElement)
	    {
		    if (StartElement.AllKeys.Contains(changedElement.Key))
		    {
			    OnStartChanged();
		    }
		    else if (EndElement.AllKeys.Contains(changedElement.Key))
		    {
			    OnEndChanged();
		    }
	    }
    }

}
