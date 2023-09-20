using System;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x02000008 RID: 8
	public class AvatarData
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00002DCA File Offset: 0x00000FCA
		public AvatarData()
		{
			this.Status = AvatarData.DataStatus.NotReady;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002DD9 File Offset: 0x00000FD9
		public AvatarData(byte[] image, uint width, uint height)
		{
			this.SetImageData(image, width, height);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002DEA File Offset: 0x00000FEA
		public AvatarData(byte[] image)
		{
			this.SetImageData(image);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002DF9 File Offset: 0x00000FF9
		public void SetImageData(byte[] image, uint width, uint height)
		{
			this.Image = image;
			this.Width = width;
			this.Height = height;
			this.Type = AvatarData.ImageType.Raw;
			this.Status = AvatarData.DataStatus.Ready;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002E1E File Offset: 0x0000101E
		public void SetImageData(byte[] image)
		{
			this.Image = image;
			this.Type = AvatarData.ImageType.Image;
			this.Status = AvatarData.DataStatus.Ready;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002E35 File Offset: 0x00001035
		public void SetFailed()
		{
			this.Status = AvatarData.DataStatus.Failed;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002E3E File Offset: 0x0000103E
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002E46 File Offset: 0x00001046
		public byte[] Image { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002E4F File Offset: 0x0000104F
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002E57 File Offset: 0x00001057
		public uint Width { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002E60 File Offset: 0x00001060
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002E68 File Offset: 0x00001068
		public uint Height { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002E71 File Offset: 0x00001071
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00002E79 File Offset: 0x00001079
		public AvatarData.ImageType Type { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002E82 File Offset: 0x00001082
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002E8A File Offset: 0x0000108A
		public AvatarData.DataStatus Status { get; private set; }

		// Token: 0x02000010 RID: 16
		public enum ImageType
		{
			// Token: 0x0400003C RID: 60
			Image,
			// Token: 0x0400003D RID: 61
			Raw
		}

		// Token: 0x02000011 RID: 17
		public enum DataStatus
		{
			// Token: 0x0400003F RID: 63
			NotReady,
			// Token: 0x04000040 RID: 64
			Ready,
			// Token: 0x04000041 RID: 65
			Failed
		}
	}
}
