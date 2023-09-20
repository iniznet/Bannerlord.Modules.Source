using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Scripts;

namespace TaleWorlds.MountAndBlade.View
{
	public class PopupSceneSpawnPoint : ScriptComponentBehavior
	{
		public CompositeComponent AddedPrefabComponent { get; private set; }

		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

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

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

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

		public string InitialAction = "";

		public string NegativeAction = "";

		public string InitialFaceAnimCode = "";

		public string PositiveFaceAnimCode = "";

		public string NegativeFaceAnimCode = "";

		public string PositiveAction = "";

		public string LeftHandWieldedItem = "";

		public string RightHandWieldedItem = "";

		public string BannerTagToUseForAddedPrefab = "";

		public bool StartWithRandomProgress;

		public Vec3 AttachedPrefabOffset = Vec3.Zero;

		public string PrefabItem = "";

		public HumanBone PrefabBone = 27;

		private AgentVisuals _mountAgentVisuals;

		private AgentVisuals _humanAgentVisuals;

		private ActionIndexCache _initialStateActionCode;

		private ActionIndexCache _positiveStateActionCode;

		private ActionIndexCache _negativeStateActionCode;

		private static readonly ActionIndexCache default_act_horse_stand = ActionIndexCache.Create("act_horse_stand_1");

		private static readonly ActionIndexCache default_act_camel_stand = ActionIndexCache.Create("act_camel_stand_1");
	}
}
