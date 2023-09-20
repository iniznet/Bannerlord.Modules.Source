using System;
using System.IO;
using System.Net;
using System.Text;

namespace TaleWorlds.Library
{
	internal class HTMLDebugData
	{
		internal HTMLDebugCategory Info { get; private set; }

		internal HTMLDebugData(string log, HTMLDebugCategory info)
		{
			this._log = log;
			this.Info = info;
			this._currentTime = DateTime.Now.ToString("yyyy/M/d h:mm:ss.fff");
		}

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

		private string _log;

		private string _currentTime;
	}
}
