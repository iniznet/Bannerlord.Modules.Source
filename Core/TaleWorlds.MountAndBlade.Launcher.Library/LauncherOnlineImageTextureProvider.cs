using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;
using TaleWorlds.TwoDimension.Standalone;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherOnlineImageTextureProvider : TextureProvider
	{
		public string OnlineSourceUrl
		{
			set
			{
				this._onlineSourceUrl = value;
				if (!string.IsNullOrEmpty(this._onlineSourceUrl))
				{
					this.RefreshOnlineImage();
				}
			}
		}

		public LauncherOnlineImageTextureProvider()
		{
			this._onlineImageCache = new Dictionary<string, string>();
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			this._onlineImageCacheFolderPath = folderPath + "/Mount and Blade II Bannerlord/Online Images/";
			if (!Directory.Exists(this._onlineImageCacheFolderPath))
			{
				Directory.CreateDirectory(this._onlineImageCacheFolderPath);
			}
			this.PopulateOnlineImageCache();
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			if (this._requiresRetry)
			{
				if (this._retryCount > 10)
				{
					Debug.FailedAssert("Couldn't download " + this._onlineSourceUrl, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Launcher.Library\\LauncherOnlineImageTextureProvider.cs", "Tick", 61);
					this._requiresRetry = false;
					return;
				}
				this._retryCount++;
				this.RefreshOnlineImage();
			}
		}

		private async void RefreshOnlineImage()
		{
			if (this._retryCount < 10)
			{
				try
				{
					this._texture = null;
					string guidOfRequestedURL = LauncherOnlineImageTextureProvider.ToGuid(this._onlineSourceUrl).ToString();
					if (!this._onlineImageCache.ContainsKey(guidOfRequestedURL))
					{
						using (WebClient client = new WebClient())
						{
							string pathOfTheDownloadedImage = this._onlineImageCacheFolderPath + guidOfRequestedURL + ".png";
							Task downloadTask = client.DownloadFileTaskAsync(new Uri(this._onlineSourceUrl), pathOfTheDownloadedImage);
							await downloadTask;
							if (downloadTask.Status == TaskStatus.RanToCompletion)
							{
								this._onlineImageCache.Add(guidOfRequestedURL, pathOfTheDownloadedImage);
							}
							pathOfTheDownloadedImage = null;
							downloadTask = null;
						}
						WebClient client = null;
					}
					string text;
					if (this._onlineImageCache.TryGetValue(guidOfRequestedURL, out text))
					{
						OpenGLTexture openGLTexture = OpenGLTexture.FromFile(text);
						if (openGLTexture == null)
						{
							this._onlineImageCache.Remove(guidOfRequestedURL);
							Debug.Print(string.Format("RETRYING TO DOWNLOAD: {0} | RETRY COUNT: {1}", this._onlineSourceUrl, this._retryCount), 0, Debug.DebugColor.Red, 17592186044416UL);
							this._requiresRetry = true;
						}
						else
						{
							openGLTexture.ClampToEdge = true;
							Texture texture = new Texture(openGLTexture);
							this.OnTextureCreated(texture);
							this._requiresRetry = false;
						}
					}
					else
					{
						Debug.Print(string.Format("RETRYING TO DOWNLOAD: {0} | RETRY COUNT: {1}", this._onlineSourceUrl, this._retryCount), 0, Debug.DebugColor.Red, 17592186044416UL);
						this._requiresRetry = true;
					}
					guidOfRequestedURL = null;
				}
				catch (Exception ex)
				{
					Debug.FailedAssert("Error while trying to get image online: " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Launcher.Library\\LauncherOnlineImageTextureProvider.cs", "RefreshOnlineImage", 123);
				}
			}
		}

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			return this._texture;
		}

		private void PopulateOnlineImageCache()
		{
			if (Directory.Exists(this._onlineImageCacheFolderPath))
			{
				foreach (string text in Directory.GetFiles(this._onlineImageCacheFolderPath, "*.png"))
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
					this._onlineImageCache.Add(fileNameWithoutExtension, text);
				}
			}
		}

		private static Guid ToGuid(string src)
		{
			if (!string.IsNullOrEmpty(src))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(src);
				byte[] array = new SHA1CryptoServiceProvider().ComputeHash(bytes);
				Array.Resize<byte>(ref array, 16);
				return new Guid(array);
			}
			return Guid.Empty;
		}

		private void OnTextureCreated(Texture texture)
		{
			this._texture = texture;
		}

		private Dictionary<string, string> _onlineImageCache;

		private const string DataFolder = "/Mount and Blade II Bannerlord/Online Images/";

		private readonly string _onlineImageCacheFolderPath;

		private Texture _texture;

		private bool _requiresRetry;

		private int _retryCount;

		private const int _maxRetryCount = 10;

		private string _onlineSourceUrl;
	}
}
