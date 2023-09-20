using System;
using System.IO;

namespace TaleWorlds.Network
{
	// Token: 0x0200002B RID: 43
	public class MessageInfo
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00005307 File Offset: 0x00003507
		// (set) Token: 0x0600014A RID: 330 RVA: 0x0000530F File Offset: 0x0000350F
		public string SourceIPAddress { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00005318 File Offset: 0x00003518
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00005320 File Offset: 0x00003520
		public Guid SourceClientId { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00005329 File Offset: 0x00003529
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00005331 File Offset: 0x00003531
		public string SourceUserName { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600014F RID: 335 RVA: 0x0000533A File Offset: 0x0000353A
		// (set) Token: 0x06000150 RID: 336 RVA: 0x00005342 File Offset: 0x00003542
		public string SourcePlatform { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000151 RID: 337 RVA: 0x0000534B File Offset: 0x0000354B
		// (set) Token: 0x06000152 RID: 338 RVA: 0x00005353 File Offset: 0x00003553
		public string SourcePlatformId { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000153 RID: 339 RVA: 0x0000535C File Offset: 0x0000355C
		// (set) Token: 0x06000154 RID: 340 RVA: 0x00005364 File Offset: 0x00003564
		public string DestinationPostBox { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000155 RID: 341 RVA: 0x0000536D File Offset: 0x0000356D
		// (set) Token: 0x06000156 RID: 342 RVA: 0x00005375 File Offset: 0x00003575
		public Guid DestinationClientId { get; set; }

		// Token: 0x06000157 RID: 343 RVA: 0x00005380 File Offset: 0x00003580
		public void WriteTo(Stream stream, bool fromServer)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(this.DestinationIsPostBox);
			if (this.DestinationIsPostBox)
			{
				binaryWriter.Write(this.DestinationPostBox);
			}
			else
			{
				binaryWriter.Write(this.DestinationClientId.ToByteArray());
			}
			if (fromServer)
			{
				binaryWriter.Write(this.SourceIPAddress);
				binaryWriter.Write(this.SourceClientId.ToByteArray());
				binaryWriter.Write(this.SourceUserName);
				binaryWriter.Write(this.SourcePlatform);
				binaryWriter.Write(this.SourcePlatformId);
			}
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00005414 File Offset: 0x00003614
		public static MessageInfo ReadFrom(Stream stream, bool fromServer)
		{
			MessageInfo messageInfo = new MessageInfo();
			BinaryReader binaryReader = new BinaryReader(stream);
			messageInfo.DestinationIsPostBox = binaryReader.ReadBoolean();
			if (messageInfo.DestinationIsPostBox)
			{
				messageInfo.DestinationPostBox = binaryReader.ReadString();
			}
			else
			{
				messageInfo.DestinationClientId = new Guid(binaryReader.ReadBytes(16));
			}
			if (fromServer)
			{
				messageInfo.SourceIPAddress = binaryReader.ReadString();
				messageInfo.SourceClientId = new Guid(binaryReader.ReadBytes(16));
				messageInfo.SourceUserName = binaryReader.ReadString();
				messageInfo.SourcePlatform = binaryReader.ReadString();
				messageInfo.SourcePlatformId = binaryReader.ReadString();
			}
			return messageInfo;
		}

		// Token: 0x04000084 RID: 132
		public bool DestinationIsPostBox = true;
	}
}
