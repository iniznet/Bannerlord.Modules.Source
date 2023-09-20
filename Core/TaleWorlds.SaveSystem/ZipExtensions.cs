using System;
using System.IO;
using System.IO.Compression;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000024 RID: 36
	internal static class ZipExtensions
	{
		// Token: 0x0600013C RID: 316 RVA: 0x000060A0 File Offset: 0x000042A0
		public static void FillFrom(this ZipArchiveEntry entry, byte[] data)
		{
			using (Stream stream = entry.Open())
			{
				stream.Write(data, 0, data.Length);
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000060DC File Offset: 0x000042DC
		public static void FillFrom(this ZipArchiveEntry entry, BinaryWriter writer)
		{
			using (Stream stream = entry.Open())
			{
				byte[] data = writer.Data;
				stream.Write(data, 0, data.Length);
			}
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00006120 File Offset: 0x00004320
		public static BinaryReader GetBinaryReader(this ZipArchiveEntry entry)
		{
			BinaryReader binaryReader = null;
			using (Stream stream = entry.Open())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					binaryReader = new BinaryReader(memoryStream.ToArray());
				}
			}
			return binaryReader;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00006184 File Offset: 0x00004384
		public static byte[] GetBinaryData(this ZipArchiveEntry entry)
		{
			byte[] array = null;
			using (Stream stream = entry.Open())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					array = memoryStream.ToArray();
				}
			}
			return array;
		}
	}
}
