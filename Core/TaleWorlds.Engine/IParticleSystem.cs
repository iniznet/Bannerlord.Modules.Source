using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200001A RID: 26
	[ApplicationInterfaceBase]
	internal interface IParticleSystem
	{
		// Token: 0x0600012D RID: 301
		[EngineMethod("set_enable", false)]
		void SetEnable(UIntPtr psysPointer, bool enable);

		// Token: 0x0600012E RID: 302
		[EngineMethod("set_runtime_emission_rate_multiplier", false)]
		void SetRuntimeEmissionRateMultiplier(UIntPtr pointer, float multiplier);

		// Token: 0x0600012F RID: 303
		[EngineMethod("restart", false)]
		void Restart(UIntPtr psysPointer);

		// Token: 0x06000130 RID: 304
		[EngineMethod("set_local_frame", false)]
		void SetLocalFrame(UIntPtr pointer, ref MatrixFrame newFrame);

		// Token: 0x06000131 RID: 305
		[EngineMethod("get_local_frame", false)]
		void GetLocalFrame(UIntPtr pointer, ref MatrixFrame frame);

		// Token: 0x06000132 RID: 306
		[EngineMethod("get_runtime_id_by_name", false)]
		int GetRuntimeIdByName(string particleSystemName);

		// Token: 0x06000133 RID: 307
		[EngineMethod("create_particle_system_attached_to_bone", false)]
		ParticleSystem CreateParticleSystemAttachedToBone(int runtimeId, UIntPtr skeletonPtr, sbyte boneIndex, ref MatrixFrame boneLocalFrame);

		// Token: 0x06000134 RID: 308
		[EngineMethod("create_particle_system_attached_to_entity", false)]
		ParticleSystem CreateParticleSystemAttachedToEntity(int runtimeId, UIntPtr entityPtr, ref MatrixFrame boneLocalFrame);

		// Token: 0x06000135 RID: 309
		[EngineMethod("set_particle_effect_by_name", false)]
		void SetParticleEffectByName(UIntPtr pointer, string effectName);
	}
}
