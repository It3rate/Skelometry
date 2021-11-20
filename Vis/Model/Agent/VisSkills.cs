using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Connections;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
    public class VisSkills
    {
        public List<float> LetterWidths = new List<float>() { };
        // Use separate pads for imagined and seen elements

        // pads are a complex version of the visual-motor connection of neurons in early life: see, process/transmit, signal muscles
        // Sensory capture - Input (visualization)
        // Understanding - processing to primitive shapes on pad (Primitives)
        // Planning - Lookup of letter recipes (cell)
        // Deciding - Form next stroke plan (thought)
        // Transmitting - encode and send to renderer (via nervous system)
        // Motor - render (muscle motion)
        // Feedback - back to step 1

        public float halfM = 0.5f;
        public float offsetX = 0.01f;
        public float offsetY = 0.1f;

        public void ResetFocus()
        {
	        offsetX = 0.01f;
	        offsetY = 0.1f;
        }

        public void TranslateFocus(float x, float y)
        {
	        offsetX += x;
	        offsetY += y;
        }

        private VisRectangle GetLetterbox(float cx, float cy, float cornerX, float cornerY)
        {
            return new VisRectangle(cx + offsetX, cy + offsetY, cornerX + offsetX, cornerY + offsetY);
        }
        public VisRectangle LetterA(VisPad focusPad, VisPad viewPad)
        {
            var letterbox = GetLetterbox(halfM * .9f, 0.5f, 0f, 0f);
            focusPad.Add(letterbox);

            var topLine = letterbox.GetLine(CompassDirection.N);
            var bottomLine = letterbox.GetLine(CompassDirection.S);

            var leftStroke = new VisStroke(topLine.MidNode, bottomLine.StartNode);
            viewPad.Add(leftStroke);

            var rightStroke = new VisStroke(topLine.MidNode, bottomLine.EndNode);
            viewPad.Add(rightStroke);

            var midStroke = new VisStroke(leftStroke.NodeAt(.6f), rightStroke.NodeAt(.6f));
            viewPad.Add(midStroke);

            return letterbox;
        }
        public VisRectangle LetterR(VisPad focusPad, VisPad viewPad)
        {
            // LB: imagine letterbox
            // LB0: find left vertical line (need to find sub pieces of imagined elements - if using the whole rect can just reference it)
            // LeftS: make left stroke along LB0 with top and bottom tipJoints
            // C: imagine circle to the top right (no need to find it as it is referenced as a whole)
            // LoopS: FullStroke loop with joints LeftS:(corner), C:1 (top tangent), LeftS:0.5 (butt)
            // Find loop stroke
            // TailS: Make with joints LoopS:0.8 (butt), LeftS:1 offset 1 (tip)

            var letterbox = GetLetterbox(0.25f, 0.5f, 0f, 0f);
            focusPad.Add(letterbox);

            var leftLine = letterbox.GetLine(CompassDirection.W);
            var leftStroke = new VisStroke(leftLine.StartNode, leftLine.EndNode);
            viewPad.Add(leftStroke);

            var topLine = letterbox.GetLine(CompassDirection.N);
            var rightLine = letterbox.GetLine(CompassDirection.E);
            var seenLeftStroke = viewPad.GetSimilar((IPath)leftLine);

            //var radius = topLine.NodeAt(0.55f, 0f);
            //var center = topLine.NodeAt(0.55f, 0.22f);
            //var topCircle = new Circle(center, radius);
            var topCircle = VisCircle.CircleFromLineAndPoint(topLine, rightLine.NodeAt(0.25f), ClockDirection.CCW);
            focusPad.Add(topCircle);
            var circleNode = new TangentNode(topCircle, ClockDirection.CW);

            var midNode = seenLeftStroke.NodeAt(0.55f);

            //var loopStartJoint = new VisJoint(seenLeftStroke.StartNode, circleNode0, VisJointType.Corner);
            //var circleJoint = new VisJoint(circleNode0, circleNode1, VisJointType.Curve);
            //var loopEndJoint = new VisJoint(circleNode1, seenLeftStroke.NodeAt(0.5f), VisJointType.Butt);
            var loopStroke = new VisStroke(seenLeftStroke.StartNode, circleNode, midNode);
            viewPad.Add(loopStroke);

            // Need a way to create a butt joint from a start to end point (or angle) at the first intersection point of a stroke.
            // Joints tell about shapes, but they should also be the way this thinks about and creates shapes.
            var circRefNode = new VisNode(topCircle, 0);
            var tailStart = loopStroke.NodeAt(rTailStart);// topCircle.NodeAt(CompassDirection.S); 
            var tailStroke = new VisStroke(tailStart, rightLine.NodeAt(1f));
            viewPad.Add(tailStroke);

            return letterbox;
        }

        public float rTailStart = 0.80f;

        public VisRectangle LetterC(VisPad focusPad, VisPad viewPad)
        {
            var letterbox = GetLetterbox(0.5f, 0.5f, 0f, 0f);
            focusPad.Add(letterbox);
            var rightLine = letterbox.GetLine(CompassDirection.E);

            var circle = new VisCircle(letterbox.Center, letterbox.GetPoint(CompassDirection.E), ClockDirection.CCW);
            focusPad.Add(circle);
            var startC = circle.NodeAt(0.1f);
            var endC = circle.NodeAt(0.9f);
            var circleNode = new TangentNode(circle, ClockDirection.CCW);
            //var loopStroke = new Stroke(rightLine.NodeAt(.4f), circleNode, rightLine.NodeAt(.6f));
            var loopStroke = new VisStroke(startC, circleNode, endC);
            viewPad.Add(loopStroke);

            return letterbox;
        }
        public VisRectangle LetterB(VisPad focusPad, VisPad viewPad)
        {
            var letterbox = GetLetterbox(0.30f, 0.5f, 0f, 0f);
            focusPad.Add(letterbox);

            var leftLine = letterbox.GetLine(CompassDirection.W);
            var leftStroke = new VisStroke(leftLine.StartNode, leftLine.EndNode);
            viewPad.Add(leftStroke);

            var rightLine = letterbox.GetLine(CompassDirection.E);
            var seenLeftStroke = viewPad.GetSimilar((IPath)leftLine);

            var midLine = letterbox.GetLine(CompassDirection.N, 0.45f);
            var midNode = midLine.StartNode;
            focusPad.Add(midLine);

            //var trLine = Line.ByEndpoints(rightLine.StartPoint, midLine.EndPoint);
            var topLine = letterbox.GetLine(CompassDirection.N);
            var trLine = VisLine.ByEndpoints(topLine.GetPoint(bTopCenter), midLine.GetPoint(bTopCenter));
            var topCircle = VisCircle.CircleFromLineAndPoint(midLine, trLine.MidNode, ClockDirection.CW); //rightLine.NodeAt(trLine.MidPoint.Y), ClockDirection.CW);
            focusPad.Add(topCircle);
            var circleNode = new TangentNode(topCircle);
            var loopStroke = new VisStroke(seenLeftStroke.StartNode, circleNode, midNode);
            viewPad.Add(loopStroke);

            var botLine = letterbox.GetLine(CompassDirection.S);
            var botCircle = VisCircle.CircleFromLineAndPoint(rightLine, botLine.NodeAt(.538f), ClockDirection.CW);
            focusPad.Add(botCircle);
            circleNode = new TangentNode(botCircle);
            loopStroke = new VisStroke(midNode, circleNode, seenLeftStroke.EndNode);
            viewPad.Add(loopStroke);

            return letterbox;
        }

        public float bTopCenter = 0.9f;
    }
}
