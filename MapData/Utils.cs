using System;
using MiniMap.Geometry;

namespace DataFrame
{
	static class Utils
	{
		public static Point LocalToPrj(Point center, double dirInRadian, double x, double y = 0.0)
		{
			double sinA = Math.Sin(dirInRadian);
			double cosA = Math.Cos(dirInRadian);

			double newX = center.X + x * cosA - y * sinA;
			double newY = center.Y + x * sinA + y * cosA;
			return new Point(newX, newY);
		}

		public static Point PrjToLocal(Point center, double dirInRadian, Point ptPrj)
		{
			double sinA = Math.Sin(dirInRadian);
			double cosA = Math.Cos(dirInRadian);

			double dX = ptPrj.X - center.X;
			double dY = ptPrj.Y - center.Y;

			double newX = dY * sinA + dX * cosA;
			double newY = dY * cosA - dX * sinA;
			return new Point(newX, newY);
		}

		private static double nearDistance(Point Pnt0, Point Pnt1)
		{
			#region constants
			const double DegToRadValue = Math.PI / 180.0;
			const double AWG = 6378137.0;
			const double FWG = 1.0 / 298.257223563;
			const double E2WG = FWG * (2.0 - FWG);
			#endregion

			double fX0 = DegToRadValue * Pnt0.X;
			double fX1 = DegToRadValue * Pnt1.X;

			double fY0 = DegToRadValue * Pnt0.Y;
			double fY1 = DegToRadValue * Pnt1.Y;

			double TanU1 = (1.0 - FWG) * Math.Tan(fY0);
			double TanU2 = (1.0 - FWG) * Math.Tan(fY1);

			double fTmp = Math.Atan(TanU1);
			double CosU1 = Math.Cos(fTmp);
			double SinU1 = Math.Sin(fTmp);

			fTmp = Math.Atan(TanU2);
			double CosU2 = Math.Cos(fTmp);
			double SinU2 = Math.Sin(fTmp);

			fTmp = Math.Sin(0.5 * (fY0 + fY1));
			double Rm = AWG * (1.0 - E2WG) / (1.0 - E2WG * fTmp * fTmp);

			fTmp = fX1 - fX0;
			//double SinL = Math.Sin(fTmp);
			double CosL = Math.Cos(fTmp);

			fTmp = SinU1 * SinU2 + CosU1 * CosU2 * CosL;
			fTmp = fTmp > 1.0 ? 1.0 : fTmp;
			fTmp = fTmp < -1.0 ? -1.0 : fTmp;
			return Math.Acos(fTmp) * Rm;
		}

		public static void CalculateRatio(Envelope extend, out double kx, out double ky)
		{
			Point ptMdl = new Point(0.5 * (extend.ptMin.X + extend.ptMax.X), 0.5 * (extend.ptMin.Y + extend.ptMax.Y));
			Point ptL = new Point(ptMdl.X - 0.01, ptMdl.Y);
			Point ptR = new Point(ptMdl.X + 0.01, ptMdl.Y);

			Point ptT = new Point(ptMdl.X, ptMdl.Y - 0.01);
			Point ptB = new Point(ptMdl.X, ptMdl.Y + 0.01);

			double dx = nearDistance(ptL, ptR);
			double dy = nearDistance(ptT, ptB);

			if (dx < dy)
			{
				ky = 1.0;
				kx = dx / dy;
			}
			else
			{
				kx = 1.0;
				ky = dy / dx;
			}
		}
	}
}
