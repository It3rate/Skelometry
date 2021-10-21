using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    public class VisAgent
    {
	    public VisPad<Point> FocusPad { get; private set; }
	    public VisPad<Stroke> ViewPad { get; private set; }

	    private VisRenderer _renderer;

	    public VisSkills Skills { get; }

	    public VisAgent(VisRenderer renderer)
	    {
		    _renderer = renderer;

            Skills = new VisSkills();
		    FocusPad = new VisPad<Point>(250, 250);
		    ViewPad = new VisPad<Stroke>(250, 250);

        }

	    public void Clear()
	    {
            FocusPad.Clear();
            ViewPad.Clear();
	    }

	    public int _unitPixels =220;
	    public void Draw(Graphics g)
	    {
		    var state = g.Save();

		    float w = _renderer.Width;
		    float h = _renderer.Height;
		    g.TranslateTransform(10, 10);
            g.ScaleTransform(_unitPixels, _unitPixels);

            DrawLetter(g, "A");
            DrawLetter(g, "R");
            DrawLetter(g, "C");
            DrawLetter(g, "B");

            g.Restore(state);
        }

	    private void DrawLetter(Graphics g, string letter)
	    {
		    Rectangle bx;
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
		    _renderer.Draw(g, this);
		    g.TranslateTransform(bx.Size.X * 1.1f, 0);
		    Clear();

        }
    }
}
