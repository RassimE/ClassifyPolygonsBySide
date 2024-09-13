namespace DataFrame
{
	enum JsonTokenType
	{
		None,
		bbox,

		point,
		multipoint,
		linestring,
		//== ring ==
		polygon,
		multilinestring,
		multipolygon,

		type,
		crs,
		featurecollection,
		geometrycollection,
		feature,
		id,
		features,
		geometry,
		geometries,
		coordinates,
		properties,
		//===========================
		leftPar = '(',
		rightPar = ')',
		Array = '[',    //StartArray
		EndArray = ']',
		Record = '{',   //StartStruct
		EndRecord = '}',

		comma = ',',
		colon = ':',

		Boolean,
		Double,
		Null,
		String,
		Unknown
	}

	class StructField
	{
		public JsonTokenType FieldId;
		public string Name;
		public object Value;

		public override string ToString()
		{
			string name = FieldId > JsonTokenType.None ? FieldId.ToString() : Name;
			return string.Format("{0} = {1}", name, Value);
		}
	}

	class JsonToken
	{
		public JsonTokenType TokenId;
		public string Text;
		public double dValue;
		public bool bValue;

		public int Row;
		public int Col;

		//public int i, startColon, startRow;
		//public int endColon, endRow;

		public override string ToString()
		{
			string text = Text != null ? ": '" + Text + "'" : "";
			string value = TokenId == JsonTokenType.Double ? ": " + dValue.ToString() : "";

			return string.Format("{0}{1}{2}", TokenId, text, value);
		}
	}
}
