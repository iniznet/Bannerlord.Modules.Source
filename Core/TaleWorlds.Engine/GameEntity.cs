using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000047 RID: 71
	[EngineClass("rglEntity")]
	public sealed class GameEntity : NativeObject
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x00003654 File Offset: 0x00001854
		public Scene Scene
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetScene(base.Pointer);
			}
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00003666 File Offset: 0x00001866
		private GameEntity()
		{
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x0000366E File Offset: 0x0000186E
		internal GameEntity(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x0000367D File Offset: 0x0000187D
		public UIntPtr GetScenePointer()
		{
			return EngineApplicationInterface.IGameEntity.GetScenePointer(base.Pointer);
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00003690 File Offset: 0x00001890
		public override string ToString()
		{
			return base.Pointer.ToString();
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x000036AB File Offset: 0x000018AB
		public void ClearEntityComponents(bool resetAll, bool removeScripts, bool deleteChildEntities)
		{
			EngineApplicationInterface.IGameEntity.ClearEntityComponents(base.Pointer, resetAll, removeScripts, deleteChildEntities);
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x000036C0 File Offset: 0x000018C0
		public void ClearComponents()
		{
			EngineApplicationInterface.IGameEntity.ClearComponents(base.Pointer);
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x000036D2 File Offset: 0x000018D2
		public void ClearOnlyOwnComponents()
		{
			EngineApplicationInterface.IGameEntity.ClearOnlyOwnComponents(base.Pointer);
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x000036E4 File Offset: 0x000018E4
		public bool CheckResources(bool addToQueue, bool checkFaceResources)
		{
			return EngineApplicationInterface.IGameEntity.CheckResources(base.Pointer, addToQueue, checkFaceResources);
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x000036F8 File Offset: 0x000018F8
		public void SetMobility(GameEntity.Mobility mobility)
		{
			EngineApplicationInterface.IGameEntity.SetMobility(base.Pointer, (int)mobility);
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x0000370B File Offset: 0x0000190B
		public void AddMesh(Mesh mesh, bool recomputeBoundingBox = true)
		{
			EngineApplicationInterface.IGameEntity.AddMesh(base.Pointer, mesh.Pointer, recomputeBoundingBox);
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x00003724 File Offset: 0x00001924
		public void AddMultiMeshToSkeleton(MetaMesh metaMesh)
		{
			EngineApplicationInterface.IGameEntity.AddMultiMeshToSkeleton(base.Pointer, metaMesh.Pointer);
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x0000373C File Offset: 0x0000193C
		public void AddMultiMeshToSkeletonBone(MetaMesh metaMesh, sbyte boneIndex)
		{
			EngineApplicationInterface.IGameEntity.AddMultiMeshToSkeletonBone(base.Pointer, metaMesh.Pointer, boneIndex);
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x00003755 File Offset: 0x00001955
		public IEnumerable<Mesh> GetAllMeshesWithTag(string tag)
		{
			List<GameEntity> list = new List<GameEntity>();
			this.GetChildrenRecursive(ref list);
			list.Add(this);
			foreach (GameEntity entity in list)
			{
				int num;
				for (int i = 0; i < entity.MultiMeshComponentCount; i = num + 1)
				{
					MetaMesh multiMesh = entity.GetMetaMesh(i);
					for (int j = 0; j < multiMesh.MeshCount; j = num + 1)
					{
						Mesh meshAtIndex = multiMesh.GetMeshAtIndex(j);
						if (meshAtIndex.HasTag(tag))
						{
							yield return meshAtIndex;
						}
						num = j;
					}
					multiMesh = null;
					num = i;
				}
				for (int i = 0; i < entity.ClothSimulatorComponentCount; i = num + 1)
				{
					ClothSimulatorComponent clothSimulator = entity.GetClothSimulator(i);
					MetaMesh multiMesh = clothSimulator.GetFirstMetaMesh();
					for (int j = 0; j < multiMesh.MeshCount; j = num + 1)
					{
						Mesh meshAtIndex2 = multiMesh.GetMeshAtIndex(j);
						if (meshAtIndex2.HasTag(tag))
						{
							yield return meshAtIndex2;
						}
						num = j;
					}
					multiMesh = null;
					num = i;
				}
				entity = null;
			}
			List<GameEntity>.Enumerator enumerator = default(List<GameEntity>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x0000376C File Offset: 0x0000196C
		public void SetColor(uint color1, uint color2, string meshTag)
		{
			foreach (Mesh mesh in this.GetAllMeshesWithTag(meshTag))
			{
				mesh.Color = color1;
				mesh.Color2 = color2;
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x000037C0 File Offset: 0x000019C0
		public uint GetFactorColor()
		{
			return EngineApplicationInterface.IGameEntity.GetFactorColor(base.Pointer);
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x000037D2 File Offset: 0x000019D2
		public void SetFactorColor(uint color)
		{
			EngineApplicationInterface.IGameEntity.SetFactorColor(base.Pointer, color);
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x000037E5 File Offset: 0x000019E5
		public void SetAsReplayEntity()
		{
			EngineApplicationInterface.IGameEntity.SetAsReplayEntity(base.Pointer);
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x000037F7 File Offset: 0x000019F7
		public void SetClothMaxDistanceMultiplier(float multiplier)
		{
			EngineApplicationInterface.IGameEntity.SetClothMaxDistanceMultiplier(base.Pointer, multiplier);
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x0000380A File Offset: 0x00001A0A
		public void RemoveMultiMeshFromSkeleton(MetaMesh metaMesh)
		{
			EngineApplicationInterface.IGameEntity.RemoveMultiMeshFromSkeleton(base.Pointer, metaMesh.Pointer);
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x00003822 File Offset: 0x00001A22
		public void RemoveMultiMeshFromSkeletonBone(MetaMesh metaMesh, sbyte boneIndex)
		{
			EngineApplicationInterface.IGameEntity.RemoveMultiMeshFromSkeletonBone(base.Pointer, metaMesh.Pointer, boneIndex);
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x0000383B File Offset: 0x00001A3B
		public bool RemoveComponentWithMesh(Mesh mesh)
		{
			return EngineApplicationInterface.IGameEntity.RemoveComponentWithMesh(base.Pointer, mesh.Pointer);
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x00003853 File Offset: 0x00001A53
		public void AddComponent(GameEntityComponent component)
		{
			EngineApplicationInterface.IGameEntity.AddComponent(base.Pointer, component.Pointer);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0000386B File Offset: 0x00001A6B
		public bool HasComponent(GameEntityComponent component)
		{
			return EngineApplicationInterface.IGameEntity.HasComponent(base.Pointer, component.Pointer);
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x00003883 File Offset: 0x00001A83
		public bool RemoveComponent(GameEntityComponent component)
		{
			return EngineApplicationInterface.IGameEntity.RemoveComponent(base.Pointer, component.Pointer);
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0000389B File Offset: 0x00001A9B
		public string GetGuid()
		{
			return EngineApplicationInterface.IGameEntity.GetGuid(base.Pointer);
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x000038AD File Offset: 0x00001AAD
		public bool IsGuidValid()
		{
			return EngineApplicationInterface.IGameEntity.IsGuidValid(base.Pointer);
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x000038BF File Offset: 0x00001ABF
		public void SetEnforcedMaximumLodLevel(int lodLevel)
		{
			EngineApplicationInterface.IGameEntity.SetEnforcedMaximumLodLevel(base.Pointer, lodLevel);
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x000038D2 File Offset: 0x00001AD2
		public float GetLodLevelForDistanceSq(float distSq)
		{
			return EngineApplicationInterface.IGameEntity.GetLodLevelForDistanceSq(base.Pointer, distSq);
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x000038E5 File Offset: 0x00001AE5
		public void GetQuickBoneEntitialFrame(sbyte index, ref MatrixFrame frame)
		{
			EngineApplicationInterface.IGameEntity.GetQuickBoneEntitialFrame(base.Pointer, index, ref frame);
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x000038F9 File Offset: 0x00001AF9
		public void UpdateVisibilityMask()
		{
			EngineApplicationInterface.IGameEntity.UpdateVisibilityMask(base.Pointer);
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x0000390B File Offset: 0x00001B0B
		public static GameEntity CreateEmpty(Scene scene, bool isModifiableFromEditor = true)
		{
			return EngineApplicationInterface.IGameEntity.CreateEmpty(scene.Pointer, isModifiableFromEditor, (UIntPtr)0UL);
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00003925 File Offset: 0x00001B25
		public static GameEntity CreateEmptyDynamic(Scene scene, bool isModifiableFromEditor = true)
		{
			GameEntity gameEntity = GameEntity.CreateEmpty(scene, isModifiableFromEditor);
			gameEntity.SetMobility(GameEntity.Mobility.dynamic);
			return gameEntity;
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x00003935 File Offset: 0x00001B35
		public static GameEntity CreateEmptyWithoutScene()
		{
			return EngineApplicationInterface.IGameEntity.CreateEmptyWithoutScene();
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x00003941 File Offset: 0x00001B41
		public static GameEntity CopyFrom(Scene scene, GameEntity entity)
		{
			return EngineApplicationInterface.IGameEntity.CreateEmpty(scene.Pointer, false, entity.Pointer);
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x0000395A File Offset: 0x00001B5A
		public static GameEntity Instantiate(Scene scene, string prefabName, bool callScriptCallbacks)
		{
			if (scene != null)
			{
				return EngineApplicationInterface.IGameEntity.CreateFromPrefab(scene.Pointer, prefabName, callScriptCallbacks);
			}
			return EngineApplicationInterface.IGameEntity.CreateFromPrefab(new UIntPtr(0U), prefabName, callScriptCallbacks);
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x0000398A File Offset: 0x00001B8A
		public void CallScriptCallbacks()
		{
			EngineApplicationInterface.IGameEntity.CallScriptCallbacks(base.Pointer);
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0000399C File Offset: 0x00001B9C
		public static GameEntity Instantiate(Scene scene, string prefabName, MatrixFrame frame)
		{
			return EngineApplicationInterface.IGameEntity.CreateFromPrefabWithInitialFrame(scene.Pointer, prefabName, ref frame);
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x000039B1 File Offset: 0x00001BB1
		private int ScriptCount
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetScriptComponentCount(base.Pointer);
			}
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x000039C3 File Offset: 0x00001BC3
		public bool IsGhostObject()
		{
			return EngineApplicationInterface.IGameEntity.IsGhostObject(base.Pointer);
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x000039D5 File Offset: 0x00001BD5
		public void CreateAndAddScriptComponent(string name)
		{
			EngineApplicationInterface.IGameEntity.CreateAndAddScriptComponent(base.Pointer, name);
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x000039E8 File Offset: 0x00001BE8
		public static bool PrefabExists(string name)
		{
			return EngineApplicationInterface.IGameEntity.PrefabExists(name);
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x000039F5 File Offset: 0x00001BF5
		public void RemoveScriptComponent(UIntPtr scriptComponent, int removeReason)
		{
			EngineApplicationInterface.IGameEntity.RemoveScriptComponent(base.Pointer, scriptComponent, removeReason);
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00003A09 File Offset: 0x00001C09
		public void SetEntityEnvMapVisibility(bool value)
		{
			EngineApplicationInterface.IGameEntity.SetEntityEnvMapVisibility(base.Pointer, value);
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00003A1C File Offset: 0x00001C1C
		internal ScriptComponentBehavior GetScriptAtIndex(int index)
		{
			return EngineApplicationInterface.IGameEntity.GetScriptComponentAtIndex(base.Pointer, index);
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00003A2F File Offset: 0x00001C2F
		public bool HasScene()
		{
			return EngineApplicationInterface.IGameEntity.HasScene(base.Pointer);
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00003A41 File Offset: 0x00001C41
		public bool HasScriptComponent(string scName)
		{
			return EngineApplicationInterface.IGameEntity.HasScriptComponent(base.Pointer, scName);
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00003A54 File Offset: 0x00001C54
		public IEnumerable<ScriptComponentBehavior> GetScriptComponents()
		{
			int count = this.ScriptCount;
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				yield return this.GetScriptAtIndex(i);
				num = i;
			}
			yield break;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00003A64 File Offset: 0x00001C64
		public IEnumerable<T> GetScriptComponents<T>() where T : ScriptComponentBehavior
		{
			int count = this.ScriptCount;
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				T t;
				if ((t = this.GetScriptAtIndex(i) as T) != null)
				{
					yield return t;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x00003A74 File Offset: 0x00001C74
		public bool HasScriptOfType(Type t)
		{
			return this.GetScriptComponents().Any((ScriptComponentBehavior sc) => sc.GetType().IsAssignableFrom(t));
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00003AA8 File Offset: 0x00001CA8
		public bool HasScriptOfType<T>() where T : ScriptComponentBehavior
		{
			int scriptCount = this.ScriptCount;
			for (int i = 0; i < scriptCount; i++)
			{
				if (this.GetScriptAtIndex(i) is T)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00003ADC File Offset: 0x00001CDC
		public T GetFirstScriptOfTypeInFamily<T>() where T : ScriptComponentBehavior
		{
			T t = this.GetFirstScriptOfType<T>();
			GameEntity gameEntity = this;
			while (t == null && gameEntity.Parent != null)
			{
				gameEntity = gameEntity.Parent;
				t = gameEntity.GetFirstScriptOfType<T>();
			}
			return t;
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00003B1C File Offset: 0x00001D1C
		public T GetFirstScriptOfType<T>() where T : ScriptComponentBehavior
		{
			int scriptCount = this.ScriptCount;
			for (int i = 0; i < scriptCount; i++)
			{
				T t;
				if ((t = this.GetScriptAtIndex(i) as T) != null)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00003B61 File Offset: 0x00001D61
		internal static GameEntity GetFirstEntityWithName(Scene scene, string entityName)
		{
			return EngineApplicationInterface.IGameEntity.FindWithName(scene.Pointer, entityName);
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x00003B74 File Offset: 0x00001D74
		// (set) Token: 0x060005F5 RID: 1525 RVA: 0x00003B86 File Offset: 0x00001D86
		public string Name
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetName(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IGameEntity.SetName(base.Pointer, value);
			}
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x00003B99 File Offset: 0x00001D99
		public void SetAlpha(float alpha)
		{
			EngineApplicationInterface.IGameEntity.SetAlpha(base.Pointer, alpha);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00003BAC File Offset: 0x00001DAC
		public void SetVisibilityExcludeParents(bool visible)
		{
			EngineApplicationInterface.IGameEntity.SetVisibilityExcludeParents(base.Pointer, visible);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00003BBF File Offset: 0x00001DBF
		public void SetReadyToRender(bool ready)
		{
			EngineApplicationInterface.IGameEntity.SetReadyToRender(base.Pointer, ready);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00003BD2 File Offset: 0x00001DD2
		public bool GetVisibilityExcludeParents()
		{
			return EngineApplicationInterface.IGameEntity.GetVisibilityExcludeParents(base.Pointer);
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x00003BE4 File Offset: 0x00001DE4
		public bool IsVisibleIncludeParents()
		{
			return EngineApplicationInterface.IGameEntity.IsVisibleIncludeParents(base.Pointer);
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x00003BF6 File Offset: 0x00001DF6
		public uint GetVisibilityLevelMaskIncludingParents()
		{
			return EngineApplicationInterface.IGameEntity.GetVisibilityLevelMaskIncludingParents(base.Pointer);
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00003C08 File Offset: 0x00001E08
		public bool GetEditModeLevelVisibility()
		{
			return EngineApplicationInterface.IGameEntity.GetEditModeLevelVisibility(base.Pointer);
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00003C1A File Offset: 0x00001E1A
		public void Remove(int removeReason)
		{
			EngineApplicationInterface.IGameEntity.Remove(base.Pointer, removeReason);
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00003C2D File Offset: 0x00001E2D
		internal static GameEntity GetFirstEntityWithTag(Scene scene, string tag)
		{
			return EngineApplicationInterface.IGameEntity.GetFirstEntityWithTag(scene.Pointer, tag);
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00003C40 File Offset: 0x00001E40
		internal static GameEntity GetNextEntityWithTag(Scene scene, GameEntity startEntity, string tag)
		{
			if (!(startEntity == null))
			{
				return EngineApplicationInterface.IGameEntity.GetNextEntityWithTag(startEntity.Pointer, tag);
			}
			return GameEntity.GetFirstEntityWithTag(scene, tag);
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00003C64 File Offset: 0x00001E64
		internal static GameEntity GetFirstEntityWithTagExpression(Scene scene, string tagExpression)
		{
			return EngineApplicationInterface.IGameEntity.GetFirstEntityWithTagExpression(scene.Pointer, tagExpression);
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00003C77 File Offset: 0x00001E77
		internal static GameEntity GetNextEntityWithTagExpression(Scene scene, GameEntity startEntity, string tagExpression)
		{
			if (!(startEntity == null))
			{
				return EngineApplicationInterface.IGameEntity.GetNextEntityWithTagExpression(startEntity.Pointer, tagExpression);
			}
			return GameEntity.GetFirstEntityWithTagExpression(scene, tagExpression);
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x00003C9B File Offset: 0x00001E9B
		internal static GameEntity GetNextPrefab(GameEntity current)
		{
			return EngineApplicationInterface.IGameEntity.GetNextPrefab((current == null) ? new UIntPtr(0U) : current.Pointer);
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x00003CBE File Offset: 0x00001EBE
		public static GameEntity CopyFromPrefab(GameEntity prefab)
		{
			if (!(prefab != null))
			{
				return null;
			}
			return EngineApplicationInterface.IGameEntity.CopyFromPrefab(prefab.Pointer);
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00003CDB File Offset: 0x00001EDB
		public void SetUpgradeLevelMask(GameEntity.UpgradeLevelMask mask)
		{
			EngineApplicationInterface.IGameEntity.SetUpgradeLevelMask(base.Pointer, (uint)mask);
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00003CEE File Offset: 0x00001EEE
		public GameEntity.UpgradeLevelMask GetUpgradeLevelMask()
		{
			return (GameEntity.UpgradeLevelMask)EngineApplicationInterface.IGameEntity.GetUpgradeLevelMask(base.Pointer);
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00003D00 File Offset: 0x00001F00
		public GameEntity.UpgradeLevelMask GetUpgradeLevelMaskCumulative()
		{
			return (GameEntity.UpgradeLevelMask)EngineApplicationInterface.IGameEntity.GetUpgradeLevelMaskCumulative(base.Pointer);
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00003D14 File Offset: 0x00001F14
		public int GetUpgradeLevelOfEntity()
		{
			int upgradeLevelMask = (int)this.GetUpgradeLevelMask();
			if ((upgradeLevelMask & 1) > 0)
			{
				return 0;
			}
			if ((upgradeLevelMask & 2) > 0)
			{
				return 1;
			}
			if ((upgradeLevelMask & 4) > 0)
			{
				return 2;
			}
			if ((upgradeLevelMask & 8) > 0)
			{
				return 3;
			}
			return -1;
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00003D49 File Offset: 0x00001F49
		public string GetOldPrefabName()
		{
			return EngineApplicationInterface.IGameEntity.GetOldPrefabName(base.Pointer);
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00003D5B File Offset: 0x00001F5B
		public string GetPrefabName()
		{
			return EngineApplicationInterface.IGameEntity.GetPrefabName(base.Pointer);
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00003D6D File Offset: 0x00001F6D
		public void CopyScriptComponentFromAnotherEntity(GameEntity otherEntity, string scriptName)
		{
			EngineApplicationInterface.IGameEntity.CopyScriptComponentFromAnotherEntity(base.Pointer, otherEntity.Pointer, scriptName);
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00003D86 File Offset: 0x00001F86
		internal static IEnumerable<GameEntity> GetEntitiesWithTag(Scene scene, string tag)
		{
			GameEntity entity = GameEntity.GetFirstEntityWithTag(scene, tag);
			while (entity != null)
			{
				yield return entity;
				entity = GameEntity.GetNextEntityWithTag(scene, entity, tag);
			}
			yield break;
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00003D9D File Offset: 0x00001F9D
		internal static IEnumerable<GameEntity> GetEntitiesWithTagExpression(Scene scene, string tagExpression)
		{
			GameEntity entity = GameEntity.GetFirstEntityWithTagExpression(scene, tagExpression);
			while (entity != null)
			{
				yield return entity;
				entity = GameEntity.GetNextEntityWithTagExpression(scene, entity, tagExpression);
			}
			yield break;
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00003DB4 File Offset: 0x00001FB4
		public void SetFrame(ref MatrixFrame frame)
		{
			EngineApplicationInterface.IGameEntity.SetFrame(base.Pointer, ref frame);
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00003DC7 File Offset: 0x00001FC7
		public void SetClothComponentKeepState(MetaMesh metaMesh, bool state)
		{
			EngineApplicationInterface.IGameEntity.SetClothComponentKeepState(base.Pointer, metaMesh.Pointer, state);
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00003DE0 File Offset: 0x00001FE0
		public void SetClothComponentKeepStateOfAllMeshes(bool state)
		{
			EngineApplicationInterface.IGameEntity.SetClothComponentKeepStateOfAllMeshes(base.Pointer, state);
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x00003DF3 File Offset: 0x00001FF3
		public void SetPreviousFrameInvalid()
		{
			EngineApplicationInterface.IGameEntity.SetPreviousFrameInvalid(base.Pointer);
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x00003E08 File Offset: 0x00002008
		public MatrixFrame GetFrame()
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.IGameEntity.GetFrame(base.Pointer, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00003E30 File Offset: 0x00002030
		public void GetFrame(out MatrixFrame frame)
		{
			frame = this.GetFrame();
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00003E3E File Offset: 0x0000203E
		public void UpdateTriadFrameForEditor()
		{
			EngineApplicationInterface.IGameEntity.UpdateTriadFrameForEditor(base.Pointer);
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00003E50 File Offset: 0x00002050
		public void UpdateTriadFrameForEditorForAllChildren()
		{
			this.UpdateTriadFrameForEditor();
			List<GameEntity> list = new List<GameEntity>();
			this.GetChildrenRecursive(ref list);
			foreach (GameEntity gameEntity in list)
			{
				EngineApplicationInterface.IGameEntity.UpdateTriadFrameForEditor(gameEntity.Pointer);
			}
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00003EBC File Offset: 0x000020BC
		public MatrixFrame GetGlobalFrame()
		{
			MatrixFrame matrixFrame;
			EngineApplicationInterface.IGameEntity.GetGlobalFrame(base.Pointer, out matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00003EDC File Offset: 0x000020DC
		public void SetGlobalFrame(in MatrixFrame frame)
		{
			EngineApplicationInterface.IGameEntity.SetGlobalFrame(base.Pointer, frame);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00003EF0 File Offset: 0x000020F0
		public void SetGlobalFrameMT(in MatrixFrame frame)
		{
			using (new TWSharedMutexReadLock(Scene.PhysicsAndRayCastLock))
			{
				EngineApplicationInterface.IGameEntity.SetGlobalFrame(base.Pointer, frame);
			}
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00003F3C File Offset: 0x0000213C
		public MatrixFrame GetPreviousGlobalFrame()
		{
			MatrixFrame matrixFrame;
			EngineApplicationInterface.IGameEntity.GetPreviousGlobalFrame(base.Pointer, out matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00003F5C File Offset: 0x0000215C
		public bool HasPhysicsBody()
		{
			return EngineApplicationInterface.IGameEntity.HasPhysicsBody(base.Pointer);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00003F6E File Offset: 0x0000216E
		public void SetLocalPosition(Vec3 position)
		{
			EngineApplicationInterface.IGameEntity.SetLocalPosition(base.Pointer, position);
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600061B RID: 1563 RVA: 0x00003F81 File Offset: 0x00002181
		// (set) Token: 0x0600061C RID: 1564 RVA: 0x00003F93 File Offset: 0x00002193
		public EntityFlags EntityFlags
		{
			get
			{
				return (EntityFlags)EngineApplicationInterface.IGameEntity.GetEntityFlags(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IGameEntity.SetEntityFlags(base.Pointer, (uint)value);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x00003FA6 File Offset: 0x000021A6
		// (set) Token: 0x0600061E RID: 1566 RVA: 0x00003FB8 File Offset: 0x000021B8
		public EntityVisibilityFlags EntityVisibilityFlags
		{
			get
			{
				return (EntityVisibilityFlags)EngineApplicationInterface.IGameEntity.GetEntityVisibilityFlags(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IGameEntity.SetEntityVisibilityFlags(base.Pointer, (uint)value);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x00003FCB File Offset: 0x000021CB
		// (set) Token: 0x06000620 RID: 1568 RVA: 0x00003FDD File Offset: 0x000021DD
		public BodyFlags BodyFlag
		{
			get
			{
				return (BodyFlags)EngineApplicationInterface.IGameEntity.GetBodyFlags(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IGameEntity.SetBodyFlags(base.Pointer, (uint)value);
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x00003FF0 File Offset: 0x000021F0
		public BodyFlags PhysicsDescBodyFlag
		{
			get
			{
				return (BodyFlags)EngineApplicationInterface.IGameEntity.GetPhysicsDescBodyFlags(base.Pointer);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x00004002 File Offset: 0x00002202
		public float Mass
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetMass(base.Pointer);
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x00004014 File Offset: 0x00002214
		public Vec3 CenterOfMass
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetCenterOfMass(base.Pointer);
			}
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x00004026 File Offset: 0x00002226
		public void SetBodyFlags(BodyFlags bodyFlags)
		{
			this.BodyFlag = bodyFlags;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0000402F File Offset: 0x0000222F
		public void SetBodyFlagsRecursive(BodyFlags bodyFlags)
		{
			EngineApplicationInterface.IGameEntity.SetBodyFlagsRecursive(base.Pointer, (uint)bodyFlags);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00004044 File Offset: 0x00002244
		public void AddBodyFlags(BodyFlags bodyFlags, bool applyToChildren = true)
		{
			this.BodyFlag |= bodyFlags;
			if (applyToChildren)
			{
				foreach (GameEntity gameEntity in this.GetChildren())
				{
					gameEntity.AddBodyFlags(bodyFlags, true);
				}
			}
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x000040A4 File Offset: 0x000022A4
		public void RemoveBodyFlags(BodyFlags bodyFlags, bool applyToChildren = true)
		{
			this.BodyFlag &= ~bodyFlags;
			if (applyToChildren)
			{
				foreach (GameEntity gameEntity in this.GetChildren())
				{
					gameEntity.RemoveBodyFlags(bodyFlags, true);
				}
			}
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00004104 File Offset: 0x00002304
		public Vec3 GetGlobalScale()
		{
			return EngineApplicationInterface.IGameEntity.GetGlobalScale(this);
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00004114 File Offset: 0x00002314
		public Vec3 GetLocalScale()
		{
			return this.GetFrame().rotation.GetScaleVector();
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x00004134 File Offset: 0x00002334
		public Vec3 GlobalPosition
		{
			get
			{
				return this.GetGlobalFrame().origin;
			}
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00004144 File Offset: 0x00002344
		public void SetAnimationSoundActivation(bool activate)
		{
			EngineApplicationInterface.IGameEntity.SetAnimationSoundActivation(base.Pointer, activate);
			foreach (GameEntity gameEntity in this.GetChildren())
			{
				gameEntity.SetAnimationSoundActivation(activate);
			}
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x000041A0 File Offset: 0x000023A0
		public void CopyComponentsToSkeleton()
		{
			EngineApplicationInterface.IGameEntity.CopyComponentsToSkeleton(base.Pointer);
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x000041B2 File Offset: 0x000023B2
		public void AddMeshToBone(sbyte boneIndex, Mesh mesh)
		{
			EngineApplicationInterface.IGameEntity.AddMeshToBone(base.Pointer, mesh.Pointer, boneIndex);
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x000041CB File Offset: 0x000023CB
		public void ActivateRagdoll()
		{
			EngineApplicationInterface.IGameEntity.ActivateRagdoll(base.Pointer);
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x000041DD File Offset: 0x000023DD
		public void PauseSkeletonAnimation()
		{
			EngineApplicationInterface.IGameEntity.Freeze(base.Pointer, true);
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x000041F0 File Offset: 0x000023F0
		public void ResumeSkeletonAnimation()
		{
			EngineApplicationInterface.IGameEntity.Freeze(base.Pointer, false);
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00004203 File Offset: 0x00002403
		public bool IsSkeletonAnimationPaused()
		{
			return EngineApplicationInterface.IGameEntity.IsFrozen(base.Pointer);
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00004215 File Offset: 0x00002415
		public sbyte GetBoneCount()
		{
			return EngineApplicationInterface.IGameEntity.GetBoneCount(base.Pointer);
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00004228 File Offset: 0x00002428
		public MatrixFrame GetBoneEntitialFrameWithIndex(sbyte boneIndex)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.IGameEntity.GetBoneEntitialFrameWithIndex(base.Pointer, boneIndex, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00004254 File Offset: 0x00002454
		public MatrixFrame GetBoneEntitialFrameWithName(string boneName)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.IGameEntity.GetBoneEntitialFrameWithName(base.Pointer, boneName, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000635 RID: 1589 RVA: 0x0000427D File Offset: 0x0000247D
		public string[] Tags
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetTags(base.Pointer).Split(new char[] { ' ' });
			}
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0000429F File Offset: 0x0000249F
		public void AddTag(string tag)
		{
			EngineApplicationInterface.IGameEntity.AddTag(base.Pointer, tag);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x000042B2 File Offset: 0x000024B2
		public void RemoveTag(string tag)
		{
			EngineApplicationInterface.IGameEntity.RemoveTag(base.Pointer, tag);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x000042C5 File Offset: 0x000024C5
		public bool HasTag(string tag)
		{
			return EngineApplicationInterface.IGameEntity.HasTag(base.Pointer, tag);
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x000042D8 File Offset: 0x000024D8
		public void AddChild(GameEntity gameEntity, bool autoLocalizeFrame = false)
		{
			EngineApplicationInterface.IGameEntity.AddChild(base.Pointer, gameEntity.Pointer, autoLocalizeFrame);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x000042F1 File Offset: 0x000024F1
		public void RemoveChild(GameEntity childEntity, bool keepPhysics, bool keepScenePointer, bool callScriptCallbacks, int removeReason)
		{
			EngineApplicationInterface.IGameEntity.RemoveChild(base.Pointer, childEntity.Pointer, keepPhysics, keepScenePointer, callScriptCallbacks, removeReason);
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0000430F File Offset: 0x0000250F
		public void BreakPrefab()
		{
			EngineApplicationInterface.IGameEntity.BreakPrefab(base.Pointer);
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x00004321 File Offset: 0x00002521
		public int ChildCount
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetChildCount(base.Pointer);
			}
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00004333 File Offset: 0x00002533
		public GameEntity GetChild(int index)
		{
			return EngineApplicationInterface.IGameEntity.GetChild(base.Pointer, index);
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x00004346 File Offset: 0x00002546
		public GameEntity Parent
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetParent(base.Pointer);
			}
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00004358 File Offset: 0x00002558
		public bool HasComplexAnimTree()
		{
			return EngineApplicationInterface.IGameEntity.HasComplexAnimTree(base.Pointer);
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0000436C File Offset: 0x0000256C
		public GameEntity Root
		{
			get
			{
				GameEntity gameEntity = this;
				while (gameEntity.Parent != null)
				{
					gameEntity = gameEntity.Parent;
				}
				return gameEntity;
			}
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x00004393 File Offset: 0x00002593
		public void AddMultiMesh(MetaMesh metaMesh, bool updateVisMask = true)
		{
			EngineApplicationInterface.IGameEntity.AddMultiMesh(base.Pointer, metaMesh.Pointer, updateVisMask);
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x000043AC File Offset: 0x000025AC
		public bool RemoveMultiMesh(MetaMesh metaMesh)
		{
			return EngineApplicationInterface.IGameEntity.RemoveMultiMesh(base.Pointer, metaMesh.Pointer);
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000643 RID: 1603 RVA: 0x000043C4 File Offset: 0x000025C4
		public int MultiMeshComponentCount
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetComponentCount(base.Pointer, GameEntity.ComponentType.MetaMesh);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x000043D7 File Offset: 0x000025D7
		public int ClothSimulatorComponentCount
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetComponentCount(base.Pointer, GameEntity.ComponentType.ClothSimulator);
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x000043EA File Offset: 0x000025EA
		public int GetComponentCount(GameEntity.ComponentType componentType)
		{
			return EngineApplicationInterface.IGameEntity.GetComponentCount(base.Pointer, componentType);
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000043FD File Offset: 0x000025FD
		public void AddAllMeshesOfGameEntity(GameEntity gameEntity)
		{
			EngineApplicationInterface.IGameEntity.AddAllMeshesOfGameEntity(base.Pointer, gameEntity.Pointer);
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00004415 File Offset: 0x00002615
		public void SetFrameChanged()
		{
			EngineApplicationInterface.IGameEntity.SetFrameChanged(base.Pointer);
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00004427 File Offset: 0x00002627
		public GameEntityComponent GetComponentAtIndex(int index, GameEntity.ComponentType componentType)
		{
			return EngineApplicationInterface.IGameEntity.GetComponentAtIndex(base.Pointer, componentType, index);
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0000443B File Offset: 0x0000263B
		public MetaMesh GetMetaMesh(int metaMeshIndex)
		{
			return (MetaMesh)EngineApplicationInterface.IGameEntity.GetComponentAtIndex(base.Pointer, GameEntity.ComponentType.MetaMesh, metaMeshIndex);
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00004454 File Offset: 0x00002654
		public ClothSimulatorComponent GetClothSimulator(int clothSimulatorIndex)
		{
			return (ClothSimulatorComponent)EngineApplicationInterface.IGameEntity.GetComponentAtIndex(base.Pointer, GameEntity.ComponentType.ClothSimulator, clothSimulatorIndex);
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x0000446D File Offset: 0x0000266D
		public void SetVectorArgument(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.IGameEntity.SetVectorArgument(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00004484 File Offset: 0x00002684
		public void SetMaterialForAllMeshes(Material material)
		{
			EngineApplicationInterface.IGameEntity.SetMaterialForAllMeshes(base.Pointer, material.Pointer);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0000449C File Offset: 0x0000269C
		public bool AddLight(Light light)
		{
			return EngineApplicationInterface.IGameEntity.AddLight(base.Pointer, light.Pointer);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x000044B4 File Offset: 0x000026B4
		public Light GetLight()
		{
			return EngineApplicationInterface.IGameEntity.GetLight(base.Pointer);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000044C6 File Offset: 0x000026C6
		public void AddParticleSystemComponent(string particleid)
		{
			EngineApplicationInterface.IGameEntity.AddParticleSystemComponent(base.Pointer, particleid);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x000044D9 File Offset: 0x000026D9
		public void RemoveAllParticleSystems()
		{
			EngineApplicationInterface.IGameEntity.RemoveAllParticleSystems(base.Pointer);
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x000044EB File Offset: 0x000026EB
		public bool CheckPointWithOrientedBoundingBox(Vec3 point)
		{
			return EngineApplicationInterface.IGameEntity.CheckPointWithOrientedBoundingBox(base.Pointer, point);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000044FE File Offset: 0x000026FE
		public void PauseParticleSystem(bool doChildren)
		{
			EngineApplicationInterface.IGameEntity.PauseParticleSystem(base.Pointer, doChildren);
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00004511 File Offset: 0x00002711
		public void ResumeParticleSystem(bool doChildren)
		{
			EngineApplicationInterface.IGameEntity.ResumeParticleSystem(base.Pointer, doChildren);
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00004524 File Offset: 0x00002724
		public void BurstEntityParticle(bool doChildren)
		{
			EngineApplicationInterface.IGameEntity.BurstEntityParticle(base.Pointer, doChildren);
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00004537 File Offset: 0x00002737
		public void SetRuntimeEmissionRateMultiplier(float emissionRateMultiplier)
		{
			EngineApplicationInterface.IGameEntity.SetRuntimeEmissionRateMultiplier(base.Pointer, emissionRateMultiplier);
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0000454A File Offset: 0x0000274A
		public Vec3 GetBoundingBoxMin()
		{
			return EngineApplicationInterface.IGameEntity.GetBoundingBoxMin(base.Pointer);
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0000455C File Offset: 0x0000275C
		public float GetBoundingBoxRadius()
		{
			return EngineApplicationInterface.IGameEntity.GetRadius(base.Pointer);
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0000456E File Offset: 0x0000276E
		public Vec3 GetBoundingBoxMax()
		{
			return EngineApplicationInterface.IGameEntity.GetBoundingBoxMax(base.Pointer);
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00004580 File Offset: 0x00002780
		public void UpdateGlobalBounds()
		{
			EngineApplicationInterface.IGameEntity.UpdateGlobalBounds(base.Pointer);
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00004592 File Offset: 0x00002792
		public void RecomputeBoundingBox()
		{
			EngineApplicationInterface.IGameEntity.RecomputeBoundingBox(this);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0000459F File Offset: 0x0000279F
		public void ValidateBoundingBox()
		{
			EngineApplicationInterface.IGameEntity.ValidateBoundingBox(base.Pointer);
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x000045B1 File Offset: 0x000027B1
		public bool GetHasFrameChanged()
		{
			return EngineApplicationInterface.IGameEntity.HasFrameChanged(base.Pointer);
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x000045C3 File Offset: 0x000027C3
		public void SetBoundingboxDirty()
		{
			EngineApplicationInterface.IGameEntity.SetBoundingboxDirty(base.Pointer);
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x000045D5 File Offset: 0x000027D5
		public Mesh GetFirstMesh()
		{
			return EngineApplicationInterface.IGameEntity.GetFirstMesh(base.Pointer);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600065F RID: 1631 RVA: 0x000045E7 File Offset: 0x000027E7
		public Vec3 GlobalBoxMax
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetGlobalBoxMax(base.Pointer);
			}
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x000045FC File Offset: 0x000027FC
		public void SetContourColor(uint? color, bool alwaysVisible = true)
		{
			if (color != null)
			{
				EngineApplicationInterface.IGameEntity.SetAsContourEntity(base.Pointer, color.Value);
				EngineApplicationInterface.IGameEntity.SetContourState(base.Pointer, alwaysVisible);
				return;
			}
			EngineApplicationInterface.IGameEntity.DisableContour(base.Pointer);
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000661 RID: 1633 RVA: 0x0000464B File Offset: 0x0000284B
		public Vec3 GlobalBoxMin
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetGlobalBoxMin(base.Pointer);
			}
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0000465D File Offset: 0x0000285D
		public void SetExternalReferencesUsage(bool value)
		{
			EngineApplicationInterface.IGameEntity.SetExternalReferencesUsage(base.Pointer, value);
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x00004670 File Offset: 0x00002870
		public void SetMorphFrameOfComponents(float value)
		{
			EngineApplicationInterface.IGameEntity.SetMorphFrameOfComponents(base.Pointer, value);
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x00004683 File Offset: 0x00002883
		public void AddEditDataUserToAllMeshes(bool entityComponents, bool skeletonComponents)
		{
			EngineApplicationInterface.IGameEntity.AddEditDataUserToAllMeshes(base.Pointer, entityComponents, skeletonComponents);
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x00004697 File Offset: 0x00002897
		public void ReleaseEditDataUserToAllMeshes(bool entityComponents, bool skeletonComponents)
		{
			EngineApplicationInterface.IGameEntity.ReleaseEditDataUserToAllMeshes(base.Pointer, entityComponents, skeletonComponents);
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x000046AB File Offset: 0x000028AB
		public void GetCameraParamsFromCameraScript(Camera cam, ref Vec3 dofParams)
		{
			EngineApplicationInterface.IGameEntity.GetCameraParamsFromCameraScript(base.Pointer, cam.Pointer, ref dofParams);
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x000046C4 File Offset: 0x000028C4
		public void GetMeshBendedFrame(MatrixFrame worldSpacePosition, ref MatrixFrame output)
		{
			EngineApplicationInterface.IGameEntity.GetMeshBendedPosition(base.Pointer, ref worldSpacePosition, ref output);
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x000046D9 File Offset: 0x000028D9
		public void SetAnimTreeChannelParameterForceUpdate(float phase, int channelNo)
		{
			EngineApplicationInterface.IGameEntity.SetAnimTreeChannelParameter(base.Pointer, phase, channelNo);
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x000046ED File Offset: 0x000028ED
		public void ChangeMetaMeshOrRemoveItIfNotExists(MetaMesh entityMetaMesh, MetaMesh newMetaMesh)
		{
			EngineApplicationInterface.IGameEntity.ChangeMetaMeshOrRemoveItIfNotExists(base.Pointer, (entityMetaMesh != null) ? entityMetaMesh.Pointer : UIntPtr.Zero, (newMetaMesh != null) ? newMetaMesh.Pointer : UIntPtr.Zero);
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0000472B File Offset: 0x0000292B
		public void AttachNavigationMeshFaces(int faceGroupId, bool isConnected, bool isBlocker = false, bool autoLocalize = false)
		{
			EngineApplicationInterface.IGameEntity.AttachNavigationMeshFaces(base.Pointer, faceGroupId, isConnected, isBlocker, autoLocalize);
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x00004742 File Offset: 0x00002942
		public void RemoveSkeleton()
		{
			this.Skeleton = null;
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x0000474B File Offset: 0x0000294B
		// (set) Token: 0x0600066D RID: 1645 RVA: 0x0000475D File Offset: 0x0000295D
		public Skeleton Skeleton
		{
			get
			{
				return EngineApplicationInterface.IGameEntity.GetSkeleton(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IGameEntity.SetSkeleton(base.Pointer, (value != null) ? value.Pointer : UIntPtr.Zero);
			}
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0000477F File Offset: 0x0000297F
		public void RemoveAllChildren()
		{
			EngineApplicationInterface.IGameEntity.RemoveAllChildren(base.Pointer);
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00004791 File Offset: 0x00002991
		public IEnumerable<GameEntity> GetChildren()
		{
			int count = this.ChildCount;
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				yield return this.GetChild(i);
				num = i;
			}
			yield break;
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x000047A1 File Offset: 0x000029A1
		public IEnumerable<GameEntity> GetEntityAndChildren()
		{
			yield return this;
			int count = this.ChildCount;
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				yield return this.GetChild(i);
				num = i;
			}
			yield break;
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x000047B4 File Offset: 0x000029B4
		public void GetChildrenRecursive(ref List<GameEntity> children)
		{
			int childCount = this.ChildCount;
			for (int i = 0; i < childCount; i++)
			{
				GameEntity child = this.GetChild(i);
				children.Add(child);
				child.GetChildrenRecursive(ref children);
			}
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x000047EB File Offset: 0x000029EB
		public bool IsSelectedOnEditor()
		{
			return EngineApplicationInterface.IGameEntity.IsEntitySelectedOnEditor(base.Pointer);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x000047FD File Offset: 0x000029FD
		public void SelectEntityOnEditor()
		{
			EngineApplicationInterface.IGameEntity.SelectEntityOnEditor(base.Pointer);
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0000480F File Offset: 0x00002A0F
		public void DeselectEntityOnEditor()
		{
			EngineApplicationInterface.IGameEntity.DeselectEntityOnEditor(base.Pointer);
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x00004821 File Offset: 0x00002A21
		public void SetAsPredisplayEntity()
		{
			EngineApplicationInterface.IGameEntity.SetAsPredisplayEntity(base.Pointer);
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x00004833 File Offset: 0x00002A33
		public void RemoveFromPredisplayEntity()
		{
			EngineApplicationInterface.IGameEntity.RemoveFromPredisplayEntity(base.Pointer);
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x00004845 File Offset: 0x00002A45
		public void GetPhysicsMinMax(bool includeChildren, out Vec3 bbmin, out Vec3 bbmax, bool returnLocal)
		{
			bbmin = Vec3.Zero;
			bbmax = Vec3.Zero;
			EngineApplicationInterface.IGameEntity.GetPhysicsMinMax(base.Pointer, includeChildren, ref bbmin, ref bbmax, returnLocal);
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00004872 File Offset: 0x00002A72
		public void SetCullMode(MBMeshCullingMode cullMode)
		{
			EngineApplicationInterface.IGameEntity.SetCullMode(base.Pointer, cullMode);
		}

		// Token: 0x020000A9 RID: 169
		public enum ComponentType : uint
		{
			// Token: 0x0400032E RID: 814
			MetaMesh,
			// Token: 0x0400032F RID: 815
			Light,
			// Token: 0x04000330 RID: 816
			CompositeComponent,
			// Token: 0x04000331 RID: 817
			ClothSimulator,
			// Token: 0x04000332 RID: 818
			ParticleSystemInstanced,
			// Token: 0x04000333 RID: 819
			TownIcon,
			// Token: 0x04000334 RID: 820
			CustomType1,
			// Token: 0x04000335 RID: 821
			Decal
		}

		// Token: 0x020000AA RID: 170
		public enum Mobility
		{
			// Token: 0x04000337 RID: 823
			stationary,
			// Token: 0x04000338 RID: 824
			dynamic,
			// Token: 0x04000339 RID: 825
			dynamic_forced
		}

		// Token: 0x020000AB RID: 171
		[Flags]
		public enum UpgradeLevelMask
		{
			// Token: 0x0400033B RID: 827
			None = 0,
			// Token: 0x0400033C RID: 828
			Level0 = 1,
			// Token: 0x0400033D RID: 829
			Level1 = 2,
			// Token: 0x0400033E RID: 830
			Level2 = 4,
			// Token: 0x0400033F RID: 831
			Level3 = 8,
			// Token: 0x04000340 RID: 832
			LevelAll = 15
		}
	}
}
