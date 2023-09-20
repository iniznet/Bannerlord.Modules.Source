using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000206 RID: 518
	[EngineStruct("Face_generation_params")]
	public struct FaceGenerationParams
	{
		// Token: 0x06001C9D RID: 7325 RVA: 0x00065BDC File Offset: 0x00063DDC
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

		// Token: 0x06001C9E RID: 7326 RVA: 0x00065D10 File Offset: 0x00063F10
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

		// Token: 0x06001C9F RID: 7327 RVA: 0x00065E0C File Offset: 0x0006400C
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

		// Token: 0x04000957 RID: 2391
		public int seed_;

		// Token: 0x04000958 RID: 2392
		public int _curBeard;

		// Token: 0x04000959 RID: 2393
		public int _currentHair;

		// Token: 0x0400095A RID: 2394
		public int _curEyebrow;

		// Token: 0x0400095B RID: 2395
		public int _currentRace;

		// Token: 0x0400095C RID: 2396
		public int _currentGender;

		// Token: 0x0400095D RID: 2397
		public int _curFaceTexture;

		// Token: 0x0400095E RID: 2398
		public int _curMouthTexture;

		// Token: 0x0400095F RID: 2399
		public int _curFaceTattoo;

		// Token: 0x04000960 RID: 2400
		public int _currentVoice;

		// Token: 0x04000961 RID: 2401
		public int hair_filter_;

		// Token: 0x04000962 RID: 2402
		public int beard_filter_;

		// Token: 0x04000963 RID: 2403
		public int tattoo_filter_;

		// Token: 0x04000964 RID: 2404
		public int face_texture_filter_;

		// Token: 0x04000965 RID: 2405
		public float _tattooZeroProbability;

		// Token: 0x04000966 RID: 2406
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 320)]
		public float[] KeyWeights;

		// Token: 0x04000967 RID: 2407
		public float _curAge;

		// Token: 0x04000968 RID: 2408
		public float _curWeight;

		// Token: 0x04000969 RID: 2409
		public float _curBuild;

		// Token: 0x0400096A RID: 2410
		public float _curSkinColorOffset;

		// Token: 0x0400096B RID: 2411
		public float _curHairColorOffset;

		// Token: 0x0400096C RID: 2412
		public float _curEyeColorOffset;

		// Token: 0x0400096D RID: 2413
		public float face_dirt_amount_;

		// Token: 0x0400096E RID: 2414
		public float _curFaceTattooColorOffset1;

		// Token: 0x0400096F RID: 2415
		public float _heightMultiplier;

		// Token: 0x04000970 RID: 2416
		public float _voicePitch;

		// Token: 0x04000971 RID: 2417
		[MarshalAs(UnmanagedType.I1)]
		public bool _isHairFlipped;

		// Token: 0x04000972 RID: 2418
		[MarshalAs(UnmanagedType.I1)]
		public bool _useCache;

		// Token: 0x04000973 RID: 2419
		[MarshalAs(UnmanagedType.I1)]
		public bool _useGpuMorph;

		// Token: 0x04000974 RID: 2420
		[MarshalAs(UnmanagedType.I1)]
		public bool _padding2;
	}
}
