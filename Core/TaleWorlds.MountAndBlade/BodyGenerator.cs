using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D3 RID: 467
	public class BodyGenerator
	{
		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06001A45 RID: 6725 RVA: 0x0005CB70 File Offset: 0x0005AD70
		// (set) Token: 0x06001A46 RID: 6726 RVA: 0x0005CB78 File Offset: 0x0005AD78
		public BasicCharacterObject Character { get; private set; }

		// Token: 0x06001A47 RID: 6727 RVA: 0x0005CB84 File Offset: 0x0005AD84
		public BodyGenerator(BasicCharacterObject troop)
		{
			this.Character = troop;
			MBDebug.Print("FaceGen set character> character face key: " + troop.GetBodyProperties(troop.Equipment, -1), 0, Debug.DebugColor.White, 17592186044416UL);
			this.Race = this.Character.Race;
			this.IsFemale = this.Character.IsFemale;
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x0005CBF0 File Offset: 0x0005ADF0
		public FaceGenerationParams InitBodyGenerator(bool isDressed)
		{
			this.CurrentBodyProperties = this.Character.GetBodyProperties(this.Character.Equipment, -1);
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			faceGenerationParams._currentRace = this.Character.Race;
			faceGenerationParams._currentGender = (this.Character.IsFemale ? 1 : 0);
			faceGenerationParams._curAge = this.Character.Age;
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, this.CurrentBodyProperties, isDressed && this.Character.Equipment.EarsAreHidden, isDressed && this.Character.Equipment.MouthIsHidden);
			faceGenerationParams.SetRaceGenderAndAdjustParams(faceGenerationParams._currentRace, faceGenerationParams._currentGender, (int)faceGenerationParams._curAge);
			return faceGenerationParams;
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x0005CCB0 File Offset: 0x0005AEB0
		public void RefreshFace(FaceGenerationParams faceGenerationParams, bool hasEquipment)
		{
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, hasEquipment && this.Character.Equipment.EarsAreHidden, hasEquipment && this.Character.Equipment.MouthIsHidden, ref this.CurrentBodyProperties);
			this.Race = faceGenerationParams._currentRace;
			this.IsFemale = faceGenerationParams._currentGender == 1;
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x0005CD10 File Offset: 0x0005AF10
		public void SaveCurrentCharacter()
		{
			this.Character.UpdatePlayerCharacterBodyProperties(this.CurrentBodyProperties, this.Race, this.IsFemale);
		}

		// Token: 0x0400085C RID: 2140
		public const string FaceGenTeethAnimationName = "facegen_teeth";

		// Token: 0x0400085D RID: 2141
		public BodyProperties CurrentBodyProperties;

		// Token: 0x0400085E RID: 2142
		public BodyProperties BodyPropertiesMin;

		// Token: 0x0400085F RID: 2143
		public BodyProperties BodyPropertiesMax;

		// Token: 0x04000860 RID: 2144
		public int Race;

		// Token: 0x04000861 RID: 2145
		public bool IsFemale;
	}
}
