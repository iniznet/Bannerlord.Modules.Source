using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BodyGenerator
	{
		public BasicCharacterObject Character { get; private set; }

		public BodyGenerator(BasicCharacterObject troop)
		{
			this.Character = troop;
			MBDebug.Print("FaceGen set character> character face key: " + troop.GetBodyProperties(troop.Equipment, -1), 0, Debug.DebugColor.White, 17592186044416UL);
			this.Race = this.Character.Race;
			this.IsFemale = this.Character.IsFemale;
		}

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

		public void RefreshFace(FaceGenerationParams faceGenerationParams, bool hasEquipment)
		{
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, hasEquipment && this.Character.Equipment.EarsAreHidden, hasEquipment && this.Character.Equipment.MouthIsHidden, ref this.CurrentBodyProperties);
			this.Race = faceGenerationParams._currentRace;
			this.IsFemale = faceGenerationParams._currentGender == 1;
		}

		public void SaveCurrentCharacter()
		{
			this.Character.UpdatePlayerCharacterBodyProperties(this.CurrentBodyProperties, this.Race, this.IsFemale);
		}

		public const string FaceGenTeethAnimationName = "facegen_teeth";

		public BodyProperties CurrentBodyProperties;

		public BodyProperties BodyPropertiesMin;

		public BodyProperties BodyPropertiesMax;

		public int Race;

		public bool IsFemale;
	}
}
