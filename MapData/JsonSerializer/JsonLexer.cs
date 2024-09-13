using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace DataFrame
{
	static class JsonLexer
	{
		static char getCodedChar(StreamReader sr)
		{
			char chr = (char)sr.Read();
			int res = 0;

			if ((chr | 32) == 'u')
			{
				for (int i = 0; i < 4; i++)
				{
					int d = sr.Read();
					if (d >= '0' && d <= '9')
						res = d - '0' + res * 16;
					else
						res = (d | 32) - 'a' + 10 + res * 16;
				}
				return (char)res;
			}

			return ' ';
		}

		static string getQuotedString(StreamReader sr)
		{
			StringBuilder sb = new StringBuilder();
			while (!sr.EndOfStream)
			{
				char chr = (char)sr.Read();
				if (chr == '\\')
					chr = getCodedChar(sr);
				else if (chr == '"')
					return sb.ToString();

				sb.Append(chr);
			}

			return sb.ToString();
		}

		static string getUnquotedString(StreamReader sr)
		{
			StringBuilder sb = new StringBuilder();
			while (!sr.EndOfStream)
			{
				char chr = (char)sr.Peek();
				if (chr == '\\')
				{
					sr.Read();
					chr = getCodedChar(sr);
				}
				else if (char.IsWhiteSpace(chr) || ",:".IndexOf(chr) >= 0)
					return sb.ToString();

				sr.Read();
				sb.Append(chr);
			}

			return sb.ToString();
		}

		internal static List<JsonToken> GetTokens(StreamReader sr)
		{
			List<JsonToken> result = new List<JsonToken>();
			int lineNum = 1;
			int chrNum = 0;

			string tmpStr;

			NumberFormatInfo numberFormat = CultureInfo.CreateSpecificCulture("en-US").NumberFormat;

			char getNextChar()
			//Func<char> getNextChar = () =>
			{
				if (sr.EndOfStream)
					return '\0';

				int chr = sr.Read();

				if (chr != '\n')
					chrNum++;

				if (chr == '\r')
				{
					lineNum++;
					chrNum = 1;
				}

				return (char)chr;
			};

			char peekNextChar()
			//Func<char> peekNextChar = () =>
			{
				if (sr.EndOfStream)
					return '\0';

				return (char)sr.Peek();
			};

			while (true)
			{
				if (sr.EndOfStream)
					break;

				char chr = getNextChar();
				while (!sr.EndOfStream && char.IsWhiteSpace(chr))
					chr = getNextChar();

				JsonToken token = new JsonToken
				{
					Row = lineNum,
					Col = chrNum
				};

				switch (chr)
				{
					case '[':
						token.Text = chr.ToString();
						token.TokenId = JsonTokenType.Array;
						break;

					case ']':
						token.Text = chr.ToString();
						token.TokenId = JsonTokenType.EndArray;
						break;

					case '{':
						token.Text = chr.ToString();
						token.TokenId = JsonTokenType.Record;
						break;

					case '}':
						token.Text = chr.ToString();
						token.TokenId = JsonTokenType.EndRecord;
						break;

					case ':':
						token.Text = chr.ToString();
						token.TokenId = JsonTokenType.colon;
						break;

					case ',':
						token.Text = chr.ToString();
						token.TokenId = JsonTokenType.comma;
						break;

					case '"':
						tmpStr = getQuotedString(sr);
						token.Text = tmpStr;

						if (!double.TryParse(tmpStr, NumberStyles.Any, numberFormat, out _) && Enum.TryParse(tmpStr.ToLower(), out JsonTokenType tokenType))
						{
							token.TokenId = tokenType;
							break;
						}

						token.TokenId = JsonTokenType.String;
						break;
					default:
						if (char.IsDigit(chr) || "-+".IndexOf(chr) >= 0)
						{
							string num = chr.ToString();

							while (char.IsDigit(chr = peekNextChar()))
							{
								getNextChar();
								num += chr;
							}

							if (chr == '.')
								do
								{
									getNextChar();
									num += chr;
								}
								while (char.IsDigit(chr = peekNextChar()));

							// Read scientific notation (suffix)
							if (char.ToLower(chr) == 'e')
							{
								getNextChar();
								num += chr;
								chr = peekNextChar();
								if (chr == '+' || chr == '-')
								{
									getNextChar();
									num += chr;
									chr = peekNextChar();
								}

								while (char.IsDigit(chr))
								{
									getNextChar();
									num += chr;
									chr = peekNextChar();
								}
							}

							if (!double.TryParse(num, NumberStyles.Any, numberFormat, out double fTmp))
							{
								string errorString = string.Format("Invalid number \"{0}\" at line {1} position {2}.", num, lineNum, chrNum);
								throw new Exception(errorString);
							}

							token.dValue = fTmp;
							token.TokenId = JsonTokenType.Double;
						}
						else
						{
							tmpStr = chr + getUnquotedString(sr);
							token.Text = tmpStr;

							if (bool.TryParse(tmpStr.ToLower(), out token.bValue))
							{
								token.TokenId = JsonTokenType.Boolean;
								break;
							}

							if (tmpStr.ToLower() == "null")
							{
								token.TokenId = JsonTokenType.Null;
								break;
							}

							token.TokenId = JsonTokenType.String;       // JsonTokenType.Unknown;
							break;
							//string errorString = string.Format("Unrecognized token \"{0}\" at line {1} position {2}.", chr, lineNum, chrNum);
							//throw new Exception(errorString);
						}
						break;
				}

				//token.endRow = lineNum;
				//token.endCol = chrNum;
				result.Add(token);
			}

			return result;
		}
	}
}
