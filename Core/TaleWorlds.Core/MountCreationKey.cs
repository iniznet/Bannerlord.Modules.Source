using System;
using System.Globalization;

namespace TaleWorlds.Core
{
	// Token: 0x020000B5 RID: 181
	public class MountCreationKey
	{
		// Token: 0x17000312 RID: 786
		// (get) Token: 0x0600092D RID: 2349 RVA: 0x0001EA3A File Offset: 0x0001CC3A
		// (set) Token: 0x0600092E RID: 2350 RVA: 0x0001EA42 File Offset: 0x0001CC42
		public byte _leftFrontLegColorIndex { get; private set; }

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x0001EA4B File Offset: 0x0001CC4B
		// (set) Token: 0x06000930 RID: 2352 RVA: 0x0001EA53 File Offset: 0x0001CC53
		public byte _rightFrontLegColorIndex { get; private set; }

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x0001EA5C File Offset: 0x0001CC5C
		// (set) Token: 0x06000932 RID: 2354 RVA: 0x0001EA64 File Offset: 0x0001CC64
		public byte _leftBackLegColorIndex { get; private set; }

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000933 RID: 2355 RVA: 0x0001EA6D File Offset: 0x0001CC6D
		// (set) Token: 0x06000934 RID: 2356 RVA: 0x0001EA75 File Offset: 0x0001CC75
		public byte _rightBackLegColorIndex { get; private set; }

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000935 RID: 2357 RVA: 0x0001EA7E File Offset: 0x0001CC7E
		// (set) Token: 0x06000936 RID: 2358 RVA: 0x0001EA86 File Offset: 0x0001CC86
		public byte MaterialIndex { get; private set; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x0001EA8F File Offset: 0x0001CC8F
		// (set) Token: 0x06000938 RID: 2360 RVA: 0x0001EA97 File Offset: 0x0001CC97
		public byte MeshMultiplierIndex { get; private set; }

		// Token: 0x06000939 RID: 2361 RVA: 0x0001EAA0 File Offset: 0x0001CCA0
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

		// Token: 0x0600093A RID: 2362 RVA: 0x0001EAF0 File Offset: 0x0001CCF0
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

		// Token: 0x0600093B RID: 2363 RVA: 0x0001EB64 File Offset: 0x0001CD64
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

		// Token: 0x0600093C RID: 2364 RVA: 0x0001EBDC File Offset: 0x0001CDDC
		private static int GetBitsFromKey(uint numericKey, int startingBit, int numBits)
		{
			int num = (int)(numericKey >> startingBit);
			uint num2 = (uint)(numBits * numBits - 1);
			return num & (int)num2;
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0001EBF8 File Offset: 0x0001CDF8
		private void SetBits(ref uint numericKey, int value, int startingBit)
		{
			uint num = (uint)((uint)value << startingBit);
			numericKey |= num;
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0001EC14 File Offset: 0x0001CE14
		public static string GetRandomMountKeyString(ItemObject mountItem, int randomSeed)
		{
			return MountCreationKey.GetRandomMountKey(mountItem, randomSeed).ToString();
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0001EC24 File Offset: 0x0001CE24
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

		// Token: 0x0400054B RID: 1355
		private const int NumLegColors = 4;
	}
}
