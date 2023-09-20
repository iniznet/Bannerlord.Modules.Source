using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerSceneValidator : ScriptComponentBehavior
	{
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "SelectFaultyEntities")
			{
				this.SelectInvalidEntities();
			}
		}

		protected internal override void OnSceneSave(string saveFolder)
		{
			base.OnSceneSave(saveFolder);
			foreach (GameEntity gameEntity in this.GetInvalidEntities())
			{
			}
		}

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

		private void SelectInvalidEntities()
		{
			base.GameEntity.DeselectEntityOnEditor();
			foreach (GameEntity gameEntity in this.GetInvalidEntities())
			{
				gameEntity.SelectEntityOnEditor();
			}
		}

		public SimpleButton SelectFaultyEntities;
	}
}
