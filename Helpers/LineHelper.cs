using System;
using System.Windows;

namespace ColorMixing.Helpers
{
    /// <summary>
    /// Helps to calculate intersection of object and connection line
    /// </summary>
    public static class LineHelper
    {
        /// <summary>
        /// Calculate the intersection point of line and circle
        /// </summary>
        public static Point? CalculateIntersection(Point circleCenter, double circleRadius, Point lineStart)
        {
            if (Math.Abs(circleCenter.X - lineStart.X) < double.Epsilon)
            {
                if (circleCenter.Y > lineStart.Y)
                {
                    return new Point(circleCenter.X, circleCenter.Y - circleRadius);
                }
                return new Point(circleCenter.X, circleCenter.Y - circleRadius);
            }
            if (Math.Abs(circleCenter.Y - lineStart.Y) < double.Epsilon)
            {
                if (circleCenter.X > lineStart.X)
                {
                    return new Point(circleCenter.X - circleRadius, circleCenter.Y);
                }
                return new Point(circleCenter.X + circleRadius, circleCenter.Y);
            }

            // translate to origin point
            var translate = new Vector(-circleCenter.X, -circleCenter.Y);

            circleCenter = circleCenter + translate;
            lineStart = lineStart + translate;

            // y=kx+t -> kx1+t=y1, kx2+t=y2 
            // k=(y1-y2)/(x1-x2), t=y1-kx1
            var k = (circleCenter.Y - lineStart.Y) / (circleCenter.X - lineStart.X);
            var t = circleCenter.Y - k * circleCenter.X;

            // x^2+y^2=r^2, y=kx+t
            // x^2+(kx+t)^2=r^2  ->  (k^2+1)*x^2+2ktx+(t^2-r^2)=0
            // ax^2+bx+c=0  ->  x1=[-b+sqrt(b^2-4ac)]/2a  x2=[-b-sqrt(b^2-4ac)]/2a

            var r = circleRadius;

            var a = k * k + 1;
            var b = 2 * k * t;
            var c = t * t - r * r;

            var delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                // has no intersection
                return null;
            }

            var sqrt = Math.Sqrt(delta);

            var x1 = (-b + sqrt) / (2 * a);
            var y1 = k * x1 + t;

            var x2 = (-b - sqrt) / (2 * a);
            var y2 = k * x2 + t;

            var point1 = new Point(x1, y1);
            var point2 = new Point(x2, y2);

            if ((point1 - lineStart).Length < (point2 - lineStart).Length)
            {
                return point1 - translate;
            }
            return point2 - translate;
        }
    }
}