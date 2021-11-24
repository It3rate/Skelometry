using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Controller;
using Vis.Model.Primitives;
using Vis.Model.UI;

namespace Vis.Model.Agent
{
    public class VisAgent : IAgent
    {
        public VisPad WorkingPad { get; private set; }
        public VisPad FocusPad { get; private set; }
        public VisPad ViewPad { get; private set; }

        public UIStatus Status { get; }

        private IRenderer _renderer;

        public VisSkills Skills { get; }
        public IPath AnchorLine { get; private set; }

        public VisAgent(IRenderer renderer)
        {
            _renderer = renderer;

            Skills = new VisSkills();
            WorkingPad = new VisPad(typeof(VisPoint), 250, 250, PadKind.Working);
            FocusPad = new VisPad(typeof(VisPoint), 250, 250, PadKind.Focus);
            ViewPad = new VisPad(typeof(VisPoint), 250, 250, PadKind.View);
            _renderer.Status = new UIStatus(WorkingPad, FocusPad, ViewPad);
            _renderer.DrawingComplete += _renderer_DrawingComplete;
        }

        public void Clear()
        {
	        WorkingPad.Clear();
	        FocusPad.Clear();
            ViewPad.Clear();
        }

        public int _unitPixels = 220;
        public void Draw()
        {
	        Clear();
            Skills.ResetFocus();
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

            Skills.TranslateFocus(bx.Size.X * 1.1f, 0);
        }
        private void _renderer_DrawingComplete(object sender, EventArgs e)
        {
        }
    }
}
