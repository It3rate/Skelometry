using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class KeyedCollection<T> where T : IElement
    {
	    private readonly Dictionary<int, T> _dict = new Dictionary<int, T>();
	    protected IElement Empty { get; }

        public IEnumerable<T> Values => _dict.Values;
        public T ValueAt(int key)
        {
            var success = _dict.TryGetValue(key, out T result);
            return success ? result : (T)Empty;
        }
        public void SetElementAt(int key, T value)
        {
            _dict[key] = value;
        }
        public RefPoint[] TerminalPoints()
        {
	        throw new NotImplementedException();
        }
        public void Merge(T from, T to)
        {
	        throw new NotImplementedException();
        }
        public void OffsetPoints(SKPoint diff)
        {
	        throw new NotImplementedException();
        }
    }
}
