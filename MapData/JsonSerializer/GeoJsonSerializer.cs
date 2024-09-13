using System;
using System.Collections.Generic;
using System.IO;
using MiniMap.Geometry;

namespace DataFrame
{
	internal static class GeoJsonSerializer
	{
		//Reader ====================================================================================
		#region Reader
		private static int order = 0;

		private static Point readPoint(StructField grometryData)
		{
			Point result = new Point();

			List<StructField> values = (List<StructField>)grometryData.Value;
			for (int i = 0; i < values.Count; i++)
			{
				switch (i)
				{
					case 0:
						result.X = (double)values[i + order].Value;
						break;
					case 1:
						result.Y = (double)values[i - order].Value;
						break;
					case 2:
						result.Z = (double)values[i].Value;
						break;
					case 3:
						result.M = (double)values[i].Value;
						break;
				}
			}

			return result;
		}

		private static MultiPoint readMultiPoint(StructField grometryData)
		{
			MultiPoint result = new MultiPoint();

			List<StructField> coordinates = (List<StructField>)grometryData.Value;
			foreach (StructField coordinate in coordinates)
				result.Add(readPoint(coordinate));

			return result;
		}

		private static LineString readLineString(StructField grometryData)
		{
			LineString result = new LineString();

			List<StructField> coordinates = (List<StructField>)grometryData.Value;
			foreach (StructField coordinate in coordinates)
				result.Add(readPoint(coordinate));

			return result;
		}

		private static GeometryBase readMultiLineString(StructField grometryData)
		{
			MultiLineString result = new MultiLineString();
			List<StructField> lineStrings = (List<StructField>)grometryData.Value;
			foreach (StructField lineString in lineStrings)
				result.Add(readLineString(lineString));
			return result;
		}

		private static Polygon readPolygon(StructField grometryData)
		{
			Polygon result = new Polygon();

			List<StructField> rings = (List<StructField>)grometryData.Value;

			foreach (var jsRing in rings)
			{
				Ring ring = new Ring();
				List<StructField> coordinates = (List<StructField>)jsRing.Value;

				foreach (var coordinate in coordinates)
					ring.Add(readPoint(coordinate));

				if (result.ExteriorRing == null)
					result.ExteriorRing = ring;
				else
					result.InteriorRingList.Add(ring);
			}

			return result;
		}

		private static GeometryBase readMultiPolygon(StructField grometryData)
		{
			MultiPolygon result = new MultiPolygon();

			List<StructField> polygons = (List<StructField>)grometryData.Value;
			foreach (var jsPolygon in polygons)
				result.Add(readPolygon(jsPolygon));

			return result;
		}

		private static void readGeometryCollection(StructField geometryData, StructField id, List<StructField> properties, MapData dataframe)
		{
			GeometryBase currGeom = null;
			int i = 0;
			foreach (StructField geomData in (List<StructField>)geometryData.Value)
			{
				currGeom = null;
				JsonTokenType geometryType = JsonTokenType.None;
				StructField coordinates = null;

				foreach (StructField fld in (List<StructField>)geomData.Value)
				{
					if (fld.FieldId == JsonTokenType.type)
						geometryType = (JsonTokenType)fld.Value;
					else if (fld.FieldId == JsonTokenType.coordinates)
						coordinates = fld;
				}

				if (geometryType != JsonTokenType.None && coordinates != null)
				{
					switch (geometryType)
					{
						case JsonTokenType.point:
							currGeom = readPoint(coordinates);
							break;
						case JsonTokenType.multipoint:
							currGeom = readMultiPoint(coordinates);
							break;

						case JsonTokenType.linestring:
							currGeom = readLineString(coordinates);
							break;

						case JsonTokenType.multilinestring:
							currGeom = readMultiLineString(coordinates);
							break;

						case JsonTokenType.polygon:
							currGeom = readPolygon(coordinates);
							break;

						case JsonTokenType.multipolygon:
							currGeom = readMultiPolygon(coordinates);
							break;

						//case JsonTokenType.geometrycollection:
						//	readGeometryCollection(grometry, id, properties, dataframe);
						//	break;
						default:
							currGeom = null;
							break;
					}

					if (currGeom != null && id != null)
						currGeom.Name = string.Format("{0}_{1}", id.Value, ++i);
				}

				if (currGeom != null)
				{
#if haveProperties
					if (properties != null)
					{
						foreach (StructField jsProperty in properties)
						{
							Property property = new Property()
							{
								Name = jsProperty.Name,
								Value = jsProperty.Value
							};

							currGeom.Propertyes.Add(property);
						}
					}
#endif
					dataframe.AddGeometry(currGeom);
				}
			}
		}

		public static void LoadGeoJson(this MapData mapData, string FileName)
		{
			if (!File.Exists(FileName))
				throw new Exception("File not found!");

			mapData.ClearViewExtendStack();
			mapData.Clear();
			mapData.Name = Path.ChangeExtension(Path.GetFileName(FileName), null);
			mapData.Units = MiniMap.MapUnits.None;

			GeometryBase currGeom = null;

			//List<StructField> RootStruct = 
			JsonParser.LoadJson(FileName);
			List<List<StructField>> FeatureCollections = JsonParser.FeatureCollections;

			for (int i = 0; i < FeatureCollections.Count; i++)
			{
				foreach (StructField featureCollection in FeatureCollections[i])
				{
					if (featureCollection.FieldId == JsonTokenType.crs)
					{
						//"EPSG:3857" - in meters, "EPSG:4326" - in degrees
						//"OGC:CRS84" - in degrees in reverse order
						//"urn:ogc:def:crs:OGC:1.3:CRS84" - in degrees

						order = 0;
						StructField crsProperties = ((List<StructField>)featureCollection.Value).Find(delegate (StructField field)
						{
							return field.FieldId == JsonTokenType.properties;
						});

						if (crsProperties != null)
						{
							try
							{
								string crsName = ((List<StructField>)crsProperties.Value)[0].Value.ToString();
								if (crsName == "EPSG:3857" || crsName == "urn:ogc:def:crs:EPSG:3857")
									mapData.Units = MiniMap.MapUnits.Meters;
								else if (crsName == "EPSG:4326" || crsName == "urn:ogc:def:crs:EPSG:4326")
									mapData.Units = MiniMap.MapUnits.Degrees;
								else if (crsName == "OGC:CRS84" || crsName == "urn:ogc:def:crs:OGC:1.3:CRS84")
								{
									order = 1;
									mapData.Units = MiniMap.MapUnits.Degrees;
								}
							}
							catch { }
						}
					}

					if (featureCollection.FieldId == JsonTokenType.features)
					{
						List<StructField> features = (List<StructField>)featureCollection.Value;
						foreach (StructField feature in features)
						{
							List<StructField> fields = (List<StructField>)feature.Value;

							JsonTokenType geometryType = JsonTokenType.None;
							StructField id = null;
							StructField geometryData = null;
							List<StructField> properties = null;
							currGeom = null;

							foreach (StructField field in fields)
							{
								if (field.FieldId == JsonTokenType.geometry)
								{
									List<StructField> geometry = (List<StructField>)field.Value;
									foreach (var fld in geometry)
									{
										if (fld.FieldId == JsonTokenType.type)
											geometryType = (JsonTokenType)fld.Value;
										else if (fld.FieldId == JsonTokenType.coordinates || fld.FieldId == JsonTokenType.geometries)
											geometryData = fld;
									}
								}
								else if (field.FieldId == JsonTokenType.id)
									id = field;
#if haveProperties
								else if (field.FieldId == JsonTokenType.properties)
									properties = (List<StructField>)field.Value;
#endif
							}

							currGeom = null;

							if (geometryType != JsonTokenType.None && (geometryData != null || geometryType == JsonTokenType.geometrycollection))
							{
								switch (geometryType)
								{
									case JsonTokenType.point:
										currGeom = readPoint(geometryData);
										break;
									case JsonTokenType.multipoint:
										currGeom = readMultiPoint(geometryData);
										break;

									case JsonTokenType.linestring:
										currGeom = readLineString(geometryData);
										break;

									case JsonTokenType.multilinestring:
										currGeom = readMultiLineString(geometryData);
										break;

									case JsonTokenType.polygon:
										currGeom = readPolygon(geometryData);
										break;

									case JsonTokenType.multipolygon:
										currGeom = readMultiPolygon(geometryData);
										break;

									case JsonTokenType.geometrycollection:
										readGeometryCollection(geometryData, id, properties, mapData);
										break;
									default:
										currGeom = null;
										break;
								}
							}

							if (currGeom != null)
							{
								if (id != null)
									currGeom.Name = id.Value.ToString();

#if haveProperties
								if (properties != null)
								{
									foreach (StructField jsProperty in properties)
									{
										Property property = new Property()
										{
											Name = jsProperty.Name,
											Value = jsProperty.Value
										};

										currGeom.Propertyes.Add(property);
									}
								}
#endif
								mapData.AddGeometry(currGeom);
							}
						}
					}
				}
			}

			return;
		}
		#endregion

		//Writer ====================================================================================
		#region Writer
		static void SavePointCoordinates(StreamWriter writer, Point pt)
		{
			writer.Write(string.Format("[ {0}, {1}", pt.X, pt.Y));
			if (!double.IsNaN(pt.Z))
				writer.Write(string.Format(", {0}", pt.Z));

			if (!double.IsNaN(pt.M))
				writer.Write(string.Format(", {0}", pt.M));

			writer.Write(" ]");
		}

		static void SaveMultiPointCoordinates(StreamWriter writer, MultiPoint mlpt, int tabs = 4)
		{
			string indent = new string('\t', tabs);
			writer.WriteLine("[");
			int n = mlpt.Count;
			for (int i = 0; i < n; i++)
			{
				writer.Write(indent + "\t");
				SavePointCoordinates(writer, mlpt[i]);
				writer.WriteLine(i < n - 1 ? "," : "");
			}

			writer.Write(indent + "]");
		}

		static void SavePolygonCoordinates(StreamWriter writer, Polygon polygon, int tabs = 0)
		{
			string indent = new string('\t', 4 + tabs);
			if (tabs > 0)
				writer.Write(indent);

			writer.WriteLine("[");
			writer.Write(indent + "\t");

			SaveMultiPointCoordinates(writer, polygon.ExteriorRing, 5 + tabs);
			int n = polygon.InteriorRingList.Count;

			if (n > 0)
			{
				writer.WriteLine(",");
				writer.Write(indent + '\t');

				for (int i = 0; i < n; i++)
				{
					SaveMultiPointCoordinates(writer, polygon.InteriorRingList[i], 5 + tabs);
					writer.WriteLine(i < n - 1 ? "," : "");
				}
				writer.Write(indent + "]");
			}
			else
			{
				writer.WriteLine();
				writer.Write(indent);
				writer.Write("]");
			}
		}

		static void SaveMultiLineStringCoordinates(StreamWriter writer, MultiLineString lineStrings)
		{
			string indent = new string('\t', 4);

			writer.WriteLine("[");
			writer.Write(indent + "\t");

			int n = lineStrings.Count;

			for (int i = 0; i < n; i++)
			{
				SaveMultiPointCoordinates(writer, lineStrings[i], 5);
				writer.WriteLine(i < n - 1 ? "," : "");
			}

			writer.WriteLine(indent + "]");
		}

		static void SaveMultiPolygonCoordinates(StreamWriter writer, MultiPolygon polygons)
		{
			string indent = new string('\t', 4);
			writer.WriteLine("[");
			//writer.Write(indent + "\t");

			int n = polygons.Count;

			for (int i = 0; i < n; i++)
			{
				SavePolygonCoordinates(writer, polygons[i], 1);
				writer.WriteLine(i < n - 1 ? "," : "");
			}

			writer.WriteLine(indent + "]");
		}

		public static void SaveGeometryCoordinates(StreamWriter writer, GeometryBase geometry)
		{
			writer.WriteLine(",");
			writer.Write("\t\t\t\t\"coordinates\": ");

			switch (geometry.Type)
			{
				case GeometryType.Point:
					SavePointCoordinates(writer, (Point)geometry);
					writer.WriteLine();
					break;

				case GeometryType.MultiPoint:
				case GeometryType.LineString:
				case GeometryType.Ring:
					SaveMultiPointCoordinates(writer, (MultiPoint)geometry);
					writer.WriteLine();
					break;

				case GeometryType.Polygon:
					SavePolygonCoordinates(writer, (Polygon)geometry);
					writer.WriteLine();
					break;

				case GeometryType.MultiLineString:
					SaveMultiLineStringCoordinates(writer, (MultiLineString)geometry);
					break;

				case GeometryType.MultiPolygon:
					SaveMultiPolygonCoordinates(writer, (MultiPolygon)geometry);
					break;
			}
		}

		static void SaveCRS(MapData dataframe, StreamWriter writer)
		{
			if (dataframe.Units == MiniMap.MapUnits.None)
				return;

			writer.WriteLine(",\r\n\t\"crs\": {");
			writer.WriteLine("\t\t\"type\": \"name\",");
			writer.WriteLine("\t\t\"properties\": {");
			writer.Write("\t\t\t\"name\": ");

			switch (dataframe.Units)
			{
				case MiniMap.MapUnits.Meters:
					writer.WriteLine("\"EPSG:3857\"");
					break;
				case MiniMap.MapUnits.Degrees:
					writer.WriteLine("\"EPSG:4326\"");
					break;
			}

			writer.WriteLine("\t\t}");
			writer.Write("\t}");
		}


		public static void SaveToGeoJsonFile(this MapData mapData, string FileName)
		{
			using (StreamWriter writer = new StreamWriter(FileName))
			{
				writer.WriteLine("{");
				writer.Write("\t\"type\": \"FeatureCollection\"");
				SaveCRS(mapData, writer);

				writer.WriteLine(",\r\n\t\"features\": [");

				int n = mapData.GeometryCount;
				for (int i = 0; i < n; i++)
				{
					GeometryBase item = mapData[i];
					writer.WriteLine("\t\t{");
					writer.WriteLine("\t\t\t\"type\": \"Feature\",");

					if (!string.IsNullOrEmpty(item.Name))
						writer.WriteLine("\t\t\t\"id\": \"{0}\",", item.Name);

					writer.WriteLine("\t\t\t\"geometry\": {");
					writer.Write("\t\t\t\t\"type\": ");

					Polygon polygon = null;
					switch (item.Type)
					{
						case GeometryType.Point:
							writer.Write("\"Point\"");
							break;

						case GeometryType.MultiPoint:
							writer.Write("\"MultiPoint\"");
							break;

						case GeometryType.LineString:
							writer.Write("\"LineString\"");
							break;

						case GeometryType.Ring:
							polygon = new Polygon { ExteriorRing = (Ring)item };

							writer.Write("\"Polygon\"");
							break;

						case GeometryType.Polygon:
							writer.Write("\"Polygon\"");
							break;

						case GeometryType.MultiLineString:
							writer.Write("\"MultiLineString\"");
							break;

						case GeometryType.MultiPolygon:
							writer.Write("\"MultiPolygon\"");
							break;
					}

					if (item.Type == GeometryType.Ring)
						SaveGeometryCoordinates(writer, polygon);
					else
						SaveGeometryCoordinates(writer, item);

					writer.Write("\t\t\t}");
#if haveProperties
					if (item.Propertyes.Count > 0)
					{
						writer.WriteLine(",");
						writer.WriteLine("\t\t\t\"properties\": {");
						int np = item.Propertyes.Count;
						for (int j = 0; j < np; j++)
						{
							var propperty = item.Propertyes[j];

							writer.Write("\t\t\t\t\"{0}\": ", propperty.Name);
							writer.WriteLine("\"{0}\"{1}", propperty.Value.ToString(), j < np - 1 ? "," : "");
							//if(propperty.value is IList<> )
						}
						writer.Write("\t\t\t}");
					}
#endif
					writer.WriteLine();

					if (i < n - 1)
						writer.WriteLine("\t\t},");
					else
						writer.WriteLine("\t\t}");
				}
				writer.WriteLine("\t]\r\n}");
			}
		}
		#endregion
	}
}