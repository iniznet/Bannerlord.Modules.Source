using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	public class SpawnerEntityMissionHelper
	{
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

		private void RemoveChildEntity(GameEntity child)
		{
			child.CallScriptCallbacks();
			child.Remove(85);
		}

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

		private static object GetFieldValue(object src, string propName)
		{
			return src.GetType().GetField(propName).GetValue(src);
		}

		private static bool HasField(object obj, string propertyName)
		{
			return obj.GetType().GetField(propertyName) != null;
		}

		private const string EnabledSuffix = "_enabled";

		public GameEntity SpawnedEntity;

		private GameEntity _ownerEntity;

		private SpawnerBase _spawner;

		private string _gameEntityName;

		private bool _fireVersion;
	}
}
