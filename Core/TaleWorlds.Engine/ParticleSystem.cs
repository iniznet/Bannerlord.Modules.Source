using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200006D RID: 109
	[EngineClass("rglParticle_system_instanced")]
	public sealed class ParticleSystem : GameEntityComponent
	{
		// Token: 0x06000880 RID: 2176 RVA: 0x0000876B File Offset: 0x0000696B
		internal ParticleSystem(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x00008774 File Offset: 0x00006974
		public static ParticleSystem CreateParticleSystemAttachedToBone(string systemName, Skeleton skeleton, sbyte boneIndex, ref MatrixFrame boneLocalFrame)
		{
			return ParticleSystem.CreateParticleSystemAttachedToBone(ParticleSystemManager.GetRuntimeIdByName(systemName), skeleton, boneIndex, ref boneLocalFrame);
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x00008784 File Offset: 0x00006984
		public static ParticleSystem CreateParticleSystemAttachedToBone(int systemRuntimeId, Skeleton skeleton, sbyte boneIndex, ref MatrixFrame boneLocalFrame)
		{
			return EngineApplicationInterface.IParticleSystem.CreateParticleSystemAttachedToBone(systemRuntimeId, skeleton.Pointer, boneIndex, ref boneLocalFrame);
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00008799 File Offset: 0x00006999
		public static ParticleSystem CreateParticleSystemAttachedToEntity(string systemName, GameEntity parentEntity, ref MatrixFrame boneLocalFrame)
		{
			return ParticleSystem.CreateParticleSystemAttachedToEntity(ParticleSystemManager.GetRuntimeIdByName(systemName), parentEntity, ref boneLocalFrame);
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x000087A8 File Offset: 0x000069A8
		public static ParticleSystem CreateParticleSystemAttachedToEntity(int systemRuntimeId, GameEntity parentEntity, ref MatrixFrame boneLocalFrame)
		{
			return EngineApplicationInterface.IParticleSystem.CreateParticleSystemAttachedToEntity(systemRuntimeId, parentEntity.Pointer, ref boneLocalFrame);
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x000087BC File Offset: 0x000069BC
		public void AddMesh(Mesh mesh)
		{
			EngineApplicationInterface.IMetaMesh.AddMesh(base.Pointer, mesh.Pointer, 0U);
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x000087D5 File Offset: 0x000069D5
		public void SetEnable(bool enable)
		{
			EngineApplicationInterface.IParticleSystem.SetEnable(base.Pointer, enable);
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x000087E8 File Offset: 0x000069E8
		public void SetRuntimeEmissionRateMultiplier(float multiplier)
		{
			EngineApplicationInterface.IParticleSystem.SetRuntimeEmissionRateMultiplier(base.Pointer, multiplier);
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x000087FB File Offset: 0x000069FB
		public void Restart()
		{
			EngineApplicationInterface.IParticleSystem.Restart(base.Pointer);
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x0000880D File Offset: 0x00006A0D
		public void SetLocalFrame(ref MatrixFrame newLocalFrame)
		{
			EngineApplicationInterface.IParticleSystem.SetLocalFrame(base.Pointer, ref newLocalFrame);
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x00008820 File Offset: 0x00006A20
		public MatrixFrame GetLocalFrame()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			EngineApplicationInterface.IParticleSystem.GetLocalFrame(base.Pointer, ref identity);
			return identity;
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x00008846 File Offset: 0x00006A46
		public void SetParticleEffectByName(string effectName)
		{
			EngineApplicationInterface.IParticleSystem.SetParticleEffectByName(base.Pointer, effectName);
		}
	}
}
