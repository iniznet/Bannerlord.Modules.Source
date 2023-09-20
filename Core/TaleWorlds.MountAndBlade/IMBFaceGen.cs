using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A3 RID: 419
	[ScriptingInterfaceBase]
	internal interface IMBFaceGen
	{
		// Token: 0x060016FF RID: 5887
		[EngineMethod("get_num_editable_deform_keys", false)]
		int GetNumEditableDeformKeys(int race, bool initialGender, float age);

		// Token: 0x06001700 RID: 5888
		[EngineMethod("get_params_from_key", false)]
		void GetParamsFromKey(ref FaceGenerationParams faceGenerationParams, ref BodyProperties bodyProperties, bool earsAreHidden, bool mouthHidden);

		// Token: 0x06001701 RID: 5889
		[EngineMethod("get_params_max", false)]
		void GetParamsMax(int race, int curGender, float curAge, ref int hairNum, ref int beardNum, ref int faceTextureNum, ref int mouthTextureNum, ref int faceTattooNum, ref int soundNum, ref int eyebrowNum, ref float scale);

		// Token: 0x06001702 RID: 5890
		[EngineMethod("get_zero_probabilities", false)]
		void GetZeroProbabilities(int race, int curGender, float curAge, ref float tattooZeroProbability);

		// Token: 0x06001703 RID: 5891
		[EngineMethod("produce_numeric_key_with_params", false)]
		void ProduceNumericKeyWithParams(ref FaceGenerationParams faceGenerationParams, bool earsAreHidden, bool mouthIsHidden, ref BodyProperties bodyProperties);

		// Token: 0x06001704 RID: 5892
		[EngineMethod("produce_numeric_key_with_default_values", false)]
		void ProduceNumericKeyWithDefaultValues(ref BodyProperties initialBodyProperties, bool earsAreHidden, bool mouthIsHidden, int race, int gender, float age);

		// Token: 0x06001705 RID: 5893
		[EngineMethod("transform_face_keys_to_default_face", false)]
		void TransformFaceKeysToDefaultFace(ref FaceGenerationParams faceGenerationParams);

		// Token: 0x06001706 RID: 5894
		[EngineMethod("get_random_body_properties", false)]
		void GetRandomBodyProperties(int race, int gender, ref BodyProperties bodyPropertiesMin, ref BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tatooTags, ref BodyProperties outBodyProperties);

		// Token: 0x06001707 RID: 5895
		[EngineMethod("enforce_constraints", false)]
		bool EnforceConstraints(ref FaceGenerationParams faceGenerationParams);

		// Token: 0x06001708 RID: 5896
		[EngineMethod("get_deform_key_data", false)]
		void GetDeformKeyData(int keyNo, ref DeformKeyData deformKeyData, int race, int gender, float age);

		// Token: 0x06001709 RID: 5897
		[EngineMethod("get_face_gen_instances_length", false)]
		int GetFaceGenInstancesLength(int race, int gender, float age);

		// Token: 0x0600170A RID: 5898
		[EngineMethod("get_scale", false)]
		float GetScaleFromKey(int race, int gender, ref BodyProperties initialBodyProperties);

		// Token: 0x0600170B RID: 5899
		[EngineMethod("get_voice_records_count", false)]
		int GetVoiceRecordsCount(int race, int curGender, float age);

		// Token: 0x0600170C RID: 5900
		[EngineMethod("get_hair_color_count", false)]
		int GetHairColorCount(int race, int curGender, float age);

		// Token: 0x0600170D RID: 5901
		[EngineMethod("get_hair_color_gradient_points", false)]
		void GetHairColorGradientPoints(int race, int curGender, float age, Vec3[] colors);

		// Token: 0x0600170E RID: 5902
		[EngineMethod("get_tatoo_color_count", false)]
		int GetTatooColorCount(int race, int curGender, float age);

		// Token: 0x0600170F RID: 5903
		[EngineMethod("get_tatoo_color_gradient_points", false)]
		void GetTatooColorGradientPoints(int race, int curGender, float age, Vec3[] colors);

		// Token: 0x06001710 RID: 5904
		[EngineMethod("get_skin_color_count", false)]
		int GetSkinColorCount(int race, int curGender, float age);

		// Token: 0x06001711 RID: 5905
		[EngineMethod("get_maturity_type", false)]
		int GetMaturityType(float age);

		// Token: 0x06001712 RID: 5906
		[EngineMethod("get_voice_type_usable_for_player_data", false)]
		void GetVoiceTypeUsableForPlayerData(int race, int curGender, float age, bool[] aiArray);

		// Token: 0x06001713 RID: 5907
		[EngineMethod("get_skin_color_gradient_points", false)]
		void GetSkinColorGradientPoints(int race, int curGender, float age, Vec3[] colors);

		// Token: 0x06001714 RID: 5908
		[EngineMethod("get_race_ids", false)]
		string GetRaceIds();
	}
}
