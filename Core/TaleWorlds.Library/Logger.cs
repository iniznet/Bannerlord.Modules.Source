using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace TaleWorlds.Library
{
	public class Logger
	{
		public bool LogOnlyErrors { get; set; }

		public Logger(string name)
			: this(name, false, false, false, 1, -1, false)
		{
		}

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

		public void Print(string log, HTMLDebugCategory debugInfo = HTMLDebugCategory.General)
		{
			this.Print(log, debugInfo, true);
		}

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

		private Queue<HTMLDebugData> _logQueue;

		private static Encoding _logFileEncoding = Encoding.UTF8;

		private string _name;

		private bool _writeErrorsToDifferentFile;

		private static List<Logger> _loggers = new List<Logger>();

		private Logger.FileManager _fileManager;

		private static Thread _thread;

		private static bool _running = true;

		private static bool _printedOnThisCycle = false;

		private static bool _isOver = false;

		public static string LogsFolder = Environment.CurrentDirectory + "\\logs";

		private class FileManager
		{
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

			public FileStream GetFileStream()
			{
				return this._streams[this._currentStreamIndex];
			}

			public FileStream GetErrorFileStream()
			{
				return this._errorStream;
			}

			public void CheckForFileSize()
			{
				if (this._isCheckingFileSize && this._streams[this._currentStreamIndex].Length > (long)this._maxFileSize)
				{
					this._currentStreamIndex = (this._currentStreamIndex + 1) % this._numFiles;
					this.ResetFileStream(this._streams[this._currentStreamIndex]);
				}
			}

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

			private void FillEmptyStream(FileStream stream)
			{
				if (stream.Length == 0L)
				{
					string text = "<table></table>";
					byte[] bytes = Logger._logFileEncoding.GetBytes(text);
					stream.Write(bytes, 0, bytes.Length);
				}
			}

			private void ResetFileStream(FileStream stream)
			{
				stream.SetLength(0L);
				this.FillEmptyStream(stream);
			}

			private bool _isCheckingFileSize;

			private int _maxFileSize;

			private int _numFiles;

			private FileStream[] _streams;

			private int _currentStreamIndex;

			private FileStream _errorStream;
		}
	}
}
