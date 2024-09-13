using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MiniMap.Geometry;

namespace DataFrame
{
	public static class GeoTxtSerializer
	{
		//Reader ====================================================================================
		#region Reader
		private enum Reading    //GeometryReaderState
		{
			point = 0,
			multipoint = 1,
			ring = 2,
			polyline = 3,
			polygon = 4,
			multipolyline = 5,
			multipolygon = 6,

			pointE = 7,
			multipointE = 8,
			ringE = 9,
			polylineE = 10,
			polygonE = 11,
			multipolylineE = 12,
			multipolygonE = 13,
			units = 14,
			file = 15,
			none = 16
		}

		private enum TokenType
		{
			Point = 0,
			MultiPoint = 1,
			Ring = 2,
			Polyline = 3,
			Polygon = 4,
			MultiPolyline = 5,
			MultiPolygon = 6,
			End = 7,
			Double = 8, //Numeric
			String = 9,

			X = 10,
			Y = 11,
			Z = 12,
			M = 13,
			Name = 14,

			File = 15,
			Units = 16,

			None = 17,
			Unknown = 18,
			eof = 19
		}

		private struct Token
		{
			public TokenType tokenId;
			public string text;
			public double value;
			public int line;
		}

		private static int lineNum;
		private const string msg1 = "Unerecognized word: ";
		private const string msg2 = "Unexpected keyword: ";
		private const string msg3 = "Missplaced keyword: ";
		private const string msg4 = "Missplaced definition: ";
		private const string msg10 = "Unexpected number: ";
		private const string msg14 = "Expected value for X, but found: ";
		private const string msg15 = "Expected value for Y, but found: ";
		private const string msg16 = "Expected value for Z, but found: ";
		private const string msg17 = "Expected value for M, but found: ";

		private readonly static string[] msgTable = new string[]
			{
				msg1, msg2, msg3, msg4, msg3, msg3,
				msg3, msg3, msg3, msg10, msg3, msg3,
				msg3, msg14, msg15, msg16, msg17, msg3
			};

		private readonly static char[] seperators = new char[] { ' ', '\t', ',', ':', '=' };

		private readonly static Dictionary<string, TokenType> keyWords = new Dictionary<string, TokenType>()
		{
			{ "point", TokenType.Point},
			{ "multipoint", TokenType.MultiPoint},
			{ "ring", TokenType.Ring},
			{ "polyline", TokenType.Polyline},
			{ "polygon", TokenType.Polygon},
			{ "multipolyline", TokenType.MultiPolyline},
			{ "multipolygon", TokenType.MultiPolygon},
			{ "end", TokenType.End},
			{ "x", TokenType.X},
			{ "y", TokenType.Y},
			{ "z", TokenType.Z},
			{ "m", TokenType.M},

			{ "name", TokenType.Name},
			{ "file", TokenType.File},
			{ "units", TokenType.Units}
		};

		private static void ShowError(int errnum, Token tok)
		{
			throw new Exception(string.Format("Error № {0} at line {1}\r\n{2}{3}", new object[] { errnum, tok.line, msgTable[errnum - 1], tok.text }));
		}

		public static IWin32Window Owner { get; set; }

		private readonly static Reading[] endTable =
		{
			Reading.pointE,
			Reading.multipointE,
			Reading.ringE,
			Reading.polylineE,
			Reading.polygonE,
			Reading.multipolylineE,
			Reading.multipolygonE
		};

		private static IEnumerable<Token> GetNextToken(StreamReader sr)
		{
			lineNum = 0;
			NumberFormatInfo numberFormat = CultureInfo.CreateSpecificCulture("en-US").NumberFormat;

			while (true)
			{
				if (sr.EndOfStream)
					break;

				string srcLine = sr.ReadLine();
				lineNum++;
				string[] srcWords = srcLine.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

				foreach (string word in srcWords)
				{
					if (word[0] == '#')
						break;

					string lWord = word.ToLower();
					Token token = new Token
					{
						text = word,
						line = lineNum,
						tokenId = TokenType.None
					};

					if (keyWords.TryGetValue(lWord, out TokenType tokenType))
						token.tokenId = tokenType;
					else if (word[0] == '\"')
						token.tokenId = TokenType.String;
					else if (double.TryParse(lWord, NumberStyles.Any, numberFormat, out token.value))
						token.tokenId = TokenType.Double;
					else if (int.TryParse(lWord, NumberStyles.Any, numberFormat, out int intValue))
					{
						token.value = intValue;
						token.tokenId = TokenType.Double;
					}

					yield return token;
				}
			}
		}

		public static void LoadFromFile(this MapData mapData, string FileName)
		{
			if (!File.Exists(FileName))
				throw new Exception("File not found!");

			mapData.Clear();
			mapData.ClearViewExtendStack();

			mapData.Name = "";
			mapData.Units = MiniMap.MapUnits.None;

			Stack<Reading> readerState = new Stack<Reading>();
			Reading currState = Reading.none;

			GeometryBase currGeom = null;
			MultiPolygon mltpolygon = null;
			Polygon polygon = null;
			Ring ring = null;
			MultiLineString mltLinestr = null;
			LineString lineString = null;
			MultiPoint mlpt = null;
			Point pt = null;
#if haveProperties
			Property property = new Property();
			bool readProperty = false;
#endif
			bool readX = false;
			bool readY = false;
			bool readZ = false;
			bool readM = false;
			bool readName = false;

			using (StreamReader sr = new StreamReader(FileName))
				while (!sr.EndOfStream)
				{
					foreach (var token in GetNextToken(sr))
					{
						if (token.tokenId != TokenType.Double)
						{
							if (readX)
								ShowError(14, token);
							else if (readY)
								ShowError(15, token);
							else if (readZ)
								ShowError(16, token);
							else if (readM)
								ShowError(17, token);
						}

						switch (token.tokenId)
						{
							case TokenType.None:
								if (currState == Reading.file)
								{
									mapData.Name = token.text;
									currState = readerState.Pop();
								}
								else if (currState == Reading.units)
								{
									string units = token.text.ToLower();
									if (units == "meters")
										mapData.Units = MiniMap.MapUnits.Meters;
									else if (units == "degrees")
										mapData.Units = MiniMap.MapUnits.Degrees;
									else
										mapData.Units = MiniMap.MapUnits.None;

									currState = readerState.Pop();
								}
								else if (readName)
								{
									readName = false;
									currGeom.Name = token.text;
								}
								else
								{
									if (currGeom == null)
										ShowError(1, token);
#if haveProperties
									property.Name = token.text;
									readProperty = true;
#else
									//ShowError(1, token);
#endif
								}

								continue;

							case TokenType.String:
								if (readName)
								{
									readName = false;
									string name = token.text.Trim('\"');

									if (currGeom.Name != null)
										currGeom.Name += "/" + name;
									else
										currGeom.Name = name;
								}
#if haveProperties
								else if (readProperty)
								{
									string strTmp = token.text.Trim('\"');

									property.Value = strTmp;
									readProperty = false;
									currGeom.Propertyes.Add(property);
									property = new Property();
								}
#endif
								else
									ShowError(1, token);

								continue;

							case TokenType.File:
								if (currState != Reading.none)
									ShowError(2, token);

								readerState.Push(currState);
								currState = Reading.file;
								continue;

							case TokenType.Units:
								if (currState != Reading.none)
									ShowError(2, token);

								readerState.Push(currState);
								currState = Reading.units;
								continue;

							case TokenType.MultiPolygon:
								if (currState != Reading.none)
								{
									if (currState == Reading.multipolygonE)
									{
										currState = readerState.Pop();
										mapData.AddGeometry(mltpolygon);
										mltpolygon = null;
										currGeom = null;
									}
									else
										ShowError(4, token);
								}
								else
								{
									readerState.Push(currState);
									currState = Reading.multipolygon;
									mltpolygon = new MultiPolygon();
									currGeom = mltpolygon;
								}

								continue;

							case TokenType.Polygon:
								if (currState == Reading.none || currState == Reading.multipolygon)
								{
									readerState.Push(currState);
									polygon = new Polygon();

									if (currState == Reading.none)
										currGeom = polygon;

									currState = Reading.polygon;
									continue;
								}

								if (currState == Reading.polygonE)
								{
									currState = readerState.Pop();
									if (currState == Reading.multipolygon)
										mltpolygon.Add(polygon);
									else
									{
										mapData.AddGeometry(polygon);
										currGeom = null;
									}
									polygon = null;
								}
								else
									ShowError(5, token);

								continue;

							case TokenType.Ring:
								if (currState == Reading.polygon)
								{
									readerState.Push(currState);
									currState = Reading.ring;
									ring = new Ring();
								}
								else if (currState == Reading.ringE)
								{
									currState = readerState.Pop();

									if (polygon.ExteriorRing == null)
										polygon.ExteriorRing = ring;
									else
										polygon.InteriorRingList.Add(ring);

									ring = null;
								}
								else
									ShowError(6, token);

								continue;

							case TokenType.MultiPolyline:
								if (currState == Reading.none)
								{
									readerState.Push(currState);
									mltLinestr = new MultiLineString();
									currGeom = mltLinestr;

									currState = Reading.multipolyline;
									continue;
								}

								if (currState == Reading.multipolylineE)
								{
									mapData.AddGeometry(mltLinestr);
									currGeom = null;
									mltLinestr = null;
									currState = readerState.Pop();
									continue;
								}

								ShowError(5, token);
								continue;

							case TokenType.Polyline:
								if (currState == Reading.none || currState == Reading.multipolyline)
								{
									readerState.Push(currState);
									lineString = new LineString();
									if (currState == Reading.none)
										currGeom = lineString;

									currState = Reading.polyline;

									continue;
								}

								if (currState == Reading.polylineE)
								{
									currState = readerState.Pop();

									if (currState == Reading.multipolyline)
										mltLinestr.Add(lineString);
									else
									{
										mapData.AddGeometry(lineString);
										currGeom = null;
									}

									lineString = null;
								}
								else
									ShowError(6, token);

								continue;
							case TokenType.MultiPoint:
								if (currState == Reading.none)
								{
									readerState.Push(currState);
									mlpt = new MultiPoint();
									currGeom = mlpt;

									currState = Reading.multipoint;
									continue;
								}

								if (currState == Reading.multipointE)
								{
									mapData.AddGeometry(mlpt);
									currGeom = null;
									mlpt = null;
									currState = readerState.Pop();
									continue;
								}

								ShowError(5, token);
								continue;
							case TokenType.Point:
								if (currState == Reading.none || currState == Reading.ring || currState == Reading.polyline || currState == Reading.multipoint)
								{
									readerState.Push(currState);
									pt = new Point();
									if (currState == Reading.none)
										currGeom = pt;
									currState = Reading.point;

									continue;
								}

								if (currState == Reading.pointE)
								{
									currState = readerState.Pop();

									if (currState == Reading.ring)
										ring.Add(pt);
									else if (currState == Reading.polyline)
										lineString.Add(pt);
									else if (currState == Reading.multipoint)
										mlpt.Add(pt);
									else
									{
										mapData.AddGeometry(pt);
										currGeom = null;
									}

									pt = null;
								}
								else
									ShowError(7, token);

								continue;

							case TokenType.X:
								if (currState != Reading.point)
									ShowError(8, token);

								if (readX)
									ShowError(12, token);
								else
									readX = true;

								continue;

							case TokenType.Y:
								if (currState != Reading.point)
									ShowError(9, token);

								if (readY)
									ShowError(13, token);
								else
									readY = true;

								continue;

							case TokenType.Z:
								if (currState != Reading.point)
									ShowError(9, token);

								if (readZ)
									ShowError(13, token);
								else
									readZ = true;

								continue;

							case TokenType.M:
								if (currState != Reading.point)
									ShowError(9, token);

								if (readM)
									ShowError(13, token);
								else
									readM = true;

								continue;

							case TokenType.Name:
								if (currGeom == null)
									ShowError(9, token);

								readName = true;
								continue;

							case TokenType.Double:
								if (readX)
								{
									readX = false;
									pt.X = token.value;
									continue;
								}

								if (readY)
								{
									readY = false;
									pt.Y = token.value;
									continue;
								}

								if (readZ)
								{
									readZ = false;
									pt.Z = token.value;
									continue;
								}

								if (readM)
								{
									readM = false;
									pt.M = token.value;
									continue;
								}

								ShowError(10, token);
								continue;

							case TokenType.End:
								if (currState > Reading.multipolygon)
									ShowError(3, token);

								currState = endTable[(int)currState];
								continue;

							default:
								ShowError(11, token);
								continue;
						}
					}
				}
		}
		#endregion

		//Writer ====================================================================================
		#region Writer
#if haveProperties
		static void saveProperties(StreamWriter writer, GeometryBase geometry)
		{
			if (geometry.Propertyes.Count > 0)
			{
				int np = geometry.Propertyes.Count;
				for (int j = 0; j < np; j++)
				{
					var propperty = geometry.Propertyes[j];
					//writer.Write("\t\"{0}\": ", propperty.Name);
					writer.Write("\t{0}: ", propperty.Name);
					writer.WriteLine("\"{0}\"{1}", propperty.Value.ToString(), j < np - 1 ? "," : "");
					//if(propperty.value is IList<> )
				}
			}
		}
#endif

		//static void SavePoint(StreamWriter writer, Point point, bool saveName = true)
		//{
		//	writer.Write(string.Format("Point "));
		//	if (saveName && !string.IsNullOrEmpty(point.Name))
		//		writer.Write(string.Format("Name: \"{0}\", ", point.Name));

		//	writer.Write(string.Format("X: {0}, Y: {1}", point.X, point.Y));

		//	if (!double.IsNaN(point.Z))
		//		writer.Write(string.Format(", Z: {0}", point.Z));

		//	if (!double.IsNaN(point.M))
		//		writer.Write(string.Format(", M: {0}", point.M));

		//	writer.WriteLine(" End Point");
		//}

		static void SavePoint(StreamWriter writer, Point point, bool saveProps = true)
		{
			writer.Write(string.Format("Point X: {0}, Y: {1}", point.X, point.Y));

			if (!double.IsNaN(point.Z))
				writer.Write(string.Format(", Z: {0}", point.Z));

			if (!double.IsNaN(point.M))
				writer.Write(string.Format(", M: {0}", point.M));

			if (saveProps && !string.IsNullOrEmpty(point.Name))
				writer.Write(string.Format(", Name: \"{0}\"", point.Name));

#if haveProperties
			if (saveProps)
				saveProperties(writer, point);
#endif
			writer.WriteLine(" End Point");
		}

		static void SaveMultiPoint(StreamWriter writer, MultiPoint points)
		{
			writer.WriteLine("MultiPoint");
			if (!string.IsNullOrEmpty(points.Name))
				writer.WriteLine(string.Format("Name: \"{0}\"", points.Name));

			foreach (var pt in points)
				SavePoint(writer, pt, false);

#if haveProperties
			saveProperties(writer, points);
#endif

			writer.WriteLine("End MultiPoint");
		}

		static void SaveRing(StreamWriter writer, Ring ring)//, bool saveProps = false
		{
			writer.WriteLine("Ring");
			//if (saveProps && !string.IsNullOrEmpty(ring.Name))
			//	writer.WriteLine(string.Format("Name: \"{0}\"", ring.Name));

			foreach (var pt in ring)
				SavePoint(writer, pt, false);

			//#if haveProperties
			//			if (saveProps)
			//				saveProperties(writer, ring);
			//#endif
			writer.WriteLine("End Ring");
		}

		static void SavePolygon(StreamWriter writer, Polygon polygon, bool saveProps = true)
		{
			writer.WriteLine("Polygon");

			if (saveProps && !string.IsNullOrEmpty(polygon.Name))
				writer.WriteLine(string.Format("Name: \"{0}\"", polygon.Name));

			SaveRing(writer, polygon.ExteriorRing); //, false

			foreach (var ring in polygon.InteriorRingList)
				SaveRing(writer, ring);             //, false

#if haveProperties
			if (saveProps)
				saveProperties(writer, polygon);
#endif
			writer.WriteLine("End Polygon");
		}

		static void SaveMultiPolygon(StreamWriter writer, MultiPolygon mltPolygon)
		{
			writer.WriteLine("MultiPolygon");

			if (!string.IsNullOrEmpty(mltPolygon.Name))
				writer.WriteLine(string.Format("Name: \"{0}\"", mltPolygon.Name));

			foreach (Polygon polygon in mltPolygon)
				SavePolygon(writer, polygon, false);

#if haveProperties
			saveProperties(writer, mltPolygon);
#endif
			writer.WriteLine("End MultiPolygon");
		}

		static void SaveLineString(StreamWriter writer, LineString lineString, bool saveProps = true)
		{
			writer.WriteLine("Polyline");

			if (saveProps && !string.IsNullOrEmpty(lineString.Name))
				writer.WriteLine(string.Format("Name: \"{0}\"", lineString.Name));

			foreach (var pt in lineString)
				SavePoint(writer, pt, false);

#if haveProperties
			if (saveProps)
				saveProperties(writer, lineString);
#endif
			writer.WriteLine("End Polyline");
		}

		static void SaveMultiLineString(StreamWriter writer, MultiLineString mltLineString)
		{
			writer.WriteLine("MultiPolyline");
			if (!string.IsNullOrEmpty(mltLineString.Name))
				writer.WriteLine(string.Format("Name: \"{0}\"", mltLineString.Name));

			foreach (LineString lineString in mltLineString)
				SaveLineString(writer, lineString, false);

#if haveProperties
			saveProperties(writer, mltLineString);
#endif

			writer.WriteLine("End MultiPolyline");
		}

		public static void SaveToTxtFile(this MapData mapData, string FileName)
		{
			using (StreamWriter writer = new StreamWriter(FileName))
			{
				if (!string.IsNullOrEmpty(mapData.Name))
					writer.WriteLine("File " + mapData.Name);

				writer.WriteLine("Units " + mapData.Units.ToString());
				writer.WriteLine();

				foreach (GeometryBase item in mapData)
				{
					switch (item.Type)
					{
						case GeometryType.Point:
							SavePoint(writer, (Point)item);
							break;

						case GeometryType.MultiPoint:
							SaveMultiPoint(writer, (MultiPoint)item);
							break;

						case GeometryType.LineString:
							SaveLineString(writer, ((LineString)item));
							break;

						//case GeometryType.Ring:
						//	SaveRing(writer, (Ring)item, true);
						//	break;

						case GeometryType.Polygon:
							SavePolygon(writer, (Polygon)item);
							break;

						case GeometryType.MultiLineString:
							SaveMultiLineString(writer, (MultiLineString)item);
							break;

						case GeometryType.MultiPolygon:
							SaveMultiPolygon(writer, (MultiPolygon)item);
							break;
					}

					writer.WriteLine();
				}
			}
		}
		#endregion
	}
}
