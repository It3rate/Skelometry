using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Controller;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
    public class VisAgent : IAgent
    {
        public VisPad<VisPoint> WorkingPad { get; private set; }
        public VisPad<VisPoint> FocusPad { get; private set; }
        public VisPad<VisStroke> ViewPad { get; private set; }

        private IRenderer _renderer;

        public VisSkills Skills { get; }

        public VisAgent(IRenderer renderer)
        {
            _renderer = renderer;

            Skills = new VisSkills();
            WorkingPad = new VisPad<VisPoint>(250, 250);
            FocusPad = new VisPad<VisPoint>(250, 250);
            ViewPad = new VisPad<VisStroke>(250, 250);
            _renderer.Pads = new List<IPad>() { WorkingPad, FocusPad, ViewPad };
            _renderer.DrawingComplete += _renderer_DrawingComplete;
        }

        public void Clear()
        {
            FocusPad.Clear();
            ViewPad.Clear();
        }

        public int _unitPixels = 220;
        public void Draw()
        {
	        DrawLetter("A");
            DrawLetter("R");
            DrawLetter("C");
            DrawLetter("B");
        }

        private void DrawLetter(string letter)
        {
            VisRectangle bx;
            switch (letter)
            {
                case "A":
                    bx = Skills.LetterA(FocusPad, ViewPad);
                    break;
                case "B":
                    bx = Skills.LetterB(FocusPad, ViewPad);
                    break;
                case "C":
                    bx = Skills.LetterC(FocusPad, ViewPad);
                    break;
                default:
                    bx = Skills.LetterR(FocusPad, ViewPad);
                    break;
            }
            //todo: need to create new dynamic contexts for each focused view. Can be per letter for now, probably copies of agent.
            //_renderer.Draw(this);
            //_renderer.TranslateContext(bx.Size.X * 1.1f, 0);
            //Clear();

        }
        private void _renderer_DrawingComplete(object sender, EventArgs e)
        {
	        Clear();
        }
    }
}
