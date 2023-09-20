using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.AnimationPoints
{
	public class PlayMusicPoint : AnimationPoint
	{
		protected override void OnInit()
		{
			base.OnInit();
			this.KeepOldVisibility = true;
			base.IsDisabledForPlayers = true;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public void StartLoop(SoundEvent trackEvent)
		{
			this._trackEvent = trackEvent;
			if (base.HasUser && MBActionSet.CheckActionAnimationClipExists(base.UserAgent.ActionSet, this.LoopStartActionCode))
			{
				base.UserAgent.SetActionChannel(0, this.LoopStartActionCode, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		public void EndLoop()
		{
			if (this._trackEvent != null)
			{
				this._trackEvent = null;
				this.ChangeInstrument(null);
			}
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.HasUser)
			{
				return 2 | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._trackEvent != null && base.HasUser && MBActionSet.CheckActionAnimationClipExists(base.UserAgent.ActionSet, this.LoopStartActionCode))
			{
				base.UserAgent.SetActionChannel(0, this.LoopStartActionCode, this._hasInstrumentAttached, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			this.DefaultActionCode = ActionIndexCache.act_none;
			this.EndLoop();
		}

		public void ChangeInstrument(Tuple<InstrumentData, float> instrument)
		{
			InstrumentData instrumentData = ((instrument != null) ? instrument.Item1 : null);
			if (this._instrumentData != instrumentData)
			{
				this._instrumentData = instrumentData;
				if (base.HasUser && base.UserAgent.IsActive())
				{
					if (base.UserAgent.IsSitting())
					{
						this.LoopStartAction = ((instrumentData == null) ? "act_sit_1" : instrumentData.SittingAction);
					}
					else
					{
						this.LoopStartAction = ((instrumentData == null) ? "act_stand_1" : instrumentData.StandingAction);
					}
					this.ActionSpeed = ((instrument != null) ? instrument.Item2 : 1f);
					this.SetActionCodes();
					base.ClearAssignedItems();
					base.UserAgent.SetActionChannel(0, this.LoopStartActionCode, false, (ulong)((long)base.UserAgent.GetCurrentActionPriority(0)), 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					if (this._instrumentData != null)
					{
						foreach (ValueTuple<HumanBone, string> valueTuple in this._instrumentData.InstrumentEntities)
						{
							AnimationPoint.ItemForBone itemForBone = new AnimationPoint.ItemForBone(valueTuple.Item1, valueTuple.Item2, true);
							base.AssignItemToBone(itemForBone);
						}
						base.AddItemsToAgent();
						this._hasInstrumentAttached = !this._instrumentData.IsDataWithoutInstrument;
					}
				}
			}
		}

		private InstrumentData _instrumentData;

		private SoundEvent _trackEvent;

		private bool _hasInstrumentAttached;
	}
}
