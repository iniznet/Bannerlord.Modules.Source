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
			Debug.Print(string.Format("Header Size: {0} Strings Size: {1} Object Size: {2} Container Size: {3}", new object[]
			{
				this.Header.Length,
				this.Strings.Length,
				this.ObjectData.Length,
				this.ContainerData.Length
			}), 0, Debug.DebugColor.White, 17592186044416UL);
			float num = (float)this.TotalSize / 1048576f;
			Debug.Print(string.Format("Total size: {0:##.00} MB", num), 0, Debug.DebugColor.White, 17592186044416UL);
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

		public bool IsEqualTo(GameData gameData)
		{
			bool flag = this.CompareByteArrays(this.Header, gameData.Header, "Header");
			bool flag2 = this.CompareByteArrays(this.Strings, gameData.Strings, "Strings");
			bool flag3 = this.CompareByteArrays(this.ObjectData, gameData.ObjectData, "ObjectData");
			bool flag4 = this.CompareByteArrays(this.ContainerData, gameData.ContainerData, "ContainerData");
			return flag && flag2 && flag3 && flag4;
		}

		private bool CompareByteArrays(byte[] arr1, byte[] arr2, string name)
		{
			if (arr1.Length != arr2.Length)
			{
				Debug.FailedAssert(name + " failed length comparison.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\GameData.cs", "CompareByteArrays", 142);
				return false;
			}
			for (int i = 0; i < arr1.Length; i++)
			{
				if (arr1[i] != arr2[i])
				{
					Debug.FailedAssert(string.Format("{0} failed byte comparison at index {1}.", name, i), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\GameData.cs", "CompareByteArrays", 150);
					return false;
				}
			}
			return true;
		}

		private bool CompareByteArrays(byte[][] arr1, byte[][] arr2, string name)
		{
			if (arr1.Length != arr2.Length)
			{
				Debug.FailedAssert(name + " failed length comparison.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\GameData.cs", "CompareByteArrays", 161);
				return false;
			}
			for (int i = 0; i < arr1.Length; i++)
			{
				if (arr1[i].Length != arr2[i].Length)
				{
					Debug.FailedAssert(name + " failed length comparison.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\GameData.cs", "CompareByteArrays", 168);
					return false;
				}
				for (int j = 0; j < arr1[i].Length; j++)
				{
					if (arr1[i][j] != arr2[i][j])
					{
						Debug.FailedAssert(string.Format("{0} failed byte comparison at index {1}-{2}.", name, i, j), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\GameData.cs", "CompareByteArrays", 176);
					}
				}
			}
			return true;
		}
	}
}
