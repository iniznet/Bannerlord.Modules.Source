using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Source.Objects
{
	// Token: 0x020003EC RID: 1004
	public class SceneLeveler : ScriptComponentBehavior
	{
		// Token: 0x06003499 RID: 13465 RVA: 0x000DA37C File Offset: 0x000D857C
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(variableName);
			if (num <= 90459893U)
			{
				if (num != 40127036U)
				{
					if (num != 73682274U)
					{
						if (num != 90459893U)
						{
							return;
						}
						if (!(variableName == "CreateLevel2"))
						{
							return;
						}
						this.OnLevelizeButtonPressed(2);
						return;
					}
					else
					{
						if (!(variableName == "CreateLevel3"))
						{
							return;
						}
						this.OnLevelizeButtonPressed(3);
						return;
					}
				}
				else
				{
					if (!(variableName == "CreateLevel1"))
					{
						return;
					}
					this.OnLevelizeButtonPressed(1);
					return;
				}
			}
			else if (num <= 1310461563U)
			{
				if (num != 804927328U)
				{
					if (num != 1310461563U)
					{
						return;
					}
					if (!(variableName == "DeleteLevel1"))
					{
						return;
					}
					this.OnDeleteButtonPressed(1);
					return;
				}
				else
				{
					if (!(variableName == "SelectEntitiesWithoutLevel"))
					{
						return;
					}
					this.OnSelectEntitiesWithoutLevelButtonPressed();
					return;
				}
			}
			else if (num != 1327239182U)
			{
				if (num != 1344016801U)
				{
					return;
				}
				if (!(variableName == "DeleteLevel3"))
				{
					return;
				}
				this.OnDeleteButtonPressed(3);
				return;
			}
			else
			{
				if (!(variableName == "DeleteLevel2"))
				{
					return;
				}
				this.OnDeleteButtonPressed(2);
				return;
			}
		}

		// Token: 0x0600349A RID: 13466 RVA: 0x000DA474 File Offset: 0x000D8674
		private void OnLevelizeButtonPressed(int level)
		{
			if (this.SourceSelectionSetName.IsEmpty<char>())
			{
				MessageManager.DisplayMessage("ApplyToSelectionSet is empty!");
				return;
			}
			if (this.TargetSelectionSetName.IsEmpty<char>())
			{
				MessageManager.DisplayMessage("NewSelectionSetName is empty!");
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			GameEntity.UpgradeLevelMask levelMask = this.GetLevelMask(level);
			List<GameEntity> list = this.CollectEntitiesWithLevel();
			List<GameEntity> list2 = new List<GameEntity>();
			foreach (GameEntity gameEntity in list)
			{
				string text = this.FindPossiblePrefabName(gameEntity);
				if (text.IsEmpty<char>())
				{
					num++;
				}
				else
				{
					GameEntity.UpgradeLevelMask upgradeLevelMask = gameEntity.GetUpgradeLevelMask();
					if ((upgradeLevelMask & levelMask) != GameEntity.UpgradeLevelMask.None)
					{
						num2++;
						list2.Add(gameEntity);
					}
					else
					{
						string text2 = this.ConvertPrefabName(text, levelMask);
						GameEntity gameEntity2 = GameEntity.Instantiate(base.Scene, text2, gameEntity.GetGlobalFrame());
						if (gameEntity2 == null)
						{
							num3++;
						}
						else
						{
							num4++;
							GameEntity.UpgradeLevelMask upgradeLevelMask2 = upgradeLevelMask & ~GameEntity.UpgradeLevelMask.Level1 & ~GameEntity.UpgradeLevelMask.Level2 & ~GameEntity.UpgradeLevelMask.Level3;
							upgradeLevelMask2 |= levelMask;
							gameEntity2.SetUpgradeLevelMask(upgradeLevelMask2);
							this.CopyScriptParameters(gameEntity2, gameEntity);
							list2.Add(gameEntity2);
						}
					}
				}
			}
			Debug.Print(string.Concat(new object[] { "Created Entities : ", num4, "\nAlready Visible In Desired Level : ", num2, "\nWithout Prefab For Level : ", num3, "\nWithout Prefab Info : ", num }), 0, Debug.DebugColor.Magenta, 17592186044416UL);
			Utilities.CreateSelectionInEditor(list2, this.TargetSelectionSetName);
		}

		// Token: 0x0600349B RID: 13467 RVA: 0x000DA620 File Offset: 0x000D8820
		private void CopyScriptParameters(GameEntity entity, GameEntity copyFromEntity)
		{
			if (copyFromEntity.HasScriptComponent("WallSegment") && !entity.HasScriptComponent("WallSegment"))
			{
				entity.CopyScriptComponentFromAnotherEntity(copyFromEntity, "WallSegment");
			}
			if (copyFromEntity.HasScriptComponent("mesh_bender") && !entity.HasScriptComponent("mesh_bender"))
			{
				entity.CopyScriptComponentFromAnotherEntity(copyFromEntity, "mesh_bender");
			}
			int num = 0;
			while (num < entity.ChildCount && num < copyFromEntity.ChildCount)
			{
				this.CopyScriptParameters(entity.GetChild(num), copyFromEntity.GetChild(num));
				num++;
			}
		}

		// Token: 0x0600349C RID: 13468 RVA: 0x000DA6A7 File Offset: 0x000D88A7
		private GameEntity.UpgradeLevelMask GetLevelMask(int level)
		{
			if (level == 1)
			{
				return GameEntity.UpgradeLevelMask.Level1;
			}
			if (level != 2)
			{
				return GameEntity.UpgradeLevelMask.Level3;
			}
			return GameEntity.UpgradeLevelMask.Level2;
		}

		// Token: 0x0600349D RID: 13469 RVA: 0x000DA6B6 File Offset: 0x000D88B6
		private string GetLevelSubString(GameEntity.UpgradeLevelMask levelMask)
		{
			if (levelMask == GameEntity.UpgradeLevelMask.Level1)
			{
				return "_l1";
			}
			if (levelMask == GameEntity.UpgradeLevelMask.Level2)
			{
				return "_l2";
			}
			if (levelMask != GameEntity.UpgradeLevelMask.Level3)
			{
				return "";
			}
			return "_l3";
		}

		// Token: 0x0600349E RID: 13470 RVA: 0x000DA6E0 File Offset: 0x000D88E0
		private string ConvertPrefabName(string prefabName, GameEntity.UpgradeLevelMask newLevelMask)
		{
			string text = prefabName;
			string levelSubString = this.GetLevelSubString(newLevelMask);
			if (newLevelMask != GameEntity.UpgradeLevelMask.Level1)
			{
				text = text.Replace(this.GetLevelSubString(GameEntity.UpgradeLevelMask.Level1), levelSubString);
			}
			if (newLevelMask != GameEntity.UpgradeLevelMask.Level2)
			{
				text = text.Replace(this.GetLevelSubString(GameEntity.UpgradeLevelMask.Level2), levelSubString);
			}
			if (newLevelMask != GameEntity.UpgradeLevelMask.Level3)
			{
				text = text.Replace(this.GetLevelSubString(GameEntity.UpgradeLevelMask.Level3), levelSubString);
			}
			if (text.Equals(prefabName))
			{
				return "";
			}
			return text;
		}

		// Token: 0x0600349F RID: 13471 RVA: 0x000DA740 File Offset: 0x000D8940
		private string FindPossiblePrefabName(GameEntity gameEntity)
		{
			string prefabName = gameEntity.GetPrefabName();
			if (prefabName.IsEmpty<char>())
			{
				return gameEntity.GetOldPrefabName();
			}
			return prefabName;
		}

		// Token: 0x060034A0 RID: 13472 RVA: 0x000DA764 File Offset: 0x000D8964
		private void OnDeleteButtonPressed(int level)
		{
			if (this.SourceSelectionSetName.IsEmpty<char>())
			{
				MessageManager.DisplayMessage("ApplyToSelectionSet is empty!");
				return;
			}
			List<GameEntity> list = this.CollectEntitiesWithLevel();
			GameEntity.UpgradeLevelMask levelMask = this.GetLevelMask(level);
			List<GameEntity> list2 = new List<GameEntity>();
			int num = 0;
			int num2 = 0;
			foreach (GameEntity gameEntity in list)
			{
				GameEntity.UpgradeLevelMask upgradeLevelMask = gameEntity.GetUpgradeLevelMask();
				if (upgradeLevelMask == levelMask)
				{
					list2.Add(gameEntity);
					num++;
				}
				else if ((upgradeLevelMask & levelMask) != GameEntity.UpgradeLevelMask.None)
				{
					gameEntity.SetUpgradeLevelMask(upgradeLevelMask & ~levelMask);
					num2++;
				}
			}
			Utilities.DeleteEntitiesInEditorScene(list2);
			TextObject textObject = new TextObject("{=!}Deleted entity count : {DELETED_ENTRY_COUNT}", null);
			TextObject textObject2 = new TextObject("{=!}Removed level mask count : {REMOVED_LEVEL_MASK}", null);
			textObject.SetTextVariable("DELETED_ENTRY_COUNT", num);
			textObject2.SetTextVariable("REMOVED_LEVEL_MASK", num2);
			MessageManager.DisplayMessage(textObject.ToString());
			MessageManager.DisplayMessage(textObject2.ToString());
		}

		// Token: 0x060034A1 RID: 13473 RVA: 0x000DA860 File Offset: 0x000D8A60
		private void OnSelectEntitiesWithoutLevelButtonPressed()
		{
			List<GameEntity> list = new List<GameEntity>();
			base.Scene.GetEntities(ref list);
			List<GameEntity> list2 = list.FindAll((GameEntity x) => x.GetUpgradeLevelMask() == GameEntity.UpgradeLevelMask.None);
			TextObject textObject = new TextObject("{=!}Selected entity count : {SELECTED_ENTITIES}", null);
			textObject.SetTextVariable("SELECTED_ENTITIES", list2.Count);
			MessageManager.DisplayMessage(textObject.ToString());
			if (list2.Count > 0)
			{
				Utilities.SelectEntities(list2);
			}
		}

		// Token: 0x060034A2 RID: 13474 RVA: 0x000DA8DC File Offset: 0x000D8ADC
		private List<GameEntity> CollectEntitiesWithLevel()
		{
			List<GameEntity> list = new List<GameEntity>();
			Utilities.GetEntitiesOfSelectionSet(this.SourceSelectionSetName, ref list);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if ((list[i].GetUpgradeLevelMask() & (GameEntity.UpgradeLevelMask.Level1 | GameEntity.UpgradeLevelMask.Level2 | GameEntity.UpgradeLevelMask.Level3)) == GameEntity.UpgradeLevelMask.None)
				{
					list.RemoveAt(i);
				}
			}
			return list;
		}

		// Token: 0x04001678 RID: 5752
		public string SourceSelectionSetName = "";

		// Token: 0x04001679 RID: 5753
		public string TargetSelectionSetName = "";

		// Token: 0x0400167A RID: 5754
		public SimpleButton CreateLevel1;

		// Token: 0x0400167B RID: 5755
		public SimpleButton CreateLevel2;

		// Token: 0x0400167C RID: 5756
		public SimpleButton CreateLevel3;

		// Token: 0x0400167D RID: 5757
		public SimpleButton DeleteLevel1;

		// Token: 0x0400167E RID: 5758
		public SimpleButton DeleteLevel2;

		// Token: 0x0400167F RID: 5759
		public SimpleButton DeleteLevel3;

		// Token: 0x04001680 RID: 5760
		public SimpleButton SelectEntitiesWithoutLevel;
	}
}
