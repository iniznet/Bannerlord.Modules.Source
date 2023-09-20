using System;
using System.Diagnostics;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000049 RID: 73
	public static class GameEntityPhysicsExtensions
	{
		// Token: 0x0600067C RID: 1660 RVA: 0x000048AE File Offset: 0x00002AAE
		[Conditional("_RGL_KEEP_ASSERTS")]
		private static void AssertSingleThreadRead()
		{
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x000048B0 File Offset: 0x00002AB0
		[Conditional("_RGL_KEEP_ASSERTS")]
		private static void AssertSingleThreadWrite()
		{
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x000048B2 File Offset: 0x00002AB2
		[Conditional("_RGL_KEEP_ASSERTS")]
		private static void AssertMultiThreadRead()
		{
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x000048B4 File Offset: 0x00002AB4
		[Conditional("_RGL_KEEP_ASSERTS")]
		private static void AssertMultiThreadWrite()
		{
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x000048B6 File Offset: 0x00002AB6
		public static bool HasBody(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.HasBody(gameEntity.Pointer);
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x000048C8 File Offset: 0x00002AC8
		public static void AddSphereAsBody(this GameEntity gameEntity, Vec3 sphere, float radius, BodyFlags bodyFlags)
		{
			EngineApplicationInterface.IGameEntity.AddSphereAsBody(gameEntity.Pointer, sphere, radius, (uint)bodyFlags);
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x000048DD File Offset: 0x00002ADD
		public static void RemovePhysics(this GameEntity gameEntity, bool clearingTheScene = false)
		{
			EngineApplicationInterface.IGameEntity.RemovePhysics(gameEntity.Pointer, clearingTheScene);
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x000048F0 File Offset: 0x00002AF0
		public static void RemovePhysicsMT(this GameEntity gameEntity, bool clearingTheScene = false)
		{
			EngineApplicationInterface.IGameEntity.RemovePhysics(gameEntity.Pointer, clearingTheScene);
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x00004903 File Offset: 0x00002B03
		public static bool GetPhysicsState(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.GetPhysicsState(gameEntity.Pointer);
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x00004915 File Offset: 0x00002B15
		public static void AddDistanceJoint(this GameEntity gameEntity, GameEntity otherGameEntity, float minDistance, float maxDistance)
		{
			EngineApplicationInterface.IGameEntity.AddDistanceJoint(gameEntity.Pointer, otherGameEntity.Pointer, minDistance, maxDistance);
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0000492F File Offset: 0x00002B2F
		public static bool HasPhysicsDefinitionWithoutFlags(this GameEntity gameEntity, int excludeFlags)
		{
			return EngineApplicationInterface.IGameEntity.HasPhysicsDefinition(gameEntity.Pointer, excludeFlags);
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x00004942 File Offset: 0x00002B42
		public static void SetPhysicsState(this GameEntity gameEntity, bool isEnabled, bool setChildren)
		{
			EngineApplicationInterface.IGameEntity.SetPhysicsState(gameEntity.Pointer, isEnabled, setChildren);
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x00004956 File Offset: 0x00002B56
		public static void RemoveEnginePhysics(this GameEntity gameEntity)
		{
			EngineApplicationInterface.IGameEntity.RemoveEnginePhysics(gameEntity.Pointer);
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x00004968 File Offset: 0x00002B68
		public static bool IsEngineBodySleeping(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.IsEngineBodySleeping(gameEntity.Pointer);
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x0000497A File Offset: 0x00002B7A
		public static bool IsDynamicBodyStationary(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.IsDynamicBodyStationary(gameEntity.Pointer);
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0000498C File Offset: 0x00002B8C
		public static bool IsDynamicBodyStationaryMT(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.IsDynamicBodyStationary(gameEntity.Pointer);
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0000499E File Offset: 0x00002B9E
		public static PhysicsShape GetBodyShape(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.GetBodyShape(gameEntity);
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x000049AB File Offset: 0x00002BAB
		public static void SetBodyShape(this GameEntity gameEntity, PhysicsShape shape)
		{
			EngineApplicationInterface.IGameEntity.SetBodyShape(gameEntity.Pointer, (shape == null) ? ((UIntPtr)0UL) : shape.Pointer);
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x000049D8 File Offset: 0x00002BD8
		public static void AddPhysics(this GameEntity gameEntity, float mass, Vec3 localCenterOfMass, PhysicsShape body, Vec3 initialVelocity, Vec3 angularVelocity, PhysicsMaterial physicsMaterial, bool isStatic, int collisionGroupID)
		{
			EngineApplicationInterface.IGameEntity.AddPhysics(gameEntity.Pointer, (body != null) ? body.Pointer : UIntPtr.Zero, mass, ref localCenterOfMass, ref initialVelocity, ref angularVelocity, physicsMaterial.Index, isStatic, collisionGroupID);
			gameEntity.BodyFlag |= BodyFlags.Moveable;
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x00004A2C File Offset: 0x00002C2C
		public static void ApplyLocalImpulseToDynamicBody(this GameEntity gameEntity, Vec3 localPosition, Vec3 impulse)
		{
			EngineApplicationInterface.IGameEntity.ApplyLocalImpulseToDynamicBody(gameEntity.Pointer, ref localPosition, ref impulse);
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x00004A42 File Offset: 0x00002C42
		public static void ApplyForceToDynamicBody(this GameEntity gameEntity, Vec3 force)
		{
			EngineApplicationInterface.IGameEntity.ApplyForceToDynamicBody(gameEntity.Pointer, ref force);
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x00004A56 File Offset: 0x00002C56
		public static void ApplyLocalForceToDynamicBody(this GameEntity gameEntity, Vec3 localPosition, Vec3 force)
		{
			EngineApplicationInterface.IGameEntity.ApplyLocalForceToDynamicBody(gameEntity.Pointer, ref localPosition, ref force);
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x00004A6C File Offset: 0x00002C6C
		public static void ApplyAccelerationToDynamicBody(this GameEntity gameEntity, Vec3 acceleration)
		{
			EngineApplicationInterface.IGameEntity.ApplyAccelerationToDynamicBody(gameEntity.Pointer, ref acceleration);
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x00004A80 File Offset: 0x00002C80
		public static void DisableDynamicBodySimulation(this GameEntity gameEntity)
		{
			EngineApplicationInterface.IGameEntity.DisableDynamicBodySimulation(gameEntity.Pointer);
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x00004A92 File Offset: 0x00002C92
		public static void DisableDynamicBodySimulationMT(this GameEntity gameEntity)
		{
			EngineApplicationInterface.IGameEntity.DisableDynamicBodySimulation(gameEntity.Pointer);
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00004AA4 File Offset: 0x00002CA4
		public static void EnableDynamicBody(this GameEntity gameEntity)
		{
			EngineApplicationInterface.IGameEntity.EnableDynamicBody(gameEntity.Pointer);
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00004AB6 File Offset: 0x00002CB6
		public static float GetMass(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.GetMass(gameEntity.Pointer);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00004AC8 File Offset: 0x00002CC8
		public static void SetMass(this GameEntity gameEntity, float mass)
		{
			EngineApplicationInterface.IGameEntity.SetMass(gameEntity.Pointer, mass);
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00004ADB File Offset: 0x00002CDB
		public static void SetMassSpaceInertia(this GameEntity gameEntity, Vec3 inertia)
		{
			EngineApplicationInterface.IGameEntity.SetMassSpaceInertia(gameEntity.Pointer, ref inertia);
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x00004AEF File Offset: 0x00002CEF
		public static void SetDamping(this GameEntity gameEntity, float linearDamping, float angularDamping)
		{
			EngineApplicationInterface.IGameEntity.SetDamping(gameEntity.Pointer, linearDamping, angularDamping);
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x00004B03 File Offset: 0x00002D03
		public static void DisableGravity(this GameEntity gameEntity)
		{
			EngineApplicationInterface.IGameEntity.DisableGravity(gameEntity.Pointer);
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x00004B15 File Offset: 0x00002D15
		public static Vec3 GetLinearVelocity(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.GetLinearVelocity(gameEntity.Pointer);
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x00004B27 File Offset: 0x00002D27
		public static Vec3 GetLinearVelocityMT(this GameEntity gameEntity)
		{
			return EngineApplicationInterface.IGameEntity.GetLinearVelocity(gameEntity.Pointer);
		}
	}
}
