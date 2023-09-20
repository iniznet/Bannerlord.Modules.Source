using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleWorlds.Library
{
	// Token: 0x02000085 RID: 133
	public static class SRTHelper
	{
		// Token: 0x020000D1 RID: 209
		public static class SrtParser
		{
			// Token: 0x060006EC RID: 1772 RVA: 0x00015368 File Offset: 0x00013568
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

			// Token: 0x060006ED RID: 1773 RVA: 0x00015534 File Offset: 0x00013734
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

			// Token: 0x060006EE RID: 1774 RVA: 0x00015544 File Offset: 0x00013744
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

			// Token: 0x060006EF RID: 1775 RVA: 0x00015584 File Offset: 0x00013784
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

			// Token: 0x0400029B RID: 667
			private static readonly string[] _delimiters = new string[] { "-->", "- >", "->" };
		}

		// Token: 0x020000D2 RID: 210
		public static class StreamHelpers
		{
			// Token: 0x060006F1 RID: 1777 RVA: 0x000155F0 File Offset: 0x000137F0
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

		// Token: 0x020000D3 RID: 211
		public class SubtitleItem
		{
			// Token: 0x170000F3 RID: 243
			// (get) Token: 0x060006F2 RID: 1778 RVA: 0x00015639 File Offset: 0x00013839
			// (set) Token: 0x060006F3 RID: 1779 RVA: 0x00015641 File Offset: 0x00013841
			public int StartTime { get; set; }

			// Token: 0x170000F4 RID: 244
			// (get) Token: 0x060006F4 RID: 1780 RVA: 0x0001564A File Offset: 0x0001384A
			// (set) Token: 0x060006F5 RID: 1781 RVA: 0x00015652 File Offset: 0x00013852
			public int EndTime { get; set; }

			// Token: 0x170000F5 RID: 245
			// (get) Token: 0x060006F6 RID: 1782 RVA: 0x0001565B File Offset: 0x0001385B
			// (set) Token: 0x060006F7 RID: 1783 RVA: 0x00015663 File Offset: 0x00013863
			public List<string> Lines { get; set; }

			// Token: 0x060006F8 RID: 1784 RVA: 0x0001566C File Offset: 0x0001386C
			public SubtitleItem()
			{
				this.Lines = new List<string>();
			}

			// Token: 0x060006F9 RID: 1785 RVA: 0x00015680 File Offset: 0x00013880
			public override string ToString()
			{
				TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, this.StartTime);
				TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, this.EndTime);
				return string.Format("{0} --> {1}: {2}", timeSpan.ToString("G"), timeSpan2.ToString("G"), string.Join(Environment.NewLine, this.Lines));
			}
		}
	}
}
