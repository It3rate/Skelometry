using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vis.Model.Primitives
{
    public class VisLocator
    {
    }

    public enum ClockDirection { CW, CCW }
    public static class ClockDirectionExtensions
    {
        public static ClockDirection Counter(this ClockDirection direction) => direction == ClockDirection.CW ? ClockDirection.CCW : ClockDirection.CW;
    }

    public enum CompassDirection { Default, N, NE, E, SE, S, SW, W, NW, Center, WE, NS }
    public static class CompassDirectionExtensions
    {
        private const float pi4 = (float)(Math.PI / 4f);

        public static float Radians(this CompassDirection direction)
        {
            float result;
            switch (direction)
            {
                case CompassDirection.N:
                    result = pi4 * 2;
                    break;
                case CompassDirection.S:
                    result = pi4 * -2f;
                    break;
                case CompassDirection.E:
                    result = pi4 * 0f;
                    break;
                case CompassDirection.W:
                    result = pi4 * 1f;
                    break;

                case CompassDirection.NW:
                    result = pi4 * 3f;
                    break;
                case CompassDirection.NE:
                    result = pi4 * 1f;
                    break;
                case CompassDirection.SW:
                    result = pi4 * -3f;
                    break;
                case CompassDirection.SE:
                    result = pi4 * -1f;
                    break;

                case CompassDirection.NS:
                    result = pi4 * 2f;
                    break;
                case CompassDirection.WE:
                    result = pi4 * 1f;
                    break;
                default:
                    result = 0f;
                    break;
            }
            return result;
        }

        private static float sin45 = (float)(1.0 / Math.Sin(Math.PI / 4.0));
        public static VisLine GetLineFrom(this CompassDirection direction, VisRectangle rect, float offset = 0)
        {
            VisLine result;
            var x = rect.X;
            var y = rect.Y;
            var rx = rect.HalfSize.X;
            var ry = rect.HalfSize.Y;
            switch (direction)
            {
                case CompassDirection.N:
                    result = VisLine.ByEndpoints(x - rx, y - ry + offset, x + rx, y - ry + offset);
                    break;
                case CompassDirection.S:
                    result = VisLine.ByEndpoints(x - rx, y + ry + offset, x + rx, y + ry + offset);
                    break;
                case CompassDirection.E:
                    result = VisLine.ByEndpoints(x + rx + offset, y - ry, x + rx + offset, y + ry);
                    break;
                case CompassDirection.W:
                    result = VisLine.ByEndpoints(x - rx + offset, y - ry, x - rx + offset, y + ry);
                    break;

                // diagonal from given point - these can go both directions because the origin depends on the use case (V vs 7)
                case CompassDirection.NW:
                    result = VisLine.ByEndpoints(x - rx + offset * sin45, y - ry + offset * sin45, x + rx + offset * sin45, y + ry + offset * sin45);
                    break;
                case CompassDirection.NE:
                    result = VisLine.ByEndpoints(x + rx - offset * sin45, y - ry + offset * sin45, x - rx - offset * sin45, y + ry + offset * sin45);
                    break;
                case CompassDirection.SW:
                    result = VisLine.ByEndpoints(x - rx - offset * sin45, y + ry - offset * sin45, x + rx - offset * sin45, y - ry - offset * sin45);
                    break;
                case CompassDirection.SE:
                    result = VisLine.ByEndpoints(x + rx + offset * sin45, y + ry - offset * sin45, x - rx + offset * sin45, y - ry - offset * sin45);
                    break;

                // centered lines
                case CompassDirection.NS:
                    result = VisLine.ByEndpoints(x + offset, y - ry, x + offset, y + ry);
                    break;
                case CompassDirection.WE:
                    result = VisLine.ByEndpoints(x - rx, y + offset, x + rx, y + offset);
                    break;
                default:
                    result = VisLine.ByEndpoints(x - rx + offset, y - ry, x - rx + offset, y + ry);
                    break;
            }
            return result;
        }

        public static VisPoint GetPointFrom(this CompassDirection direction, VisRectangle rect)
        {
            VisPoint result;
            var x = rect.X;
            var y = rect.Y;
            var rx = rect.HalfSize.X;
            var ry = rect.HalfSize.Y;
            switch (direction)
            {
                case CompassDirection.N:
                    result = new VisPoint(x, y - ry);
                    break;
                case CompassDirection.S:
                    result = new VisPoint(x, y + ry);
                    break;
                case CompassDirection.E:
                    result = new VisPoint(x + rx, y);
                    break;
                case CompassDirection.W:
                    result = new VisPoint(x - rx, y);
                    break;

                case CompassDirection.NW:
                    result = new VisPoint(x - rx, y - ry);
                    break;
                case CompassDirection.NE:
                    result = new VisPoint(x + rx, y - ry);
                    break;
                case CompassDirection.SW:
                    result = new VisPoint(x - rx, y + ry);
                    break;
                case CompassDirection.SE:
                    result = new VisPoint(x + rx, y + ry);
                    break;
                default:
                    result = new VisPoint(x, y);
                    break;
            }
            return result;
        }
    }

}
