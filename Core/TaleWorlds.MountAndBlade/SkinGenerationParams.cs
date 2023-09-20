using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Skin_generation_params")]
	public struct SkinGenerationParams
	{
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

		public int _skinMeshesVisibilityMask;

		public Equipment.UnderwearTypes _underwearType;

		public int _bodyMeshType;

		public int _hairCoverType;

		public int _beardCoverType;

		public int _bodyDeformType;

		public bool _prepareImmediately;

		public bool _useTranslucency;

		public bool _useTesselation;

		public float _faceDirtAmount;

		public int _gender;

		public int _race;
	}
}
