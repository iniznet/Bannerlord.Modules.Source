using System;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
	[Serializable]
	public class GameData
	{
		public byte[] Header { get; private set; }

		public byte[] Strings { get; private set; }

		public byte[][] ObjectData { get; private set; }

		public byte[][] ContainerData { get; private set; }

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

		public GameData(byte[] header, byte[] strings, byte[][] objectData, byte[][] containerData)
		{
			this.Header = header;
			this.Strings = strings;
			this.ObjectData = objectData;
			this.ContainerData = containerData;
		}

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

		public static GameData CreateFrom(byte[] readBytes)
		{
			return (GameData)Common.DeserializeObject(readBytes);
		}

		public byte[] GetData()
		{
			return Common.SerializeObject(this);
		}

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
