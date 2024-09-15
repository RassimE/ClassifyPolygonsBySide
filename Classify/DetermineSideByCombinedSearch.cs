using MiniMap.Geometry;
using System;
using System.Collections.Generic;

namespace CutOperatorTest
{
	internal static class DetermineSideByCombinedSearch
	{
		// Epsilons are added to address inconsistencies
		const double epsilonD = 0.000001;
		const double epsilonA = 0.000001;
		const int lineStringPointCountThreshold = 60;

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

				// Calculating the parameters of Hough transformation. "a" stands for the angle in radians between the positive x-axis and the line
				var a = Math.Atan2(dy, dx);

				A = Modulus(a);

				// Finding the direction of the line based on the difference between its angle from the positive x-axis and its modulus
				LessThanPI = SubtractAngles(a, A) < epsilonA;

				var nXs = start.X - xOrigin;
				var nXe = end.X - xOrigin;

				var nYs = start.Y - yOrigin;
				var nYe = end.Y - yOrigin;

				// Stands for the perpendicular distance from the origin (xOrigin, yOrigin) point to a line. Excluded division by 2 from the formula as not needed in code.
				D = Math.Abs(nYs * nXe - nXs * nYe) / Math.Sqrt(dx * dx + dy * dy);

				// Finding the dominant component of coordinate, meaning we will use X if the absolute value of deltaX is higher that the absolute value
				// of deltaY, and the opposite otherwise.
				if (Math.Abs(dy) > Math.Abs(dx))
				{
					if (start.Y > end.Y)
					{
						P0 = nYe;
						P1 = nYs;
					}
					else
					{
						P0 = nYs;
						P1 = nYe;
					}
				}
				else
				{
					if (start.X > end.X)
					{
						P0 = nXe;
						P1 = nXs;
					}
					else
					{
						P0 = nXs;
						P1 = nXe;
					}
				}

				// The epsilon is added here for optimization purposes, as in later condition checks this addition would be repetitive.
				P1 += epsilonD;
			}
#if DEBUG
			public override string ToString()
			{
				return string.Format("{0:N} / {1:N}", D, A);
			}
#endif
		}

		public static void СlassifyBySide(List<GeometryBase> inputPolygons, LineString lineString,
			double xOrigin, double yOrigin, out GeometryBase geometryLeft, out GeometryBase geometryRight)
		{
			void LinearSearchEdge(MultiPolygon leftGeometry, MultiPolygon rightGeometry)
			{
				// calculating Hough parameters for each piece of polyline
				LineStat[] segmentStats = new LineStat[lineString.Count - 1];

				for (var i = 1; i < lineString.Count; i++)
					segmentStats[i - 1] = new LineStat(lineString[i - 1], lineString[i], xOrigin, yOrigin);

				// executing the algorithm of linear search
				Array.Sort(segmentStats, (x, y) => x.D.CompareTo(y.D));

				//defining the minimum and maximum d-values to look for with consideration for inconsistency
				double dMin = segmentStats[0].D - epsilonD;
				double dMax = segmentStats[segmentStats.Length - 1].D + epsilonD;

				foreach (Polygon polygon in inputPolygons)
				{
					bool classified = false;

					for (var i = 1; i < polygon.ExteriorRing.Count; i++)
					{
						LineStat currEdge = new LineStat(polygon.ExteriorRing[i - 1], polygon.ExteriorRing[i], xOrigin, yOrigin);

						// Skip the edge if the value is outside our defined boundaries
						if (currEdge.D < dMin || currEdge.D > dMax)
							continue;

						int downIndex = 0;

						// If there are elements to the right and the current d-value is too low, shift the lowest index up,
						// therefore lower indexes won't be searched later
						while (downIndex < segmentStats.Length && segmentStats[downIndex].D < currEdge.D - epsilonD)
							downIndex++;

						// If there are no more elements to the right, skip edge
						if (downIndex >= segmentStats.Length)
							continue;

						// If the difference is higher than the inconsistency, skip edge
						if (segmentStats[downIndex].D - currEdge.D > epsilonD)
							continue;

						int upIndex = downIndex + 1;
						// If there are elements to the right and the current difference is smaller than the allowed inconsistency, shift the highest index up, so
						// more elements will be searched
						while (upIndex < segmentStats.Length && Math.Abs(segmentStats[upIndex].D - currEdge.D) < epsilonD)
							upIndex++;

						for (int j = downIndex; j < upIndex; j++)
						{
							// Skip if exceeds if the difference of angles against positive x-axis exceeds allowed inconsistency
							if (SubtractAngles(segmentStats[j].A, currEdge.A) >= epsilonA)
								continue;

							// Skip if the coordinates do not align and there is no overlap present between the lines
							if (segmentStats[j].P0 >= currEdge.P1 || currEdge.P0 >= segmentStats[j].P1)
								continue;

							// If the line and polygon edge align on direction, the edge is on the left, and the opposite otherwise
							if (segmentStats[j].LessThanPI != currEdge.LessThanPI)
								leftGeometry.Add(polygon);
							else
								rightGeometry.Add(polygon);

							classified = true;
							break;
						}

						if (classified)
							break;
					}
				}
			}

			void BinarySearchEdge(MultiPolygon leftGeometry, MultiPolygon rightGeometry)
			{
				// calculating Hough parameters for each piece of polyline
				LineStat[] segmentStats = new LineStat[lineString.Count - 1];

				for (var i = 1; i < lineString.Count; i++)
					segmentStats[i - 1] = new LineStat(lineString[i - 1], lineString[i], xOrigin, yOrigin);


				// executing the algorithm of binary search
				Array.Sort(segmentStats, (x, y) => x.D.CompareTo(y.D));

				//defining the minimum and maximum d-values to look for with consideration for inconsistency
				double dMin = segmentStats[0].D - epsilonD;
				double dMax = segmentStats[segmentStats.Length - 1].D + epsilonD;

				foreach (Polygon polygon in inputPolygons)
				{
					bool classified = false;

					for (var i = 1; i < polygon.ExteriorRing.Count; i++)
					{
						LineStat currEdge = new LineStat(polygon.ExteriorRing[i - 1], polygon.ExteriorRing[i], xOrigin, yOrigin);

						// Skip the edge if the value is outside our defined boundaries
						if (currEdge.D < dMin || currEdge.D > dMax)
							continue;

						int downIndex = 0;
						int upIndex = segmentStats.Length - 1;
						int midIndex = 0;
						bool found = false;
						//int iter = -1;
						while (downIndex <= upIndex)
						{
							//iter++;
							// Start in the middle if the number of elements is 8 or less
							midIndex = (downIndex + upIndex) >> 1;

							if (currEdge.D - segmentStats[midIndex].D > epsilonD)
								downIndex = midIndex + 1;
							else if (segmentStats[midIndex].D - currEdge.D > epsilonD)
								upIndex = midIndex - 1;
							else
							{
								found = true;
								break;
							}
						}

						if (!found)
							continue;

						downIndex = midIndex;
						while (downIndex >= 1 && segmentStats[downIndex - 1].D > currEdge.D - epsilonD)
							downIndex--;

						upIndex = midIndex + 1;
						while (upIndex < segmentStats.Length && segmentStats[upIndex].D < currEdge.D + epsilonD)
							upIndex++;

						for (int j = downIndex; j < upIndex; j++)
						{
							// Skip if exceeds if the difference of angles against positive x-axis exceeds allowed inconsistency
							if (SubtractAngles(segmentStats[j].A, currEdge.A) >= epsilonA)
								continue;

							// Skip if the coordinates do not align and there is no overlap present between the lines
							if (segmentStats[j].P0 >= currEdge.P1 || currEdge.P0 >= segmentStats[j].P1)
								continue;

							// If the line and polygon edge align on direction, the edge is on the left, and the opposite otherwise
							if (segmentStats[j].LessThanPI != currEdge.LessThanPI)
								leftGeometry.Add(polygon);
							else
								rightGeometry.Add(polygon);

							classified = true;
							break;
						}

						if (classified)
							break;
					}
				}
			}

			// creating polygons for "right" and "left"
			geometryLeft = new MultiPolygon();
			geometryRight = new MultiPolygon();

			if (lineString.Count <= lineStringPointCountThreshold)
				LinearSearchEdge((MultiPolygon)geometryLeft, (MultiPolygon)geometryRight);
			else
				BinarySearchEdge((MultiPolygon)geometryLeft, (MultiPolygon)geometryRight);
		}
	}
}
