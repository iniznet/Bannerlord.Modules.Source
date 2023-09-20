using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Face_generation_params", false)]
	public struct FaceGenerationParams
	{
		public static FaceGenerationParams Create()
		{
			FaceGenerationParams faceGenerationParams;
			faceGenerationParams.Seed = 0;
			faceGenerationParams.CurrentBeard = 0;
			faceGenerationParams.CurrentHair = 0;
			faceGenerationParams.CurrentEyebrow = 0;
			faceGenerationParams.IsHairFlipped = false;
			faceGenerationParams.CurrentRace = 0;
			faceGenerationParams.CurrentGender = 0;
			faceGenerationParams.CurrentFaceTexture = 0;
			faceGenerationParams.CurrentMouthTexture = 0;
			faceGenerationParams.CurrentFaceTattoo = 0;
			faceGenerationParams.CurrentVoice = 0;
			faceGenerationParams.HairFilter = 0;
			faceGenerationParams.BeardFilter = 0;
			faceGenerationParams.TattooFilter = 0;
			faceGenerationParams.FaceTextureFilter = 0;
			faceGenerationParams.TattooZeroProbability = 0f;
			faceGenerationParams.KeyWeights = new float[320];
			faceGenerationParams.CurrentAge = 0f;
			faceGenerationParams.CurrentWeight = 0f;
			faceGenerationParams.CurrentBuild = 0f;
			faceGenerationParams.CurrentSkinColorOffset = 0f;
			faceGenerationParams.CurrentHairColorOffset = 0f;
			faceGenerationParams.CurrentEyeColorOffset = 0f;
			faceGenerationParams.FaceDirtAmount = 0f;
			faceGenerationParams.CurrentFaceTattooColorOffset1 = 0f;
			faceGenerationParams.HeightMultiplier = 0f;
			faceGenerationParams.VoicePitch = 0f;
			faceGenerationParams.UseCache = false;
			faceGenerationParams.UseGpuMorph = false;
			faceGenerationParams.Padding2 = false;
			return faceGenerationParams;
		}

		public void SetRaceGenderAndAdjustParams(int race, int gender, int curAge)
		{
			this.CurrentGender = gender;
			this.CurrentRace = race;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			float num8 = 0f;
			MBBodyProperties.GetParamsMax(race, gender, curAge, ref num, ref num2, ref num3, ref num4, ref num7, ref num6, ref num5, ref num8);
			this.CurrentHair = MBMath.ClampInt(this.CurrentHair, 0, num - 1);
			this.CurrentBeard = MBMath.ClampInt(this.CurrentBeard, 0, num2 - 1);
			this.CurrentFaceTexture = MBMath.ClampInt(this.CurrentFaceTexture, 0, num3 - 1);
			this.CurrentMouthTexture = MBMath.ClampInt(this.CurrentMouthTexture, 0, num4 - 1);
			this.CurrentFaceTattoo = MBMath.ClampInt(this.CurrentFaceTattoo, 0, num7 - 1);
			this.CurrentVoice = MBMath.ClampInt(this.CurrentVoice, 0, num6 - 1);
			this.VoicePitch = MBMath.ClampFloat(this.VoicePitch, 0f, 1f);
			this.CurrentEyebrow = MBMath.ClampInt(this.CurrentEyebrow, 0, num5 - 1);
		}

		public void SetRandomParamsExceptKeys(int race, int gender, int minAge, out float scale)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			scale = 0f;
			MBBodyProperties.GetParamsMax(race, gender, minAge, ref num, ref num2, ref num3, ref num4, ref num7, ref num6, ref num5, ref scale);
			this.CurrentHair = MBRandom.RandomInt(num);
			this.CurrentBeard = MBRandom.RandomInt(num2);
			this.CurrentFaceTexture = MBRandom.RandomInt(num3);
			this.CurrentMouthTexture = MBRandom.RandomInt(num4);
			this.CurrentFaceTattoo = MBRandom.RandomInt(num7);
			this.CurrentVoice = MBRandom.RandomInt(num6);
			this.VoicePitch = MBRandom.RandomFloat;
			this.CurrentEyebrow = MBRandom.RandomInt(num5);
			this.CurrentSkinColorOffset = MBRandom.RandomFloat;
			this.CurrentHairColorOffset = MBRandom.RandomFloat;
			this.CurrentEyeColorOffset = MBRandom.RandomFloat;
			this.CurrentFaceTattooColorOffset1 = MBRandom.RandomFloat;
			this.HeightMultiplier = MBRandom.RandomFloat;
		}

		public int Seed;

		public int CurrentBeard;

		public int CurrentHair;

		public int CurrentEyebrow;

		public int CurrentRace;

		public int CurrentGender;

		public int CurrentFaceTexture;

		public int CurrentMouthTexture;

		public int CurrentFaceTattoo;

		public int CurrentVoice;

		public int HairFilter;

		public int BeardFilter;

		public int TattooFilter;

		public int FaceTextureFilter;

		public float TattooZeroProbability;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 320)]
		public float[] KeyWeights;

		public float CurrentAge;

		public float CurrentWeight;

		public float CurrentBuild;

		public float CurrentSkinColorOffset;

		public float CurrentHairColorOffset;

		public float CurrentEyeColorOffset;

		public float FaceDirtAmount;

		[CustomEngineStructMemberData("current_face_tattoo_color_offset_1")]
		public float CurrentFaceTattooColorOffset1;

		public float HeightMultiplier;

		public float VoicePitch;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsHairFlipped;

		[MarshalAs(UnmanagedType.U1)]
		public bool UseCache;

		[MarshalAs(UnmanagedType.U1)]
		public bool UseGpuMorph;

		[MarshalAs(UnmanagedType.U1)]
		public bool Padding2;
	}
}
