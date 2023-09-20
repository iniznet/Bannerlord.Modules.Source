using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000205 RID: 517
	[EngineStruct("Skin_generation_params")]
	public struct SkinGenerationParams
	{
		// Token: 0x06001C9B RID: 7323 RVA: 0x00065AF8 File Offset: 0x00063CF8
		public static SkinGenerationParams Create()
		{
			SkinGenerationParams skinGenerationParams;
			skinGenerationParams._skinMeshesVisibilityMask = 481;
			skinGenerationParams._underwearType = Equipment.UnderwearTypes.FullUnderwear;
			skinGenerationParams._bodyMeshType = 0;
			skinGenerationParams._hairCoverType = 0;
			skinGenerationParams._beardCoverType = 0;
			skinGenerationParams._prepareImmediately = false;
			skinGenerationParams._bodyDeformType = -1;
			skinGenerationParams._faceDirtAmount = 0f;
			skinGenerationParams._gender = 0;
			skinGenerationParams._race = 0;
			skinGenerationParams._useTranslucency = false;
			skinGenerationParams._useTesselation = false;
			return skinGenerationParams;
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x00065B70 File Offset: 0x00063D70
		public SkinGenerationParams(int skinMeshesVisibilityMask, Equipment.UnderwearTypes underwearType, int bodyMeshType, int hairCoverType, int beardCoverType, int bodyDeformType, bool prepareImmediately, float faceDirtAmount, int gender, int race, bool useTranslucency, bool useTesselation)
		{
			this._skinMeshesVisibilityMask = skinMeshesVisibilityMask;
			this._underwearType = underwearType;
			this._bodyMeshType = bodyMeshType;
			this._hairCoverType = hairCoverType;
			this._beardCoverType = beardCoverType;
			this._bodyDeformType = bodyDeformType;
			this._prepareImmediately = prepareImmediately;
			this._faceDirtAmount = faceDirtAmount;
			this._gender = gender;
			this._race = race;
			this._useTranslucency = useTranslucency;
			this._useTesselation = useTesselation;
		}

		// Token: 0x0400094B RID: 2379
		public int _skinMeshesVisibilityMask;

		// Token: 0x0400094C RID: 2380
		public Equipment.UnderwearTypes _underwearType;

		// Token: 0x0400094D RID: 2381
		public int _bodyMeshType;

		// Token: 0x0400094E RID: 2382
		public int _hairCoverType;

		// Token: 0x0400094F RID: 2383
		public int _beardCoverType;

		// Token: 0x04000950 RID: 2384
		public int _bodyDeformType;

		// Token: 0x04000951 RID: 2385
		public bool _prepareImmediately;

		// Token: 0x04000952 RID: 2386
		public bool _useTranslucency;

		// Token: 0x04000953 RID: 2387
		public bool _useTesselation;

		// Token: 0x04000954 RID: 2388
		public float _faceDirtAmount;

		// Token: 0x04000955 RID: 2389
		public int _gender;

		// Token: 0x04000956 RID: 2390
		public int _race;
	}
}
