using MiniMap.Geometry;
using System;

namespace CutOperatorTest.Classify
{
	internal static class Meandr
	{
		public static LineString GenerateMeanderLine(Envelope box, double alpha = 0.0, double k = 0.25)
		{
			Point LocalToPrj(Point center, double dirInRadian, double x, double yy = 0.0)
			{
				double sinA = Math.Sin(dirInRadian);
				double cosA = Math.Cos(dirInRadian);

				double newX = center.X + x * cosA - yy * sinA;
				double newY = center.Y + x * sinA + yy * cosA;
				return new Point(newX, newY);
			}

			var random = new Random();
			//var box = polygon.Extend;

			double deltaX = box.ptMax.X - box.ptMin.X;
			double deltaY = box.ptMax.Y - box.ptMin.Y;

			double diogonal = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

			var startX = box.ptMin.X + 0.5 * deltaX - random.Next(10, 60);
			var startY = box.ptMin.Y + 0.5 * deltaY + random.Next(0, 10);

			var startPoint = new Point(startX, startY);

			double y = k * deltaY;
			var segments = 50;
			var step = diogonal / segments;

			var line = new LineString();

			for (int i = 0; i <= segments; i++)
			{
				double x = i * step - 0.5 * deltaX;

				var point1 = LocalToPrj(startPoint, alpha, x, y);

				y = -y;
				var point2 = LocalToPrj(startPoint, alpha, x, y);

				line.Add(point1);
				line.Add(point2);
			}

			return line;
		}
	}
}
