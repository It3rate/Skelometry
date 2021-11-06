using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vis.Model.Controller
{
    public class VisPens
    {
        public List<Pen> Pens = new List<Pen>();
        public VisPens(float scale)
        {
            GenPens(scale);
        }
        public Pen this[int index]
        {
            get { return Pens[index]; }
        }
        public Pen GetPenForElement(PadAttributes attributes)
        {
	        Pen result = Pens[1]; // todo: sync pens with skia
            return result;
        }

        public Pen GetPenForUIType(UIType uiType)
        {
	        Pen result = Pens[1]; // todo: sync pens with skia
	        return result;
        }

        private void GenPens(float scale)
        {
            Pens.Clear();
            Pens.Add(GetPen(Color.LightGray, 8f / scale));
            Pens.Add(GetPen(Color.Black, 8f / scale));
            Pens.Add(GetPen(Color.DarkRed, 8f / scale));
            Pens.Add(GetPen(Color.Orange, 8f / scale));
            Pens.Add(GetPen(Color.DarkGreen, 8f / scale));
            Pens.Add(GetPen(Color.DarkBlue, 8f / scale));
            Pens.Add(GetPen(Color.DarkViolet, 16f / scale));
            Pens.Add(GetPen(Color.Red, 32f / scale));
        }
        public Pen GetPen(Color color, float width)
        {
            var pen = new Pen(color, width);
            pen.LineJoin = LineJoin.Round;
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
    }
}
