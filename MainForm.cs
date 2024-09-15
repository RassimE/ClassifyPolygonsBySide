using CutOperatorTest;
using CutOperatorTest.Classify;
using DataFrame;
using MiniMap;
using MiniMap.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
	public partial class MainForm : Form
	{
		private readonly MapEngine engine;
		private readonly MapData vectorData;

		private readonly List<Polygon> polygons;
		private readonly List<LineString> lineStrings;
		private Envelope envelope;
		GeometryBase left, right;

		public MainForm()
		{
			InitializeComponent();

			engine = mapControl1.MapGraphics;
			vectorData = new MapData(engine);
			GeoTxtSerializer.Owner = this;

			vectorData.Units = MapUnits.Meters;
			Point ptTHR = new Point(0, 0, 9) { Name = "ptTHR" };
			vectorData.AddGeometry(ptTHR);

			const int n = 10;
			const double dDir = 2.0 * Math.PI / n;
			const double fieldPolySpan = 10000;

			Random rnd = new Random();

			for (int i = 0; i < n; i++)
			{
				double dir = i * dDir;
				double x = fieldPolySpan - 100.0 - 100.0 * rnd.NextDouble();
				double y = fieldPolySpan - 100.0 - 100.0 * rnd.NextDouble();

				Point ptPrj = Utils.LocalToPrj(ptTHR, dir, x, y);
				ptPrj.Name = "DP_" + (i < 10 ? "0" : "") + i.ToString();
				//vectorData.Draw(ptPrj, new Style { Color = 255, Size = 8, Kind = 7 });
				vectorData.AddGeometry(ptPrj);
			}
			envelope = new Envelope();

			LineString ls = Meandr.GenerateMeanderLine(vectorData.Extend);
			ls.Name = "Meandr";
			vectorData.AddGeometry(ls);

			polygons = new List<Polygon>();
			lineStrings = new List<LineString>();
			vectorData.CalckExtent();
			vectorData.DrawData();
			left = right = null;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openMapData.ShowDialog() != DialogResult.OK)
				return;

			try
			{
				int FilterIndex = 1;
				string ext = Path.GetExtension(openMapData.FileName).ToLower();
				switch (ext)
				{
					case ".txt":
						FilterIndex = 1;
						break;
					case ".geojson":
					case ".json":
						FilterIndex = 2;
						break;
					default:
						break;
				}

				if (FilterIndex == 1)
					vectorData.LoadFromFile(openMapData.FileName);
				else if (FilterIndex == 2)
					vectorData.LoadGeoJson(openMapData.FileName);

				mapControl1.Units = vectorData.Units;
				vectorData.DrawData();

				lineStrings.Clear();
				polygons.Clear();
				envelope.SetEmpty();

				foreach (GeometryBase geom in vectorData)
				{
					if (geom.Type == GeometryType.LineString)
						lineStrings.Add((LineString)geom);
					else if (geom.Type == GeometryType.Polygon)
					{
						polygons.Add((Polygon)geom);
						envelope.ExpandToInclude(geom);
					}
				}

				btnReverse.Enabled = lineStrings.Count > 0;
				btnBenc3.Enabled = btnBenc2.Enabled = btnBenc1.Enabled = btnClaasify.Enabled = lineStrings.Count > 0 && polygons.Count > 0;
				lblTime3.Text = lblTime2.Text = lblTime1.Text = "Elapsed time";

				btnRemove.Enabled = false;

				saveMapData.FileName = Path.ChangeExtension(openMapData.FileName, null);
				Text = "Test Application [" + Path.GetFileName(openMapData.FileName) + "]";
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveMapData.ShowDialog() != DialogResult.OK)
				return;

			try
			{
				int FilterIndex = 1;
				string ext = Path.GetExtension(saveMapData.FileName).ToLower();
				switch (ext)
				{
					case ".txt":
						FilterIndex = 1;
						break;
					case ".geojson":
					case ".json":
						FilterIndex = 2;
						break;
					default:
						break;
				}

				if (FilterIndex == 1)
					vectorData.SaveToTxtFile(saveMapData.FileName);
				else if (FilterIndex == 2)
					vectorData.SaveToGeoJsonFile(saveMapData.FileName);

				openMapData.FileName = Path.ChangeExtension(saveMapData.FileName, null);
				Text = "Test Application [" + Path.GetFileName(saveMapData.FileName) + "]";
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void saveAsBitmapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveAsBitmap.ShowDialog() != DialogResult.OK)
				return;

			System.Drawing.Bitmap bmp = engine.GetBitmap();

			try
			{
				string ext = Path.GetExtension(saveAsBitmap.FileName).ToLower();
				switch (ext)
				{
					case ".gif":
						bmp.Save(saveAsBitmap.FileName, System.Drawing.Imaging.ImageFormat.Gif);
						break;
					case ".bmp":
						bmp.Save(saveAsBitmap.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
						break;
					case ".png":
						bmp.Save(saveAsBitmap.FileName, System.Drawing.Imaging.ImageFormat.Png);
						break;
					case ".jpg":
					case ".jpeg":
						bmp.Save(saveAsBitmap.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
						break;
					default:
						bmp.Save(saveAsBitmap.FileName);
						break;
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void drawGridToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawGridToolStripMenuItem.Checked = !drawGridToolStripMenuItem.Checked;
			mapControl1.DrawGrid = drawGridToolStripMenuItem.Checked;
		}

		private void zoomToExtendToolStripMenuItem_Click(object sender, EventArgs e)
		{
			vectorData.ZoomToExtend();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "MiniMap test application", "Test application", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void mapControl1_ScaleChanged(object sender, EventArgs e)
		{
			if (mapControl1.Units == MapUnits.Degrees)
			{
				scaleToolStripStatusLabel.Text = "-:-";
				return;
			}

			double scale = engine.Scale * 0.00026;
			string scaleText;

			if (scale < 1)
			{
				int a = (int)Math.Round(1.0 / scale);
				if (a == 0)
					a = 1;
				scaleText = "1 : " + a;
			}
			else
			{
				int a = (int)Math.Round(scale);
				if (a == 0)
					a = 1;
				scaleText = a + " : 1";
			}

			scaleToolStripStatusLabel.Text = scaleText;
		}

		private void Classify()
		{
			double xOrigin = 0.5 * (envelope.ptMin.X + envelope.ptMax.X);
			double yOrigin = 0.5 * (envelope.ptMin.Y + envelope.ptMax.Y);

			switch(methodUpDown1.Value)
			{
				case 1:
					DetermineSideByBinarySearch.СlassifyBySide(polygons.Cast<GeometryBase>().ToList(), lineStrings[0], xOrigin, yOrigin, out left, out right);
					labelCnt.Text = DetermineSideByBinarySearch.cnt.ToString();
					break;
				case 2:
					DetermineSideByBinarySearch2.СlassifyBySide(polygons.Cast<GeometryBase>().ToList(), lineStrings[0], xOrigin, yOrigin, out left, out right);
					labelCnt.Text = DetermineSideByBinarySearch2.cnt.ToString();
					break;
				case 3:
					DetermineSideByCombinedSearch.СlassifyBySide(polygons.Cast<GeometryBase>().ToList(), lineStrings[0], xOrigin, yOrigin, out left, out right);
					labelCnt.Text = "-";
					break;
			}

			Style style = new Style();

			style.Size = 1;
			style.Kind = 8;

			style.Color = MapEngine.RGB(0, 255, 0);
			vectorData.Draw(right, style);

			style.Color = MapEngine.RGB(0, 0, 255);
			vectorData.Draw(left, style);

			vectorData.DrawData();
		}

		private void btnClaasify_Click(object sender, EventArgs e)
		{
			vectorData.RemoveGeometry(left);
			vectorData.RemoveGeometry(right);
			Classify();
			btnRemove.Enabled = true;
		}

		private void btnReverse_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < lineStrings.Count; i++)
			{
				LineString line = new LineString();
				line.AddReverse(lineStrings[i]);
				lineStrings[i] = line;
			}
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			vectorData.RemoveGeometry(left);
			vectorData.RemoveGeometry(right);
			vectorData.DrawData();
		}

		#region Bench Mark
		const int n = 1000;
		private void btnBenc1_Click(object sender, EventArgs e)
		{
			double xOrigin = 0.5 * (envelope.ptMin.X + envelope.ptMax.X);
			double yOrigin = 0.5 * (envelope.ptMin.Y + envelope.ptMax.Y);

			lblTime1.Text = "";
			Application.DoEvents();

			Stopwatch st = new Stopwatch();
			st.Start();
			for (int i = 0; i < n; i++)
				DetermineSideByBinarySearch.СlassifyBySide(polygons.Cast<GeometryBase>().ToList(), lineStrings[0], xOrigin, yOrigin, out left, out right);
			//DetermineSideByLinearSearch.СlassifyBySide(polygons.Cast<GeometryBase>().ToList(), lineStrings[0], xOrigin, yOrigin, out left, out right);

			st.Stop();

			lblTime1.Text = string.Format("Elapsed time: {0}", st.ElapsedMilliseconds / (double)n);
		}

		private void btnBenc2_Click(object sender, EventArgs e)
		{
			double xOrigin = 0.5 * (envelope.ptMin.X + envelope.ptMax.X);
			double yOrigin = 0.5 * (envelope.ptMin.Y + envelope.ptMax.Y);

			lblTime2.Text = "";
			Application.DoEvents();

			Stopwatch st = new Stopwatch();
			st.Start();
			for (int i = 0; i < n; i++)
				DetermineSideByBinarySearch2.СlassifyBySide(polygons.Cast<GeometryBase>().ToList(), lineStrings[0], xOrigin, yOrigin, out left, out right);
			st.Stop();

			lblTime2.Text = string.Format("Elapsed time: {0}", st.ElapsedMilliseconds / (double)n);
		}

		private void btnBenc3_Click(object sender, EventArgs e)
		{
			double xOrigin = 0.5 * (envelope.ptMin.X + envelope.ptMax.X);
			double yOrigin = 0.5 * (envelope.ptMin.Y + envelope.ptMax.Y);

			lblTime3.Text = "";
			Application.DoEvents();

			Stopwatch st = new Stopwatch();
			st.Start();
			for (int i = 0; i < n; i++)
				DetermineSideByCombinedSearch.СlassifyBySide(polygons.Cast<GeometryBase>().ToList(), lineStrings[0], xOrigin, yOrigin, out left, out right);
			st.Stop();

			lblTime3.Text = string.Format("Elapsed time: {0}", st.ElapsedMilliseconds / (double)n);
		}

		#endregion
	}
}
