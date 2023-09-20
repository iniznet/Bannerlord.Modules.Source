using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Scripts;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000016 RID: 22
	public class PopupSceneSpawnPoint : ScriptComponentBehavior
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000064B3 File Offset: 0x000046B3
		// (set) Token: 0x06000094 RID: 148 RVA: 0x000064BB File Offset: 0x000046BB
		public CompositeComponent AddedPrefabComponent { get; private set; }

		// Token: 0x06000095 RID: 149 RVA: 0x000064C4 File Offset: 0x000046C4
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000064D8 File Offset: 0x000046D8
		public void InitializeWithAgentVisuals(AgentVisuals humanVisuals, AgentVisuals mountVisuals = null)
		{
			this._humanAgentVisuals = humanVisuals;
			this._mountAgentVisuals = mountVisuals;
			this._initialStateActionCode = ActionIndexCache.Create(this.InitialAction);
			this._positiveStateActionCode = ((this.PositiveAction == "") ? this._initialStateActionCode : ActionIndexCache.Create(this.PositiveAction));
			this._negativeStateActionCode = ((this.NegativeAction == "") ? this._initialStateActionCode : ActionIndexCache.Create(this.NegativeAction));
			bool flag = !string.IsNullOrEmpty(this.RightHandWieldedItem);
			bool flag2 = !string.IsNullOrEmpty(this.LeftHandWieldedItem);
			if (flag2 || flag)
			{
				AgentVisualsData copyAgentVisualsData = this._humanAgentVisuals.GetCopyAgentVisualsData();
				Equipment equipment = this._humanAgentVisuals.GetEquipment().Clone(false);
				if (flag)
				{
					equipment[0] = new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>(this.RightHandWieldedItem), null, null, false);
				}
				if (flag2)
				{
					equipment[1] = new EquipmentElement(Game.Current.ObjectManager.GetObject<ItemObject>(this.LeftHandWieldedItem), null, null, false);
				}
				int num = (flag ? 0 : (-1));
				int num2 = (flag2 ? 1 : (-1));
				copyAgentVisualsData.RightWieldedItemIndex(num).LeftWieldedItemIndex(num2).Equipment(equipment);
				this._humanAgentVisuals.Refresh(false, copyAgentVisualsData, false);
			}
			else if (!flag2 && !flag2)
			{
				AgentVisualsData copyAgentVisualsData2 = this._humanAgentVisuals.GetCopyAgentVisualsData();
				Equipment equipment2 = this._humanAgentVisuals.GetEquipment().Clone(false);
				copyAgentVisualsData2.RightWieldedItemIndex(-1).LeftWieldedItemIndex(-1).Equipment(equipment2);
				this._humanAgentVisuals.Refresh(false, copyAgentVisualsData2, false);
			}
			if (this.PrefabItem != "")
			{
				if (!GameEntity.PrefabExists(this.PrefabItem))
				{
					MBDebug.ShowWarning(string.Concat(new string[]
					{
						"Cannot find prefab '",
						this.PrefabItem,
						"' for popup agent '",
						base.GameEntity.Name,
						"'"
					}));
				}
				else
				{
					this.AddedPrefabComponent = this._humanAgentVisuals.AddPrefabToAgentVisualBoneByBoneType(this.PrefabItem, this.PrefabBone);
					if (this.AddedPrefabComponent != null)
					{
						MatrixFrame frame = this.AddedPrefabComponent.Frame;
						MatrixFrame identity = MatrixFrame.Identity;
						identity.origin = this.AttachedPrefabOffset;
						this.AddedPrefabComponent.Frame = identity * frame;
					}
				}
			}
			foreach (GameEntity gameEntity in base.GameEntity.Scene.FindEntitiesWithTag(base.GameEntity.Name))
			{
				PopupSceneSequence firstScriptOfType = gameEntity.GetFirstScriptOfType<PopupSceneSequence>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.InitializeWithAgentVisuals(humanVisuals);
				}
			}
			AgentVisuals humanAgentVisuals = this._humanAgentVisuals;
			if (humanAgentVisuals != null)
			{
				MBAgentVisuals visuals = humanAgentVisuals.GetVisuals();
				if (visuals != null)
				{
					visuals.CheckResources(true);
				}
			}
			AgentVisuals mountAgentVisuals = this._mountAgentVisuals;
			if (mountAgentVisuals != null)
			{
				MBAgentVisuals visuals2 = mountAgentVisuals.GetVisuals();
				if (visuals2 != null)
				{
					visuals2.CheckResources(true);
				}
			}
			AgentVisuals mountAgentVisuals2 = this._mountAgentVisuals;
			if (mountAgentVisuals2 != null)
			{
				mountAgentVisuals2.Tick(null, 0.0001f, false, 0f);
			}
			AgentVisuals mountAgentVisuals3 = this._mountAgentVisuals;
			if (mountAgentVisuals3 != null)
			{
				GameEntity entity = mountAgentVisuals3.GetEntity();
				if (entity != null)
				{
					Skeleton skeleton = entity.Skeleton;
					if (skeleton != null)
					{
						skeleton.ForceUpdateBoneFrames();
					}
				}
			}
			AgentVisuals humanAgentVisuals2 = this._humanAgentVisuals;
			if (humanAgentVisuals2 != null)
			{
				humanAgentVisuals2.Tick(this._mountAgentVisuals, 0.0001f, false, 0f);
			}
			AgentVisuals humanAgentVisuals3 = this._humanAgentVisuals;
			if (humanAgentVisuals3 == null)
			{
				return;
			}
			GameEntity entity2 = humanAgentVisuals3.GetEntity();
			if (entity2 == null)
			{
				return;
			}
			Skeleton skeleton2 = entity2.Skeleton;
			if (skeleton2 == null)
			{
				return;
			}
			skeleton2.ForceUpdateBoneFrames();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000685C File Offset: 0x00004A5C
		public void SetInitialState()
		{
			if (this._initialStateActionCode != null)
			{
				float num = (this.StartWithRandomProgress ? MBRandom.RandomFloatRanged(0.5f) : 0f);
				AgentVisuals humanAgentVisuals = this._humanAgentVisuals;
				if (humanAgentVisuals != null)
				{
					humanAgentVisuals.SetAction(this._initialStateActionCode, num, true);
				}
				AgentVisuals mountAgentVisuals = this._mountAgentVisuals;
				if (mountAgentVisuals != null)
				{
					mountAgentVisuals.SetAction(this._initialStateActionCode, num, true);
				}
			}
			if (!string.IsNullOrEmpty(this.InitialFaceAnimCode))
			{
				MBSkeletonExtensions.SetFacialAnimation(this._humanAgentVisuals.GetVisuals().GetSkeleton(), 1, this.InitialFaceAnimCode, false, true);
			}
			foreach (GameEntity gameEntity in base.GameEntity.Scene.FindEntitiesWithTag(base.GameEntity.Name))
			{
				PopupSceneSequence firstScriptOfType = gameEntity.GetFirstScriptOfType<PopupSceneSequence>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.SetInitialState();
				}
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000694C File Offset: 0x00004B4C
		public void SetPositiveState()
		{
			if (this._positiveStateActionCode != null)
			{
				AgentVisuals humanAgentVisuals = this._humanAgentVisuals;
				if (humanAgentVisuals != null)
				{
					humanAgentVisuals.SetAction(this._positiveStateActionCode, 0f, true);
				}
				AgentVisuals mountAgentVisuals = this._mountAgentVisuals;
				if (mountAgentVisuals != null)
				{
					mountAgentVisuals.SetAction(this._positiveStateActionCode, 0f, true);
				}
			}
			if (!string.IsNullOrEmpty(this.PositiveFaceAnimCode))
			{
				MBSkeletonExtensions.SetFacialAnimation(this._humanAgentVisuals.GetVisuals().GetSkeleton(), 1, this.PositiveFaceAnimCode, false, true);
			}
			foreach (GameEntity gameEntity in base.GameEntity.Scene.FindEntitiesWithTag(base.GameEntity.Name))
			{
				PopupSceneSequence firstScriptOfType = gameEntity.GetFirstScriptOfType<PopupSceneSequence>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.SetPositiveState();
				}
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006A2C File Offset: 0x00004C2C
		public void SetNegativeState()
		{
			if (this._negativeStateActionCode != null)
			{
				AgentVisuals humanAgentVisuals = this._humanAgentVisuals;
				if (humanAgentVisuals != null)
				{
					humanAgentVisuals.SetAction(this._negativeStateActionCode, 0f, true);
				}
				AgentVisuals mountAgentVisuals = this._mountAgentVisuals;
				if (mountAgentVisuals != null)
				{
					mountAgentVisuals.SetAction(this._negativeStateActionCode, 0f, true);
				}
			}
			if (!string.IsNullOrEmpty(this.NegativeFaceAnimCode))
			{
				MBSkeletonExtensions.SetFacialAnimation(this._humanAgentVisuals.GetVisuals().GetSkeleton(), 1, this.NegativeFaceAnimCode, false, true);
			}
			foreach (GameEntity gameEntity in base.GameEntity.Scene.FindEntitiesWithTag(base.GameEntity.Name))
			{
				PopupSceneSequence firstScriptOfType = gameEntity.GetFirstScriptOfType<PopupSceneSequence>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.SetNegativeState();
				}
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006B0C File Offset: 0x00004D0C
		public void Destroy()
		{
			AgentVisuals humanAgentVisuals = this._humanAgentVisuals;
			if (humanAgentVisuals != null)
			{
				humanAgentVisuals.Reset();
			}
			this._humanAgentVisuals = null;
			AgentVisuals mountAgentVisuals = this._mountAgentVisuals;
			if (mountAgentVisuals != null)
			{
				mountAgentVisuals.Reset();
			}
			this._mountAgentVisuals = null;
			this._initialStateActionCode = null;
			this._positiveStateActionCode = null;
			this._negativeStateActionCode = null;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006B5E File Offset: 0x00004D5E
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006B68 File Offset: 0x00004D68
		protected override void OnTick(float dt)
		{
			AgentVisuals mountAgentVisuals = this._mountAgentVisuals;
			if (mountAgentVisuals != null)
			{
				mountAgentVisuals.Tick(null, dt, false, 0f);
			}
			AgentVisuals mountAgentVisuals2 = this._mountAgentVisuals;
			if (mountAgentVisuals2 != null)
			{
				GameEntity entity = mountAgentVisuals2.GetEntity();
				if (entity != null)
				{
					Skeleton skeleton = entity.Skeleton;
					if (skeleton != null)
					{
						skeleton.ForceUpdateBoneFrames();
					}
				}
			}
			AgentVisuals humanAgentVisuals = this._humanAgentVisuals;
			if (humanAgentVisuals != null)
			{
				humanAgentVisuals.Tick(this._mountAgentVisuals, dt, false, 0f);
			}
			AgentVisuals humanAgentVisuals2 = this._humanAgentVisuals;
			if (humanAgentVisuals2 == null)
			{
				return;
			}
			GameEntity entity2 = humanAgentVisuals2.GetEntity();
			if (entity2 == null)
			{
				return;
			}
			Skeleton skeleton2 = entity2.Skeleton;
			if (skeleton2 == null)
			{
				return;
			}
			skeleton2.ForceUpdateBoneFrames();
		}

		// Token: 0x04000029 RID: 41
		public string InitialAction = "";

		// Token: 0x0400002A RID: 42
		public string NegativeAction = "";

		// Token: 0x0400002B RID: 43
		public string InitialFaceAnimCode = "";

		// Token: 0x0400002C RID: 44
		public string PositiveFaceAnimCode = "";

		// Token: 0x0400002D RID: 45
		public string NegativeFaceAnimCode = "";

		// Token: 0x0400002E RID: 46
		public string PositiveAction = "";

		// Token: 0x0400002F RID: 47
		public string LeftHandWieldedItem = "";

		// Token: 0x04000030 RID: 48
		public string RightHandWieldedItem = "";

		// Token: 0x04000031 RID: 49
		public string BannerTagToUseForAddedPrefab = "";

		// Token: 0x04000032 RID: 50
		public bool StartWithRandomProgress;

		// Token: 0x04000033 RID: 51
		public Vec3 AttachedPrefabOffset = Vec3.Zero;

		// Token: 0x04000034 RID: 52
		public string PrefabItem = "";

		// Token: 0x04000035 RID: 53
		public HumanBone PrefabBone = 27;

		// Token: 0x04000036 RID: 54
		private AgentVisuals _mountAgentVisuals;

		// Token: 0x04000037 RID: 55
		private AgentVisuals _humanAgentVisuals;

		// Token: 0x04000038 RID: 56
		private ActionIndexCache _initialStateActionCode;

		// Token: 0x04000039 RID: 57
		private ActionIndexCache _positiveStateActionCode;

		// Token: 0x0400003A RID: 58
		private ActionIndexCache _negativeStateActionCode;

		// Token: 0x0400003B RID: 59
		private static readonly ActionIndexCache default_act_horse_stand = ActionIndexCache.Create("act_horse_stand_1");

		// Token: 0x0400003C RID: 60
		private static readonly ActionIndexCache default_act_camel_stand = ActionIndexCache.Create("act_camel_stand_1");
	}
}
