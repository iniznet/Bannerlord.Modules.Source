using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineClass("rglParticle_system_instanced")]
	public sealed class ParticleSystem : GameEntityComponent
	{
		internal ParticleSystem(UIntPtr pointer)
			: base(pointer)
		{
		}

		public static ParticleSystem CreateParticleSystemAttachedToBone(string systemName, Skeleton skeleton, sbyte boneIndex, ref MatrixFrame boneLocalFrame)
		{
			return ParticleSystem.CreateParticleSystemAttachedToBone(ParticleSystemManager.GetRuntimeIdByName(systemName), skeleton, boneIndex, ref boneLocalFrame);
		}

		public static ParticleSystem CreateParticleSystemAttachedToBone(int systemRuntimeId, Skeleton skeleton, sbyte boneIndex, ref MatrixFrame boneLocalFrame)
		{
			return EngineApplicationInterface.IParticleSystem.CreateParticleSystemAttachedToBone(systemRuntimeId, skeleton.Pointer, boneIndex, ref boneLocalFrame);
		}

		public static ParticleSystem CreateParticleSystemAttachedToEntity(string systemName, GameEntity parentEntity, ref MatrixFrame boneLocalFrame)
		{
			return ParticleSystem.CreateParticleSystemAttachedToEntity(ParticleSystemManager.GetRuntimeIdByName(systemName), parentEntity, ref boneLocalFrame);
		}

		public static ParticleSystem CreateParticleSystemAttachedToEntity(int systemRuntimeId, GameEntity parentEntity, ref MatrixFrame boneLocalFrame)
		{
			return EngineApplicationInterface.IParticleSystem.CreateParticleSystemAttachedToEntity(systemRuntimeId, parentEntity.Pointer, ref boneLocalFrame);
		}

		public void AddMesh(Mesh mesh)
		{
			EngineApplicationInterface.IMetaMesh.AddMesh(base.Pointer, mesh.Pointer, 0U);
		}

		public void SetEnable(bool enable)
		{
			EngineApplicationInterface.IParticleSystem.SetEnable(base.Pointer, enable);
		}

		public void SetRuntimeEmissionRateMultiplier(float multiplier)
		{
			EngineApplicationInterface.IParticleSystem.SetRuntimeEmissionRateMultiplier(base.Pointer, multiplier);
		}

		public void Restart()
		{
			EngineApplicationInterface.IParticleSystem.Restart(base.Pointer);
		}

		public void SetLocalFrame(ref MatrixFrame newLocalFrame)
		{
			EngineApplicationInterface.IParticleSystem.SetLocalFrame(base.Pointer, ref newLocalFrame);
		}

		public MatrixFrame GetLocalFrame()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			EngineApplicationInterface.IParticleSystem.GetLocalFrame(base.Pointer, ref identity);
			return identity;
		}

		public void SetParticleEffectByName(string effectName)
		{
			EngineApplicationInterface.IParticleSystem.SetParticleEffectByName(base.Pointer, effectName);
		}
	}
}
