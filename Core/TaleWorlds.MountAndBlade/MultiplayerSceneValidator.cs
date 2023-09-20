using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000340 RID: 832
	public class MultiplayerSceneValidator : ScriptComponentBehavior
	{
		// Token: 0x06002C75 RID: 11381 RVA: 0x000AC9F2 File Offset: 0x000AABF2
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "SelectFaultyEntities")
			{
				this.SelectInvalidEntities();
			}
		}

		// Token: 0x06002C76 RID: 11382 RVA: 0x000ACA10 File Offset: 0x000AAC10
		protected internal override void OnSceneSave(string saveFolder)
		{
			base.OnSceneSave(saveFolder);
			foreach (GameEntity gameEntity in this.GetInvalidEntities())
			{
			}
		}

		// Token: 0x06002C77 RID: 11383 RVA: 0x000ACA64 File Offset: 0x000AAC64
		private List<GameEntity> GetInvalidEntities()
		{
			List<GameEntity> list = new List<GameEntity>();
			List<GameEntity> list2 = new List<GameEntity>();
			base.Scene.GetEntities(ref list2);
			foreach (GameEntity gameEntity in list2)
			{
				foreach (ScriptComponentBehavior scriptComponentBehavior in gameEntity.GetScriptComponents())
				{
					if (scriptComponentBehavior != null && (scriptComponentBehavior.GetType().IsSubclassOf(typeof(MissionObject)) || (scriptComponentBehavior.GetType() == typeof(MissionObject) && scriptComponentBehavior.IsOnlyVisual())))
					{
						list.Add(gameEntity);
						break;
					}
				}
			}
			return list;
		}

		// Token: 0x06002C78 RID: 11384 RVA: 0x000ACB4C File Offset: 0x000AAD4C
		private void SelectInvalidEntities()
		{
			base.GameEntity.DeselectEntityOnEditor();
			foreach (GameEntity gameEntity in this.GetInvalidEntities())
			{
				gameEntity.SelectEntityOnEditor();
			}
		}

		// Token: 0x040010F9 RID: 4345
		public SimpleButton SelectFaultyEntities;
	}
}
