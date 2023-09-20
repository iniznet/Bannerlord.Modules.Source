using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.AnimationPoints
{
	// Token: 0x02000032 RID: 50
	public class PlayMusicPoint : AnimationPoint
	{
		// Token: 0x0600025C RID: 604 RVA: 0x00010391 File Offset: 0x0000E591
		protected override void OnInit()
		{
			base.OnInit();
			this.KeepOldVisibility = true;
			base.IsDisabledForPlayers = true;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x0600025D RID: 605 RVA: 0x000103B4 File Offset: 0x0000E5B4
		public void StartLoop(SoundEvent trackEvent)
		{
			this._trackEvent = trackEvent;
			if (base.HasUser && MBActionSet.CheckActionAnimationClipExists(base.UserAgent.ActionSet, this.LoopStartActionCode))
			{
				base.UserAgent.SetActionChannel(0, this.LoopStartActionCode, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0001041F File Offset: 0x0000E61F
		public void EndLoop()
		{
			if (this._trackEvent != null)
			{
				this._trackEvent = null;
				this.ChangeInstrument(null);
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00010437 File Offset: 0x0000E637
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.HasUser)
			{
				return 2 | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00010450 File Offset: 0x0000E650
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._trackEvent != null && base.HasUser && MBActionSet.CheckActionAnimationClipExists(base.UserAgent.ActionSet, this.LoopStartActionCode))
			{
				base.UserAgent.SetActionChannel(0, this.LoopStartActionCode, this._hasInstrumentAttached, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x000104C8 File Offset: 0x0000E6C8
		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			this.DefaultActionCode = ActionIndexCache.act_none;
			this.EndLoop();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000104E4 File Offset: 0x0000E6E4
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

		// Token: 0x04000137 RID: 311
		private InstrumentData _instrumentData;

		// Token: 0x04000138 RID: 312
		private SoundEvent _trackEvent;

		// Token: 0x04000139 RID: 313
		private bool _hasInstrumentAttached;
	}
}
