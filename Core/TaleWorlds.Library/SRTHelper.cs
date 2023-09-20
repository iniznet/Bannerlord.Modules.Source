using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleWorlds.Library
{
	public static class SRTHelper
	{
		public static class SrtParser
		{
			public static List<SRTHelper.SubtitleItem> ParseStream(Stream subtitleStream, Encoding encoding)
			{
				if (!subtitleStream.CanRead || !subtitleStream.CanSeek)
				{
					throw new ArgumentException("Given subtitle file is not readable.");
				}
				subtitleStream.Position = 0L;
				TextReader textReader = new StreamReader(subtitleStream, encoding, true);
				List<SRTHelper.SubtitleItem> list = new List<SRTHelper.SubtitleItem>();
				List<string> list2 = SRTHelper.SrtParser.GetSrtSubTitleParts(textReader).ToList<string>();
				if (list2.Count <= 0)
				{
					throw new FormatException("Parsing as srt returned no srt part.");
				}
				foreach (string text in list2)
				{
					List<string> list3 = (from s in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
						select s.Trim() into l
						where !string.IsNullOrEmpty(l)
						select l).ToList<string>();
					SRTHelper.SubtitleItem subtitleItem = new SRTHelper.SubtitleItem();
					foreach (string text2 in list3)
					{
						if (subtitleItem.StartTime == 0 && subtitleItem.EndTime == 0)
						{
							int num;
							int num2;
							if (SRTHelper.SrtParser.TryParseTimecodeLine(text2, out num, out num2))
							{
								subtitleItem.StartTime = num;
								subtitleItem.EndTime = num2;
							}
						}
						else
						{
							subtitleItem.Lines.Add(text2);
						}
					}
					if ((subtitleItem.StartTime != 0 || subtitleItem.EndTime != 0) && subtitleItem.Lines.Count > 0)
					{
						list.Add(subtitleItem);
					}
				}
				if (list.Count > 0)
				{
					return list;
				}
				throw new ArgumentException("Stream is not in a valid Srt format");
			}

			private static IEnumerable<string> GetSrtSubTitleParts(TextReader reader)
			{
				MBStringBuilder sb = default(MBStringBuilder);
				sb.Initialize(16, "GetSrtSubTitleParts");
				string text;
				while ((text = reader.ReadLine()) != null)
				{
					if (string.IsNullOrEmpty(text.Trim()))
					{
						string text2 = sb.ToStringAndRelease().TrimEnd(Array.Empty<char>());
						if (!string.IsNullOrEmpty(text2))
						{
							yield return text2;
						}
						sb.Initialize(16, "GetSrtSubTitleParts");
					}
					else
					{
						sb.AppendLine<string>(text);
					}
				}
				if (sb.Length > 0)
				{
					yield return sb.ToStringAndRelease();
				}
				else
				{
					sb.Release();
				}
				yield break;
			}

			private static bool TryParseTimecodeLine(string line, out int startTc, out int endTc)
			{
				string[] array = line.Split(SRTHelper.SrtParser._delimiters, StringSplitOptions.None);
				if (array.Length != 2)
				{
					startTc = -1;
					endTc = -1;
					return false;
				}
				startTc = SRTHelper.SrtParser.ParseSrtTimecode(array[0]);
				endTc = SRTHelper.SrtParser.ParseSrtTimecode(array[1]);
				return true;
			}

			private static int ParseSrtTimecode(string s)
			{
				Match match = Regex.Match(s, "[0-9]+:[0-9]+:[0-9]+([,\\.][0-9]+)?");
				if (match.Success)
				{
					s = match.Value;
					TimeSpan timeSpan;
					if (TimeSpan.TryParse(s.Replace(',', '.'), out timeSpan))
					{
						return (int)timeSpan.TotalMilliseconds;
					}
				}
				return -1;
			}

			private static readonly string[] _delimiters = new string[] { "-->", "- >", "->" };
		}

		public static class StreamHelpers
		{
			public static Stream CopyStream(Stream inputStream)
			{
				MemoryStream memoryStream = new MemoryStream();
				int num;
				do
				{
					byte[] array = new byte[1024];
					num = inputStream.Read(array, 0, 1024);
					memoryStream.Write(array, 0, num);
				}
				while (inputStream.CanRead && num > 0);
				memoryStream.ToArray();
				return memoryStream;
			}
		}

		public class SubtitleItem
		{
			public int StartTime { get; set; }

			public int EndTime { get; set; }

			public List<string> Lines { get; set; }

			public SubtitleItem()
			{
				this.Lines = new List<string>();
			}

			public override string ToString()
			{
				TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, this.StartTime);
				TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, this.EndTime);
				return string.Format("{0} --> {1}: {2}", timeSpan.ToString("G"), timeSpan2.ToString("G"), string.Join(Environment.NewLine, this.Lines));
			}
		}
	}
}
