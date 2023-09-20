using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Face_generation_params")]
	public struct FaceGenerationParams
	{
		public static FaceGenerationParams Create()
		{
			FaceGenerationParams faceGenerationParams;
			faceGenerationParams.seed_ = 0;
			faceGenerationParams._curBeard = 0;
			faceGenerationParams._currentHair = 0;
			faceGenerationParams._curEyebrow = 0;
			faceGenerationParams._isHairFlipped = false;
			faceGenerationParams._currentRace = 0;
			faceGenerationParams._currentGender = 0;
			faceGenerationParams._curFaceTexture = 0;
			faceGenerationParams._curMouthTexture = 0;
			faceGenerationParams._curFaceTattoo = 0;
			faceGenerationParams._currentVoice = 0;
			faceGenerationParams.hair_filter_ = 0;
			faceGenerationParams.beard_filter_ = 0;
			faceGenerationParams.tattoo_filter_ = 0;
			faceGenerationParams.face_texture_filter_ = 0;
			faceGenerationParams._tattooZeroProbability = 0f;
			faceGenerationParams.KeyWeights = new float[320];
			faceGenerationParams._curAge = 0f;
			faceGenerationParams._curWeight = 0f;
			faceGenerationParams._curBuild = 0f;
			faceGenerationParams._curSkinColorOffset = 0f;
			faceGenerationParams._curHairColorOffset = 0f;
			faceGenerationParams._curEyeColorOffset = 0f;
			faceGenerationParams.face_dirt_amount_ = 0f;
			faceGenerationParams._curFaceTattooColorOffset1 = 0f;
			faceGenerationParams._heightMultiplier = 0f;
			faceGenerationParams._voicePitch = 0f;
			faceGenerationParams._useCache = false;
			faceGenerationParams._useGpuMorph = false;
			faceGenerationParams._padding2 = false;
			return faceGenerationParams;
		}

		public void SetRaceGenderAndAdjustParams(int race, int gender, int curAge)
		{
			this._currentGender = gender;
			this._currentRace = race;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			float num8 = 0f;
			MBBodyProperties.GetParamsMax(race, gender, curAge, ref num, ref num2, ref num3, ref num4, ref num7, ref num6, ref num5, ref num8);
			this._currentHair = MBMath.ClampInt(this._currentHair, 0, num - 1);
			this._curBeard = MBMath.ClampInt(this._curBeard, 0, num2 - 1);
			this._curFaceTexture = MBMath.ClampInt(this._curFaceTexture, 0, num3 - 1);
			this._curMouthTexture = MBMath.ClampInt(this._curMouthTexture, 0, num4 - 1);
			this._curFaceTattoo = MBMath.ClampInt(this._curFaceTattoo, 0, num7 - 1);
			this._currentVoice = MBMath.ClampInt(this._currentVoice, 0, num6 - 1);
			this._voicePitch = MBMath.ClampFloat(this._voicePitch, 0f, 1f);
			this._curEyebrow = MBMath.ClampInt(this._curEyebrow, 0, num5 - 1);
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
			this._currentHair = MBRandom.RandomInt(num);
			this._curBeard = MBRandom.RandomInt(num2);
			this._curFaceTexture = MBRandom.RandomInt(num3);
			this._curMouthTexture = MBRandom.RandomInt(num4);
			this._curFaceTattoo = MBRandom.RandomInt(num7);
			this._currentVoice = MBRandom.RandomInt(num6);
			this._voicePitch = MBRandom.RandomFloat;
			this._curEyebrow = MBRandom.RandomInt(num5);
			this._curSkinColorOffset = MBRandom.RandomFloat;
			this._curHairColorOffset = MBRandom.RandomFloat;
			this._curEyeColorOffset = MBRandom.RandomFloat;
			this._curFaceTattooColorOffset1 = MBRandom.RandomFloat;
			this._heightMultiplier = MBRandom.RandomFloat;
		}

		public int seed_;

		public int _curBeard;

		public int _currentHair;

		public int _curEyebrow;

		public int _currentRace;

		public int _currentGender;

		public int _curFaceTexture;

		public int _curMouthTexture;

		public int _curFaceTattoo;

		public int _currentVoice;

		public int hair_filter_;

		public int beard_filter_;

		public int tattoo_filter_;

		public int face_texture_filter_;

		public float _tattooZeroProbability;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 320)]
		public float[] KeyWeights;

		public float _curAge;

		public float _curWeight;

		public float _curBuild;

		public float _curSkinColorOffset;

		public float _curHairColorOffset;

		public float _curEyeColorOffset;

		public float face_dirt_amount_;

		public float _curFaceTattooColorOffset1;

		public float _heightMultiplier;

		public float _voicePitch;

		[MarshalAs(UnmanagedType.I1)]
		public bool _isHairFlipped;

		[MarshalAs(UnmanagedType.I1)]
		public bool _useCache;

		[MarshalAs(UnmanagedType.I1)]
		public bool _useGpuMorph;

		[MarshalAs(UnmanagedType.I1)]
		public bool _padding2;
	}
}
