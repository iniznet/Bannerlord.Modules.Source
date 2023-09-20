using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace TaleWorlds.Library
{
	// Token: 0x02000058 RID: 88
	public class Logger
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000277 RID: 631 RVA: 0x00006C52 File Offset: 0x00004E52
		// (set) Token: 0x06000278 RID: 632 RVA: 0x00006C5A File Offset: 0x00004E5A
		public bool LogOnlyErrors { get; set; }

		// Token: 0x0600027A RID: 634 RVA: 0x00006CB5 File Offset: 0x00004EB5
		public Logger(string name)
			: this(name, false, false, false, 1, -1, false)
		{
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00006CC4 File Offset: 0x00004EC4
		public Logger(string name, bool writeErrorsToDifferentFile, bool logOnlyErrors, bool doNotUseProcessId, int numFiles = 1, int totalFileSize = -1, bool overwrite = false)
		{
			string text = AppDomain.CurrentDomain.FriendlyName;
			text = Path.GetFileNameWithoutExtension(text);
			this._name = name;
			this._writeErrorsToDifferentFile = writeErrorsToDifferentFile;
			this.LogOnlyErrors = logOnlyErrors;
			this._logQueue = new Queue<HTMLDebugData>();
			int id = Process.GetCurrentProcess().Id;
			DateTime now = DateTime.Now;
			string text2 = Logger.LogsFolder;
			if (!doNotUseProcessId)
			{
				string text3 = string.Concat(new object[]
				{
					text,
					"_",
					now.ToString("yyyyMMdd"),
					"_",
					now.ToString("hhmmss"),
					"_",
					id
				});
				text2 = text2 + "/" + text3;
			}
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			this._fileManager = new Logger.FileManager(text2, this._name, numFiles, totalFileSize, overwrite, writeErrorsToDifferentFile);
			List<Logger> loggers = Logger._loggers;
			lock (loggers)
			{
				if (Logger._thread == null)
				{
					Logger._thread = new Thread(new ThreadStart(Logger.ThreadMain));
					Logger._thread.IsBackground = true;
					Logger._thread.Priority = ThreadPriority.BelowNormal;
					Logger._thread.Start();
				}
				Logger._loggers.Add(this);
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00006E24 File Offset: 0x00005024
		private static void ThreadMain()
		{
			while (Logger._running)
			{
				try
				{
					Logger.Printer();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception on network debug thread: " + ex.Message);
				}
			}
			Logger._isOver = true;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00006E70 File Offset: 0x00005070
		private static void Printer()
		{
			while ((Logger._running || Logger._printedOnThisCycle) && Logger._loggers.Count > 0)
			{
				Logger._printedOnThisCycle = false;
				List<Logger> loggers = Logger._loggers;
				lock (loggers)
				{
					using (List<Logger>.Enumerator enumerator = Logger._loggers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.DoLoggingJob())
							{
								Logger._printedOnThisCycle = true;
							}
						}
					}
				}
				if (!Logger._printedOnThisCycle)
				{
					Thread.Sleep(1);
				}
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00006F20 File Offset: 0x00005120
		private bool DoLoggingJob()
		{
			bool flag = false;
			HTMLDebugData htmldebugData = null;
			Queue<HTMLDebugData> logQueue = this._logQueue;
			lock (logQueue)
			{
				if (this._logQueue.Count > 0)
				{
					htmldebugData = this._logQueue.Dequeue();
				}
			}
			if (htmldebugData != null)
			{
				FileStream fileStream = this._fileManager.GetFileStream();
				flag = true;
				htmldebugData.Print(fileStream, Logger._logFileEncoding, true);
				if ((htmldebugData.Info == HTMLDebugCategory.Error || htmldebugData.Info == HTMLDebugCategory.Warning) && this._writeErrorsToDifferentFile)
				{
					htmldebugData.Print(this._fileManager.GetErrorFileStream(), Logger._logFileEncoding, false);
				}
				this._fileManager.CheckForFileSize();
			}
			return flag;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00006FD8 File Offset: 0x000051D8
		public void Print(string log, HTMLDebugCategory debugInfo = HTMLDebugCategory.General)
		{
			this.Print(log, debugInfo, true);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00006FE4 File Offset: 0x000051E4
		public void Print(string log, HTMLDebugCategory debugInfo, bool printOnGlobal)
		{
			if (!this.LogOnlyErrors || (this.LogOnlyErrors && debugInfo == HTMLDebugCategory.Error) || (this.LogOnlyErrors && debugInfo == HTMLDebugCategory.Warning))
			{
				HTMLDebugData htmldebugData = new HTMLDebugData(log, debugInfo);
				Queue<HTMLDebugData> logQueue = this._logQueue;
				lock (logQueue)
				{
					this._logQueue.Enqueue(htmldebugData);
				}
				if (printOnGlobal)
				{
					Debug.Print(log, 0, Debug.DebugColor.White, 17592186044416UL);
				}
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00007068 File Offset: 0x00005268
		public static void FinishAndCloseAll()
		{
			List<Logger> loggers = Logger._loggers;
			lock (loggers)
			{
				Logger._running = false;
				Logger._printedOnThisCycle = true;
			}
			while (!Logger._isOver)
			{
			}
		}

		// Token: 0x040000E4 RID: 228
		private Queue<HTMLDebugData> _logQueue;

		// Token: 0x040000E5 RID: 229
		private static Encoding _logFileEncoding = Encoding.UTF8;

		// Token: 0x040000E6 RID: 230
		private string _name;

		// Token: 0x040000E7 RID: 231
		private bool _writeErrorsToDifferentFile;

		// Token: 0x040000E9 RID: 233
		private static List<Logger> _loggers = new List<Logger>();

		// Token: 0x040000EA RID: 234
		private Logger.FileManager _fileManager;

		// Token: 0x040000EB RID: 235
		private static Thread _thread;

		// Token: 0x040000EC RID: 236
		private static bool _running = true;

		// Token: 0x040000ED RID: 237
		private static bool _printedOnThisCycle = false;

		// Token: 0x040000EE RID: 238
		private static bool _isOver = false;

		// Token: 0x040000EF RID: 239
		public static string LogsFolder = Environment.CurrentDirectory + "\\logs";

		// Token: 0x020000C6 RID: 198
		private class FileManager
		{
			// Token: 0x060006BE RID: 1726 RVA: 0x000148D4 File Offset: 0x00012AD4
			public FileManager(string path, string name, int numFiles, int maxTotalSize, bool overwrite, bool logErrorsToDifferentFile)
			{
				if (maxTotalSize < numFiles * 64 * 1024)
				{
					this._numFiles = 1;
					this._isCheckingFileSize = false;
				}
				else
				{
					this._numFiles = numFiles;
					if (numFiles <= 0)
					{
						this._numFiles = 1;
						this._isCheckingFileSize = false;
					}
					this._maxFileSize = maxTotalSize / this._numFiles;
					this._isCheckingFileSize = true;
				}
				this._streams = new FileStream[this._numFiles];
				this._currentStreamIndex = 0;
				try
				{
					for (int i = 0; i < this._numFiles; i++)
					{
						string text = name + "_" + i;
						string text2 = path + "/" + text + ".html";
						this._streams[i] = (overwrite ? new FileStream(text2, FileMode.Create) : new FileStream(text2, FileMode.OpenOrCreate));
						this.FillEmptyStream(this._streams[i]);
					}
					if (logErrorsToDifferentFile)
					{
						string text3 = path + "/" + name + "_errors.html";
						this._errorStream = (overwrite ? new FileStream(text3, FileMode.Create) : new FileStream(text3, FileMode.OpenOrCreate));
						this.FillEmptyStream(this._errorStream);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error when creating log file(s): " + ex.GetBaseException().Message);
					for (int j = 0; j < this._numFiles; j++)
					{
						string text4 = name + "__" + j;
						string text5 = path + "/" + text4 + ".html";
						this._streams[j] = (overwrite ? new FileStream(text5, FileMode.Create) : new FileStream(text5, FileMode.OpenOrCreate));
						this.FillEmptyStream(this._streams[j]);
					}
					if (logErrorsToDifferentFile)
					{
						string text6 = path + "/" + name + "_errors.html";
						this._errorStream = (overwrite ? new FileStream(text6, FileMode.Create) : new FileStream(text6, FileMode.OpenOrCreate));
						this.FillEmptyStream(this._errorStream);
					}
				}
			}

			// Token: 0x060006BF RID: 1727 RVA: 0x00014AC8 File Offset: 0x00012CC8
			public FileStream GetFileStream()
			{
				return this._streams[this._currentStreamIndex];
			}

			// Token: 0x060006C0 RID: 1728 RVA: 0x00014AD7 File Offset: 0x00012CD7
			public FileStream GetErrorFileStream()
			{
				return this._errorStream;
			}

			// Token: 0x060006C1 RID: 1729 RVA: 0x00014AE0 File Offset: 0x00012CE0
			public void CheckForFileSize()
			{
				if (this._isCheckingFileSize && this._streams[this._currentStreamIndex].Length > (long)this._maxFileSize)
				{
					this._currentStreamIndex = (this._currentStreamIndex + 1) % this._numFiles;
					this.ResetFileStream(this._streams[this._currentStreamIndex]);
				}
			}

			// Token: 0x060006C2 RID: 1730 RVA: 0x00014B38 File Offset: 0x00012D38
			public void ShutDown()
			{
				for (int i = 0; i < this._numFiles; i++)
				{
					this._streams[i].Close();
					this._streams[i] = null;
				}
				if (this._errorStream != null)
				{
					this._errorStream.Close();
					this._errorStream = null;
				}
			}

			// Token: 0x060006C3 RID: 1731 RVA: 0x00014B88 File Offset: 0x00012D88
			private void FillEmptyStream(FileStream stream)
			{
				if (stream.Length == 0L)
				{
					string text = "<table></table>";
					byte[] bytes = Logger._logFileEncoding.GetBytes(text);
					stream.Write(bytes, 0, bytes.Length);
				}
			}

			// Token: 0x060006C4 RID: 1732 RVA: 0x00014BBA File Offset: 0x00012DBA
			private void ResetFileStream(FileStream stream)
			{
				stream.SetLength(0L);
				this.FillEmptyStream(stream);
			}

			// Token: 0x04000265 RID: 613
			private bool _isCheckingFileSize;

			// Token: 0x04000266 RID: 614
			private int _maxFileSize;

			// Token: 0x04000267 RID: 615
			private int _numFiles;

			// Token: 0x04000268 RID: 616
			private FileStream[] _streams;

			// Token: 0x04000269 RID: 617
			private int _currentStreamIndex;

			// Token: 0x0400026A RID: 618
			private FileStream _errorStream;
		}
	}
}
