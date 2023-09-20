using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200035F RID: 863
	public class SpawnerEntityMissionHelper
	{
		// Token: 0x06002F27 RID: 12071 RVA: 0x000C051C File Offset: 0x000BE71C
		public SpawnerEntityMissionHelper(SpawnerBase spawner, bool fireVersion = false)
		{
			this._spawner = spawner;
			this._fireVersion = fireVersion;
			this._ownerEntity = this._spawner.GameEntity;
			this._gameEntityName = this._ownerEntity.Name;
			if (this.SpawnPrefab(this._ownerEntity, this.GetPrefabName()) != null)
			{
				this.SyncMatrixFrames();
			}
			else
			{
				Debug.FailedAssert("Spawner couldn't spawn a proper entity.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SpawnerEntityMissionHelper.cs", ".ctor", 34);
			}
			this._spawner.AssignParameters(this);
			this.CallSetSpawnedFromSpawnerOfScripts();
		}

		// Token: 0x06002F28 RID: 12072 RVA: 0x000C05AC File Offset: 0x000BE7AC
		private GameEntity SpawnPrefab(GameEntity parent, string entityName)
		{
			this.SpawnedEntity = GameEntity.Instantiate(parent.Scene, entityName, false);
			this.SpawnedEntity.SetMobility(GameEntity.Mobility.dynamic);
			this.SpawnedEntity.EntityFlags = this.SpawnedEntity.EntityFlags | EntityFlags.DontSaveToScene;
			parent.AddChild(this.SpawnedEntity, false);
			MatrixFrame identity = MatrixFrame.Identity;
			this.SpawnedEntity.SetFrame(ref identity);
			foreach (string text in this._ownerEntity.Tags)
			{
				this.SpawnedEntity.AddTag(text);
			}
			return this.SpawnedEntity;
		}

		// Token: 0x06002F29 RID: 12073 RVA: 0x000C0644 File Offset: 0x000BE844
		private void RemoveChildEntity(GameEntity child)
		{
			child.CallScriptCallbacks();
			child.Remove(85);
		}

		// Token: 0x06002F2A RID: 12074 RVA: 0x000C0654 File Offset: 0x000BE854
		private void SyncMatrixFrames()
		{
			List<GameEntity> list = new List<GameEntity>();
			this.SpawnedEntity.GetChildrenRecursive(ref list);
			foreach (GameEntity gameEntity in list)
			{
				if (SpawnerEntityMissionHelper.HasField(this._spawner, gameEntity.Name))
				{
					MatrixFrame matrixFrame = (MatrixFrame)SpawnerEntityMissionHelper.GetFieldValue(this._spawner, gameEntity.Name);
					gameEntity.SetFrame(ref matrixFrame);
				}
				if (SpawnerEntityMissionHelper.HasField(this._spawner, gameEntity.Name + "_enabled") && !(bool)SpawnerEntityMissionHelper.GetFieldValue(this._spawner, gameEntity.Name + "_enabled"))
				{
					this.RemoveChildEntity(gameEntity);
				}
			}
		}

		// Token: 0x06002F2B RID: 12075 RVA: 0x000C072C File Offset: 0x000BE92C
		private void CallSetSpawnedFromSpawnerOfScripts()
		{
			foreach (GameEntity gameEntity in this.SpawnedEntity.GetEntityAndChildren())
			{
				foreach (ScriptComponentBehavior scriptComponentBehavior in from x in gameEntity.GetScriptComponents()
					where x is ISpawnable
					select x)
				{
					(scriptComponentBehavior as ISpawnable).SetSpawnedFromSpawner();
				}
			}
		}

		// Token: 0x06002F2C RID: 12076 RVA: 0x000C07D8 File Offset: 0x000BE9D8
		private string GetPrefabName()
		{
			string text;
			if (this._spawner.ToBeSpawnedOverrideName != "")
			{
				text = this._spawner.ToBeSpawnedOverrideName;
			}
			else
			{
				text = this._gameEntityName;
				text = text.Remove(this._gameEntityName.Length - this._gameEntityName.Split(new char[] { '_' }).Last<string>().Length - 1);
			}
			if (this._fireVersion)
			{
				if (this._spawner.ToBeSpawnedOverrideNameForFireVersion != "")
				{
					text = this._spawner.ToBeSpawnedOverrideNameForFireVersion;
				}
				else
				{
					text += "_fire";
				}
			}
			return text;
		}

		// Token: 0x06002F2D RID: 12077 RVA: 0x000C0880 File Offset: 0x000BEA80
		private static object GetFieldValue(object src, string propName)
		{
			return src.GetType().GetField(propName).GetValue(src);
		}

		// Token: 0x06002F2E RID: 12078 RVA: 0x000C0894 File Offset: 0x000BEA94
		private static bool HasField(object obj, string propertyName)
		{
			return obj.GetType().GetField(propertyName) != null;
		}

		// Token: 0x04001353 RID: 4947
		private const string EnabledSuffix = "_enabled";

		// Token: 0x04001354 RID: 4948
		public GameEntity SpawnedEntity;

		// Token: 0x04001355 RID: 4949
		private GameEntity _ownerEntity;

		// Token: 0x04001356 RID: 4950
		private SpawnerBase _spawner;

		// Token: 0x04001357 RID: 4951
		private string _gameEntityName;

		// Token: 0x04001358 RID: 4952
		private bool _fireVersion;
	}
}
