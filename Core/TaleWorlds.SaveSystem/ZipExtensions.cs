using System;
using System.IO;
using System.IO.Compression;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	internal static class ZipExtensions
	{
		public static void FillFrom(this ZipArchiveEntry entry, byte[] data)
		{
			using (Stream stream = entry.Open())
			{
				stream.Write(data, 0, data.Length);
			}
		}

		public static void FillFrom(this ZipArchiveEntry entry, BinaryWriter writer)
		{
			using (Stream stream = entry.Open())
			{
				byte[] data = writer.Data;
				stream.Write(data, 0, data.Length);
			}
		}

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
