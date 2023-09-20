using System;
using System.IO;
using System.Net;
using System.Text;

namespace TaleWorlds.Library
{
	// Token: 0x02000032 RID: 50
	internal class HTMLDebugData
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000190 RID: 400 RVA: 0x000060E3 File Offset: 0x000042E3
		// (set) Token: 0x06000191 RID: 401 RVA: 0x000060EB File Offset: 0x000042EB
		internal HTMLDebugCategory Info { get; private set; }

		// Token: 0x06000192 RID: 402 RVA: 0x000060F4 File Offset: 0x000042F4
		internal HTMLDebugData(string log, HTMLDebugCategory info)
		{
			this._log = log;
			this.Info = info;
			this._currentTime = DateTime.Now.ToString("yyyy/M/d h:mm:ss.fff");
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00006130 File Offset: 0x00004330
		private string Color
		{
			get
			{
				string text = "000000";
				switch (this.Info)
				{
				case HTMLDebugCategory.General:
					text = "000000";
					break;
				case HTMLDebugCategory.Connection:
					text = "FF00FF";
					break;
				case HTMLDebugCategory.IncomingMessage:
					text = "EE8800";
					break;
				case HTMLDebugCategory.OutgoingMessage:
					text = "AA6600";
					break;
				case HTMLDebugCategory.Database:
					text = "00008B";
					break;
				case HTMLDebugCategory.Warning:
					text = "0000FF";
					break;
				case HTMLDebugCategory.Error:
					text = "FF0000";
					break;
				case HTMLDebugCategory.Other:
					text = "000000";
					break;
				}
				return text;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000194 RID: 404 RVA: 0x000061B4 File Offset: 0x000043B4
		private ConsoleColor ConsoleColor
		{
			get
			{
				ConsoleColor consoleColor = ConsoleColor.Green;
				HTMLDebugCategory info = this.Info;
				if (info != HTMLDebugCategory.Warning)
				{
					if (info == HTMLDebugCategory.Error)
					{
						consoleColor = ConsoleColor.Red;
					}
				}
				else
				{
					consoleColor = ConsoleColor.Yellow;
				}
				return consoleColor;
			}
		}

		// Token: 0x06000195 RID: 405 RVA: 0x000061DC File Offset: 0x000043DC
		internal void Print(FileStream fileStream, Encoding encoding, bool writeToConsole = true)
		{
			if (writeToConsole)
			{
				Console.ForegroundColor = this.ConsoleColor;
				Console.WriteLine(this._log);
				Console.ForegroundColor = this.ConsoleColor;
			}
			int byteCount = encoding.GetByteCount("</table>");
			string color = this.Color;
			string text = string.Concat(new string[]
			{
				"<tr>",
				this.TableCell(this._log, color).Replace("\n", "<br/>"),
				this.TableCell(this.Info.ToString(), color),
				this.TableCell(this._currentTime, color),
				"</tr></table>"
			});
			byte[] bytes = encoding.GetBytes(text);
			fileStream.Seek((long)(-(long)byteCount), SeekOrigin.End);
			fileStream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x000062A8 File Offset: 0x000044A8
		private string TableCell(string innerText, string color)
		{
			return string.Concat(new string[]
			{
				"<td><font color='#",
				color,
				"'>",
				WebUtility.HtmlEncode(innerText),
				"</font></td><td>"
			});
		}

		// Token: 0x0400008F RID: 143
		private string _log;

		// Token: 0x04000091 RID: 145
		private string _currentTime;
	}
}
