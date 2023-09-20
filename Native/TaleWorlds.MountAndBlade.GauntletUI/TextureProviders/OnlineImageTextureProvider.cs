using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class OnlineImageTextureProvider : TextureProvider
	{
		public string OnlineSourceUrl
		{
			set
			{
				this._onlineSourceUrl = value;
				this.RefreshOnlineImage();
			}
		}

		public OnlineImageTextureProvider()
		{
			this._onlineImageCache = new Dictionary<string, PlatformFilePath>();
			this._onlineImageCacheFolderPath = new PlatformDirectoryPath(1, this.DataFolder);
			this.PopulateOnlineImageCache();
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			if (this._requiresRetry)
			{
				if (10 < this._retryCount)
				{
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
					string guidOfRequestedURL = OnlineImageTextureProvider.ToGuid(this._onlineSourceUrl).ToString();
					if (!this._onlineImageCache.ContainsKey(guidOfRequestedURL))
					{
						PlatformFilePath pathOfTheDownloadedImage = new PlatformFilePath(this._onlineImageCacheFolderPath, guidOfRequestedURL + ".png");
						byte[] array = await HttpHelper.DownloadDataTaskAsync(this._onlineSourceUrl);
						if (array != null)
						{
							FileHelper.SaveFile(pathOfTheDownloadedImage, array);
							this._onlineImageCache.Add(guidOfRequestedURL, pathOfTheDownloadedImage);
						}
						pathOfTheDownloadedImage = default(PlatformFilePath);
					}
					PlatformFilePath platformFilePath;
					if (this._onlineImageCache.TryGetValue(guidOfRequestedURL, out platformFilePath))
					{
						Texture texture = Texture.CreateTextureFromPath(platformFilePath);
						if (texture == null)
						{
							this._onlineImageCache.Remove(guidOfRequestedURL);
							Debug.Print(string.Format("RETRYING TO DOWNLOAD: {0} | RETRY COUNT: {1}", this._onlineSourceUrl, this._retryCount), 0, 3, 17592186044416UL);
							this._requiresRetry = true;
						}
						else
						{
							this.OnTextureCreated(texture);
							this._requiresRetry = false;
						}
					}
					else
					{
						Debug.Print(string.Format("RETRYING TO DOWNLOAD: {0} | RETRY COUNT: {1}", this._onlineSourceUrl, this._retryCount), 0, 3, 17592186044416UL);
						this._requiresRetry = true;
					}
					guidOfRequestedURL = null;
				}
				catch (Exception ex)
				{
					Debug.FailedAssert("Error while trying to get image online: " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\TextureProviders\\OnlineImageTextureProvider.cs", "RefreshOnlineImage", 109);
					Debug.Print(string.Format("RETRYING TO DOWNLOAD: {0} | RETRY COUNT: {1}", this._onlineSourceUrl, this._retryCount), 0, 3, 17592186044416UL);
					this._requiresRetry = true;
				}
			}
		}

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			if (this._texture != null)
			{
				return new Texture(new EngineTexture(this._texture));
			}
			return null;
		}

		private void PopulateOnlineImageCache()
		{
			foreach (PlatformFilePath platformFilePath in FileHelper.GetFiles(this._onlineImageCacheFolderPath, "*.png"))
			{
				string fileNameWithoutExtension = platformFilePath.GetFileNameWithoutExtension();
				this._onlineImageCache.Add(fileNameWithoutExtension, platformFilePath);
			}
		}

		private static Guid ToGuid(string src)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(src);
			byte[] array = new SHA1CryptoServiceProvider().ComputeHash(bytes);
			Array.Resize<byte>(ref array, 16);
			return new Guid(array);
		}

		private void OnTextureCreated(Texture texture)
		{
			this._texture = texture;
		}

		private Dictionary<string, PlatformFilePath> _onlineImageCache;

		private readonly string DataFolder = "Online Images";

		private readonly PlatformDirectoryPath _onlineImageCacheFolderPath;

		private Texture _texture;

		private bool _requiresRetry;

		private int _retryCount;

		private const int _maxRetryCount = 10;

		private string _onlineSourceUrl;
	}
}
