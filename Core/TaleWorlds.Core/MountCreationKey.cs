using System;
using System.Globalization;

namespace TaleWorlds.Core
{
	public class MountCreationKey
	{
		public byte _leftFrontLegColorIndex { get; private set; }

		public byte _rightFrontLegColorIndex { get; private set; }

		public byte _leftBackLegColorIndex { get; private set; }

		public byte _rightBackLegColorIndex { get; private set; }

		public byte MaterialIndex { get; private set; }

		public byte MeshMultiplierIndex { get; private set; }

		public MountCreationKey(byte leftFrontLegColorIndex, byte rightFrontLegColorIndex, byte leftBackLegColorIndex, byte rightBackLegColorIndex, byte materialIndex, byte meshMultiplierIndex)
		{
			if (leftFrontLegColorIndex == 3 || rightFrontLegColorIndex == 3)
			{
				leftFrontLegColorIndex = 3;
				rightFrontLegColorIndex = 3;
			}
			this._leftFrontLegColorIndex = leftFrontLegColorIndex;
			this._rightFrontLegColorIndex = rightFrontLegColorIndex;
			this._leftBackLegColorIndex = leftBackLegColorIndex;
			this._rightBackLegColorIndex = rightBackLegColorIndex;
			this.MaterialIndex = materialIndex;
			this.MeshMultiplierIndex = meshMultiplierIndex;
		}

		public static MountCreationKey FromString(string str)
		{
			if (str != null)
			{
				uint num = uint.Parse(str, NumberStyles.HexNumber);
				int bitsFromKey = MountCreationKey.GetBitsFromKey(num, 0, 2);
				int bitsFromKey2 = MountCreationKey.GetBitsFromKey(num, 2, 2);
				int bitsFromKey3 = MountCreationKey.GetBitsFromKey(num, 4, 2);
				int bitsFromKey4 = MountCreationKey.GetBitsFromKey(num, 6, 2);
				int bitsFromKey5 = MountCreationKey.GetBitsFromKey(num, 8, 2);
				int bitsFromKey6 = MountCreationKey.GetBitsFromKey(num, 10, 2);
				return new MountCreationKey((byte)bitsFromKey, (byte)bitsFromKey2, (byte)bitsFromKey3, (byte)bitsFromKey4, (byte)bitsFromKey5, (byte)bitsFromKey6);
			}
			return new MountCreationKey(0, 0, 0, 0, 0, 0);
		}

		public override string ToString()
		{
			uint num = 0U;
			this.SetBits(ref num, (int)this._leftFrontLegColorIndex, 0);
			this.SetBits(ref num, (int)this._rightFrontLegColorIndex, 2);
			this.SetBits(ref num, (int)this._leftBackLegColorIndex, 4);
			this.SetBits(ref num, (int)this._rightBackLegColorIndex, 6);
			this.SetBits(ref num, (int)this.MaterialIndex, 8);
			this.SetBits(ref num, (int)this.MeshMultiplierIndex, 10);
			return num.ToString("X");
		}

		private static int GetBitsFromKey(uint numericKey, int startingBit, int numBits)
		{
			int num = (int)(numericKey >> startingBit);
			uint num2 = (uint)(numBits * numBits - 1);
			return num & (int)num2;
		}

		private void SetBits(ref uint numericKey, int value, int startingBit)
		{
			uint num = (uint)((uint)value << startingBit);
			numericKey |= num;
		}

		public static string GetRandomMountKeyString(ItemObject mountItem, int randomSeed)
		{
			return MountCreationKey.GetRandomMountKey(mountItem, randomSeed).ToString();
		}

		public static MountCreationKey GetRandomMountKey(ItemObject mountItem, int randomSeed)
		{
			MBFastRandom mbfastRandom = new MBFastRandom((uint)randomSeed);
			if (mountItem == null)
			{
				return new MountCreationKey((byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), 0, 0);
			}
			HorseComponent horseComponent = mountItem.HorseComponent;
			if (horseComponent.HorseMaterialNames != null && horseComponent.HorseMaterialNames.Count > 0)
			{
				int num = mbfastRandom.Next(horseComponent.HorseMaterialNames.Count);
				float num2 = mbfastRandom.NextFloat();
				int num3 = 0;
				float num4 = 0f;
				HorseComponent.MaterialProperty materialProperty = horseComponent.HorseMaterialNames[num];
				for (int i = 0; i < materialProperty.MeshMultiplier.Count; i++)
				{
					num4 += materialProperty.MeshMultiplier[i].Item2;
					if (num2 <= num4)
					{
						num3 = i;
						break;
					}
				}
				return new MountCreationKey((byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), (byte)num, (byte)num3);
			}
			return new MountCreationKey((byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), (byte)mbfastRandom.Next(4), 0, 0);
		}

		private const int NumLegColors = 4;
	}
}
