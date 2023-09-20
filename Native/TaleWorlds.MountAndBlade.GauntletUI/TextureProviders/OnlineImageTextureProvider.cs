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
	// Token: 0x02000019 RID: 25
	public class OnlineImageTextureProvider : TextureProvider
	{
		// Token: 0x17000035 RID: 53
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00005FA4 File Offset: 0x000041A4
		public string OnlineSourceUrl
		{
			set
			{
				this._onlineSourceUrl = value;
				this.RefreshOnlineImage();
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00005FB3 File Offset: 0x000041B3
		public OnlineImageTextureProvider()
		{
			this._onlineImageCache = new Dictionary<string, PlatformFilePath>();
			this._onlineImageCacheFolderPath = new PlatformDirectoryPath(1, this.DataFolder);
			this.PopulateOnlineImageCache();
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00005FE9 File Offset: 0x000041E9
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

		// Token: 0x060000E7 RID: 231 RVA: 0x00006020 File Offset: 0x00004220
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

		// Token: 0x060000E8 RID: 232 RVA: 0x00006059 File Offset: 0x00004259
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			if (this._texture != null)
			{
				return new Texture(new EngineTexture(this._texture));
			}
			return null;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000607C File Offset: 0x0000427C
		private void PopulateOnlineImageCache()
		{
			foreach (PlatformFilePath platformFilePath in FileHelper.GetFiles(this._onlineImageCacheFolderPath, "*.png"))
			{
				string fileNameWithoutExtension = platformFilePath.GetFileNameWithoutExtension();
				this._onlineImageCache.Add(fileNameWithoutExtension, platformFilePath);
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000060C8 File Offset: 0x000042C8
		private static Guid ToGuid(string src)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(src);
			byte[] array = new SHA1CryptoServiceProvider().ComputeHash(bytes);
			Array.Resize<byte>(ref array, 16);
			return new Guid(array);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000060FC File Offset: 0x000042FC
		private void OnTextureCreated(Texture texture)
		{
			this._texture = texture;
		}

		// Token: 0x04000083 RID: 131
		private Dictionary<string, PlatformFilePath> _onlineImageCache;

		// Token: 0x04000084 RID: 132
		private readonly string DataFolder = "Online Images";

		// Token: 0x04000085 RID: 133
		private readonly PlatformDirectoryPath _onlineImageCacheFolderPath;

		// Token: 0x04000086 RID: 134
		private Texture _texture;

		// Token: 0x04000087 RID: 135
		private bool _requiresRetry;

		// Token: 0x04000088 RID: 136
		private int _retryCount;

		// Token: 0x04000089 RID: 137
		private const int _maxRetryCount = 10;

		// Token: 0x0400008A RID: 138
		private string _onlineSourceUrl;
	}
}
