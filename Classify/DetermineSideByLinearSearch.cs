using MiniMap.Geometry;
using System;
using System.Collections.Generic;

namespace CutOperatorTest
{
    internal static class DetermineSideByLinearSearch
	{
		// Epsilons are added to address inconsistencies
		const double epsilonD = 0.000001;
		const double epsilonA = 0.000001;

		static double Modulus(double x, double y = Math.PI)
		{
			x -= Math.Floor(x / y) * y;

			if (x < 0.0)
				x += y;

			if (x == y)
				return 0.0;

			return x;
		}

        // Calculating the smallest angle between two lines (the one that is smaller than 90 degrees)
        static double SubtractAngles(double X, double Y)
        {
            var result = Modulus(X - Y, 2 * Math.PI);

            if (result > Math.PI)
                return Math.PI - result;

            return result;
        }

        struct LineStat
        {
            public double P0;
            public double P1;
            public double A;
            public double D;
            public bool LessThanPI;

            public LineStat(Point start, Point end, double xOrigin, double yOrigin)
            {
                var dy = end.Y - start.Y;
                var dx = end.X - start.X;

                // Finding the dominant component of coordinate, meaning we will use X if the absolute value of deltaX is higher that the absolute value
                // of deltaY, and the opposite otherwise.
                if (Math.Abs(dy) > Math.Abs(dx))
                {
                    if (start.Y > end.Y)
                    {
                        P0 = end.Y;
                        P1 = start.Y;
                    }
                    else
                    {
                        P0 = start.Y;
                        P1 = end.Y;
                    }
                }
                else
                {
                    if (start.X > end.X)
                    {
                        P0 = end.X;
                        P1 = start.X;
                    }
                    else
                    {
                        P0 = start.X;
                        P1 = end.X;
                    }
                }

                P1 += epsilonD;
                // Calculating the parameters of Hough transformation. "a" stands for the angle in radians between the positive x-axis and the line
                var a = Math.Atan2(dy, dx);

                A = Modulus(a);

				// Stands for the perpendicular distance from the origin (xOrigin, yOrigin) point to a line. Excluded division by 2 from the formula as not needed in code.
				D = Math.Abs((start.Y - yOrigin) * (end.X - xOrigin) - (start.X - xOrigin) * (end.Y - yOrigin)) / Math.Sqrt(dx * dx + dy * dy);

                // Finding the direction of the line based on the difference between its angle from the positive x-axis and its modulus
                LessThanPI = SubtractAngles(a, A) < epsilonA;
            }

			public override string ToString()
            {
                return string.Format("{0:N} / {1:N}", D, A);
            }
        }

		public static void СlassifyBySide(List<GeometryBase> inputPolygons, LineString lineString,
            double xOrigin, double yOrigin, out GeometryBase geometryLeft, out GeometryBase geometryRight)
        {
			// calculating Hough parameters for each piece of polyline
			LineStat[] lineStats = new LineStat[lineString.Count - 1];

            for (var i = 1; i < lineString.Count; i++)
                lineStats[i - 1] = new LineStat(lineString[i - 1], lineString[i], xOrigin, yOrigin);

			// creating polygons for "right" and "left"
			geometryLeft = new MultiPolygon();
			geometryRight = new MultiPolygon();

			// executing the algorithm of linear search
			Array.Sort(lineStats, (x, y) => x.D.CompareTo(y.D));

            //defining the minimum and maximum d-values to look for with consideration for inconsistency
            double dMin = lineStats[0].D - epsilonD;
            double dMax = lineStats[lineStats.Length - 1].D + epsilonD;

            foreach (Polygon polygon in inputPolygons)
            {
                bool classified = false;

                for (var i = 1; i < polygon.ExteriorRing.Count; i++)
                {
                    LineStat currLine = new LineStat(polygon.ExteriorRing[i - 1], polygon.ExteriorRing[i], xOrigin, yOrigin);
                    
                    // Skip the edge if the value is outside our defined boundaries
                    if (currLine.D < dMin || currLine.D > dMax)
                        continue;

                    int downIndex = 0;

                    // If there are elements to the right and the current d-value is too low, shift the lowest index up,
                    // therefore lower indexes won't be searched later
                    while (downIndex < lineStats.Length && lineStats[downIndex].D < currLine.D - epsilonD)
                        downIndex++;

                    // If there are no more elements to the right, skip edge
                    if (downIndex >= lineStats.Length)
                        continue;

                    // If the difference is higher than the inconsistency, skip edge
                    if (lineStats[downIndex].D - currLine.D > epsilonD)
                        continue;

                    int upIndex = downIndex + 1;
                    // If there are elements to the right and the current difference is smaller than the allowed inconsistency, shift the highest index up, so
                    // more elements will be searched
                    while (upIndex < lineStats.Length && Math.Abs(lineStats[upIndex].D - currLine.D) < epsilonD)
                        upIndex++;

                    for (int j = downIndex; j < upIndex; j++)
                    {
                        // Skip if exceeds if the difference of angles against positive x-axis exceeds allowed inconsistency
                        if (SubtractAngles(lineStats[j].A, currLine.A) >= epsilonA)
                            continue;

                        // Skip if the coordinates do not align and there is no overlap present between the lines
                        if (lineStats[j].P0 >= currLine.P1 || currLine.P0 >= lineStats[j].P1)
                            continue;

                        // If the line and polygon edge align on direction, the edge is on the left, and the opposite otherwise
                        if (lineStats[j].LessThanPI != currLine.LessThanPI)
                            ((MultiPolygon)geometryLeft).Add(polygon);
                        else
                            ((MultiPolygon)geometryRight).Add(polygon);

                        classified = true;
                        break;
                    }

                    if (classified)
                        break;
                }
            }
        }
    }
}
