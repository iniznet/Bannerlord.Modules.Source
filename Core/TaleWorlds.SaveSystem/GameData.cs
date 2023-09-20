using System;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x0200000E RID: 14
	[Serializable]
	public class GameData
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000270B File Offset: 0x0000090B
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002713 File Offset: 0x00000913
		public byte[] Header { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002D RID: 45 RVA: 0x0000271C File Offset: 0x0000091C
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002724 File Offset: 0x00000924
		public byte[] Strings { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002F RID: 47 RVA: 0x0000272D File Offset: 0x0000092D
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002735 File Offset: 0x00000935
		public byte[][] ObjectData { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000031 RID: 49 RVA: 0x0000273E File Offset: 0x0000093E
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002746 File Offset: 0x00000946
		public byte[][] ContainerData { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002750 File Offset: 0x00000950
		public int TotalSize
		{
			get
			{
				int num = this.Header.Length;
				num += this.Strings.Length;
				for (int i = 0; i < this.ObjectData.Length; i++)
				{
					num += this.ObjectData[i].Length;
				}
				for (int j = 0; j < this.ContainerData.Length; j++)
				{
					num += this.ContainerData[j].Length;
				}
				return num;
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000027B2 File Offset: 0x000009B2
		public GameData(byte[] header, byte[] strings, byte[][] objectData, byte[][] containerData)
		{
			this.Header = header;
			this.Strings = strings;
			this.ObjectData = objectData;
			this.ContainerData = containerData;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000027D8 File Offset: 0x000009D8
		public void Inspect()
		{
			Debug.Print("Header Size: " + this.Header.Length, 0, Debug.DebugColor.White, 17592186044416UL);
			Debug.Print("Strings Size: " + this.Strings.Length, 0, Debug.DebugColor.White, 17592186044416UL);
			Debug.Print("Object Count: " + this.ObjectData.Length, 0, Debug.DebugColor.White, 17592186044416UL);
			Debug.Print("Container Count: " + this.ContainerData.Length, 0, Debug.DebugColor.White, 17592186044416UL);
			int num = 0;
			for (int i = 0; i < this.ObjectData.Length; i++)
			{
				int num2 = this.ObjectData[i].Length;
				if (num2 > num)
				{
					num = num2;
				}
			}
			Debug.Print("Highest Object Size: " + num, 0, Debug.DebugColor.White, 17592186044416UL);
			int num3 = 0;
			for (int j = 0; j < this.ContainerData.Length; j++)
			{
				int num4 = this.ContainerData[j].Length;
				if (num4 > num3)
				{
					num3 = num4;
				}
			}
			Debug.Print("Highest Container Size: " + num3, 0, Debug.DebugColor.White, 17592186044416UL);
			float num5 = (float)this.TotalSize / 1048576f;
			Debug.Print(string.Format("Total size: {0:##.00} MB", num5), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000294D File Offset: 0x00000B4D
		public static GameData CreateFrom(byte[] readBytes)
		{
			return (GameData)Common.DeserializeObject(readBytes);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000295A File Offset: 0x00000B5A
		public byte[] GetData()
		{
			return Common.SerializeObject(this);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002964 File Offset: 0x00000B64
		public static void Write(BinaryWriter writer, GameData gameData)
		{
			writer.Write(gameData.Header.Length);
			writer.Write(gameData.Header);
			writer.Write(gameData.ObjectData.Length);
			foreach (byte[] array2 in gameData.ObjectData)
			{
				writer.Write(array2.Length);
				writer.Write(array2);
			}
			writer.Write(gameData.ContainerData.Length);
			foreach (byte[] array3 in gameData.ContainerData)
			{
				writer.Write(array3.Length);
				writer.Write(array3);
			}
			writer.Write(gameData.Strings.Length);
			writer.Write(gameData.Strings);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002A14 File Offset: 0x00000C14
		public static GameData Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			byte[] array = reader.ReadBytes(num);
			int num2 = reader.ReadInt32();
			byte[][] array2 = new byte[num2][];
			for (int i = 0; i < num2; i++)
			{
				int num3 = reader.ReadInt32();
				array2[i] = reader.ReadBytes(num3);
			}
			int num4 = reader.ReadInt32();
			byte[][] array3 = new byte[num4][];
			for (int j = 0; j < num4; j++)
			{
				int num5 = reader.ReadInt32();
				array3[j] = reader.ReadBytes(num5);
			}
			int num6 = reader.ReadInt32();
			byte[] array4 = reader.ReadBytes(num6);
			return new GameData(array, array4, array2, array3);
		}
	}
}
