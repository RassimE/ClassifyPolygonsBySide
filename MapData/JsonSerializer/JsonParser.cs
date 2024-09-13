using System;
using System.Collections.Generic;
using System.IO;

namespace DataFrame
{
	internal static class JsonParser
	{
		public static List<List<StructField>> FeatureCollections { get; private set; }
		private static List<JsonToken> tokens;
		private static int i, n;

		static JsonToken GetNextToken()
		{
			if (i < n)
				return tokens[i++];

			return null;
		}

		static StructField readValue(StructField field)
		{
			var token = GetNextToken();

			switch (token.TokenId)
			{
				case JsonTokenType.Boolean:
					if (field.FieldId == JsonTokenType.String)
						field.FieldId = JsonTokenType.Boolean;

					field.Value = token.bValue;
					return field;

				case JsonTokenType.Double:
					if (field.FieldId == JsonTokenType.String)
						field.FieldId = JsonTokenType.Double;

					field.Value = token.dValue;
					return field;

				case JsonTokenType.String:
					field.Value = token.Text;
					return field;

				case JsonTokenType.Record:
					if (field.FieldId == JsonTokenType.String)
						field.FieldId = JsonTokenType.Record;

					field.Value = readRecord(new List<StructField>());
					return field;

				case JsonTokenType.Array:
					if (field.FieldId == JsonTokenType.String)
						field.FieldId = JsonTokenType.Array;

					field.Value = readArray();
					return field;

				default:
					if (token.TokenId > JsonTokenType.None)
					{
						field.Value = token.TokenId;
						return field;
					}

					string errorString = string.Format("Unexpected \"{0}\" at line {1} position {2}.", token.Text, token.Row, token.Col);
					throw new Exception(errorString);
			}
		}

		static List<StructField> readRecord(List<StructField> root)
		{
			JsonToken token;
			bool bAdded = false;
			do
			{
				token = GetNextToken();

				//Empty struct
				if (token.TokenId == JsonTokenType.EndRecord)
					break;

				StructField field = new StructField
				{
					Name = token.Text,
					FieldId = token.TokenId
				};

				token = GetNextToken();
				if (token == null || token.TokenId != JsonTokenType.colon)
				{
					string errorString = string.Format("Expected \":\"  at line {0} position {1}.", token.Row, token.Col);
					throw new Exception(errorString);
				}

				//readValue(field);

				root.Add(readValue(field));

				if (!bAdded && field.FieldId == JsonTokenType.type && field.Value is JsonTokenType type && type == JsonTokenType.featurecollection)
				{
					FeatureCollections.Add(root);
					bAdded = true;
				}

				token = GetNextToken();
			} while (token != null && token.TokenId == JsonTokenType.comma);

			if (token == null || token.TokenId != JsonTokenType.EndRecord)
			{
				string errorString = string.Format("Expected \"}\"  at line {0} position {1}.", token.Row, token.Col);
				throw new Exception(errorString);
			}

			return root;
		}

		static List<StructField> readArray()
		{
			List<StructField> result = new List<StructField>();
			JsonToken token;

			do
			{
				token = GetNextToken();
				StructField structField = new StructField
				{
					Value = token.Text,
					Name = token.Text,
					FieldId = token.TokenId
				};

				//Empty array
				if (token.TokenId == JsonTokenType.EndArray)
					return result;

				switch (token.TokenId)
				{
					case JsonTokenType.Double:
						structField.Value = token.dValue;
						break;

					case JsonTokenType.String:
					case JsonTokenType.comma:
						break;

					case JsonTokenType.Record:
						structField.Value = readRecord(new List<StructField>());
						break;

					case JsonTokenType.Array:
						structField.Value = readArray();
						break;

					default:
						{
							string errorString = string.Format("Unexpected \"{0}\" at line {1} position {2}.", token.Text, token.Row, token.Col);
							throw new Exception(errorString);
						}
				}

				result.Add(structField);
				token = GetNextToken();
			}
			while (token != null && token.TokenId == JsonTokenType.comma);

			if (token == null || token.TokenId != JsonTokenType.EndArray)
			{
				string errorString = string.Format("Expected \"]\"  at line {0} position {1}.", token.Row, token.Col);
				throw new Exception(errorString);
			}

			return result;
		}

		public static List<StructField> LoadJson(string FileName)
		{
			using (StreamReader sr = new StreamReader(FileName))
				tokens = JsonLexer.GetTokens(sr);

			FeatureCollections = new List<List<StructField>>();
			i = 0;
			n = tokens.Count;

			var token = GetNextToken();
			if (token.TokenId != JsonTokenType.Record)
			{
				string errorString = string.Format("Unexpected \"{0}\" at line {1} position {2}.", token.Text, token.Row, token.Col);
				throw new Exception(errorString);
			}

			List<StructField> RootStruct = new List<StructField>();
			readRecord(RootStruct);

			tokens.Clear();
			tokens = null;

			return RootStruct;
		}
	}
}
