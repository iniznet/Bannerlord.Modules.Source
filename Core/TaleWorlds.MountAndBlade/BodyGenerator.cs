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
			faceGenerationParams.CurrentRace = this.Character.Race;
			faceGenerationParams.CurrentGender = (this.Character.IsFemale ? 1 : 0);
			faceGenerationParams.CurrentAge = this.Character.Age;
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, this.CurrentBodyProperties, isDressed && this.Character.Equipment.EarsAreHidden, isDressed && this.Character.Equipment.MouthIsHidden);
			faceGenerationParams.SetRaceGenderAndAdjustParams(faceGenerationParams.CurrentRace, faceGenerationParams.CurrentGender, (int)faceGenerationParams.CurrentAge);
			return faceGenerationParams;
		}

		public void RefreshFace(FaceGenerationParams faceGenerationParams, bool hasEquipment)
		{
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, hasEquipment && this.Character.Equipment.EarsAreHidden, hasEquipment && this.Character.Equipment.MouthIsHidden, ref this.CurrentBodyProperties);
			this.Race = faceGenerationParams.CurrentRace;
			this.IsFemale = faceGenerationParams.CurrentGender == 1;
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
