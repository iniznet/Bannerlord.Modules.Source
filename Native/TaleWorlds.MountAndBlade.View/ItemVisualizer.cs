using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000017 RID: 23
	public class ItemVisualizer : ScriptComponentBehavior
	{
		// Token: 0x0600009F RID: 159 RVA: 0x00006CAC File Offset: 0x00004EAC
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			if (Game.Current == null)
			{
				this._editorGameManager = new EditorGameManager();
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006CC6 File Offset: 0x00004EC6
		protected override void OnEditorTick(float dt)
		{
			if (!this.isFinished && this._editorGameManager != null)
			{
				this.isFinished = !this._editorGameManager.DoLoadingForGameManager();
				if (this.isFinished)
				{
					this.SpawnItems();
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006CFC File Offset: 0x00004EFC
		private void SpawnItems()
		{
			Scene scene = base.GameEntity.Scene;
			IEnumerable<ItemObject> objectTypeList = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();
			Vec3 vec;
			vec..ctor(100f, 100f, 0f, -1f);
			vec.z = 2f;
			float num = 0f;
			float num2 = 200f;
			foreach (ItemObject itemObject in objectTypeList)
			{
				if (itemObject.MultiMeshName.Length > 0)
				{
					MetaMesh copy = MetaMesh.GetCopy(itemObject.MultiMeshName, true, true);
					if (copy != null)
					{
						GameEntity gameEntity = GameEntity.CreateEmpty(scene, true);
						gameEntity.EntityFlags |= 131072;
						gameEntity.AddMultiMesh(copy, true);
						gameEntity.Name = itemObject.Name.ToString();
						gameEntity.RecomputeBoundingBox();
						float boundingBoxRadius = gameEntity.GetBoundingBoxRadius();
						if (boundingBoxRadius > num)
						{
							num = boundingBoxRadius;
						}
						vec.x += boundingBoxRadius;
						if (vec.x > num2)
						{
							vec.x = 100f;
							vec.y += num * 3f;
							num = 0f;
						}
						gameEntity.SetLocalPosition(vec);
						vec.x += boundingBoxRadius;
						if (vec.x > num2)
						{
							vec.x = 100f;
							vec.y += num * 3f;
							num = 0f;
						}
					}
				}
			}
		}

		// Token: 0x0400003D RID: 61
		private MBGameManager _editorGameManager;

		// Token: 0x0400003E RID: 62
		private bool isFinished;
	}
}
