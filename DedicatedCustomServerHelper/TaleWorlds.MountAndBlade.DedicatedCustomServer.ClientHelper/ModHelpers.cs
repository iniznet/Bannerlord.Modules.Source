using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	internal static class ModHelpers
	{
		public static string RootPath
		{
			get
			{
				return ModuleHelper.GetModuleFullPath("DedicatedCustomServerHelper");
			}
		}

		public static string GetSceneObjRootPath()
		{
			string text = Path.Combine(ModHelpers.RootPath, "SceneObj");
			if (!Directory.Exists(text))
			{
				ModLogger.Log("Helper module didn't have 'SceneObj' directory; creating it now", 0, 4);
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static bool DoesSceneFolderAlreadyExist(string sceneName)
		{
			return Directory.Exists(Path.Combine(ModHelpers.GetSceneObjRootPath(), sceneName));
		}

		public static string GetTempFilePath(string anyIdentifier)
		{
			return Path.Combine(Path.GetTempPath(), "BL_" + anyIdentifier + "_" + Guid.NewGuid().ToString());
		}

		public static string ReadSceneNameOfDirectory(string sceneDirectoryPath)
		{
			string text = null;
			using (XmlReader xmlReader = XmlReader.Create(Path.Combine(sceneDirectoryPath, "scene.xscene")))
			{
				if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "scene")
				{
					text = xmlReader.GetAttribute("name");
				}
			}
			if (text == null)
			{
				throw new Exception("Couldn't retrieve name from 'scene.xscene'");
			}
			if (DedicatedCustomServerClientHelperSubModule.DebugMode)
			{
				text = text + "__" + Guid.NewGuid().ToString();
			}
			return text;
		}

		public static string WriteBufferToTempFile(byte[] buffer)
		{
			string tempFilePath = ModHelpers.GetTempFilePath("map_dl");
			using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
			{
				fileStream.Write(buffer, 0, buffer.Length);
				ModLogger.Log("Wrote buffer to temp file", 0, 4);
			}
			return tempFilePath;
		}

		public static FileStream GetTempFileStream()
		{
			return new FileStream(ModHelpers.GetTempFilePath("map_dl"), FileMode.Create, FileAccess.Write);
		}

		public static string ExtractZipToTempDirectory(string sourceZipFilePath)
		{
			DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(ModHelpers.GetSceneObjRootPath(), "temp_" + Guid.NewGuid().ToString()));
			ZipFile.ExtractToDirectory(sourceZipFilePath, directoryInfo.FullName);
			ModLogger.Log("Extracted zip to directory '" + directoryInfo.FullName + "'", 0, 4);
			return directoryInfo.FullName;
		}

		public static async Task<string> DownloadToTempFile(HttpClient httpClient, string url, IProgress<ProgressUpdate> progress = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			string tempFilePath;
			using (FileStream tempFileStream = ModHelpers.GetTempFileStream())
			{
				tempFilePath = tempFileStream.Name;
				HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
				using (HttpResponseMessage response = httpResponseMessage)
				{
					if (!response.IsSuccessStatusCode)
					{
						string text = await response.Content.ReadAsStringAsync();
						throw new Exception(string.Format("Server responded with {0}: '{1}'", response.StatusCode, text));
					}
					long? contentLength = response.Content.Headers.ContentLength;
					using (Stream downloadStream = await response.Content.ReadAsStreamAsync())
					{
						if (progress == null || contentLength == null)
						{
							await downloadStream.CopyToAsync(tempFileStream);
						}
						else
						{
							byte[] buffer = new byte[81920];
							long totalBytesRead = 0L;
							int bytesRead;
							while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
							{
								await tempFileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
								totalBytesRead += (long)bytesRead;
								progress.Report(new ProgressUpdate(totalBytesRead, contentLength.Value));
							}
							buffer = null;
						}
					}
					Stream downloadStream = null;
					contentLength = null;
				}
				HttpResponseMessage response = null;
			}
			FileStream tempFileStream = null;
			return tempFilePath;
		}
	}
}
