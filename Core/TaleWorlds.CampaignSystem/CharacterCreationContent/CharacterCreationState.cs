using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001E3 RID: 483
	public class CharacterCreationState : PlayerGameState
	{
		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06001C2B RID: 7211 RVA: 0x0007E42E File Offset: 0x0007C62E
		// (set) Token: 0x06001C2C RID: 7212 RVA: 0x0007E436 File Offset: 0x0007C636
		public CharacterCreation CharacterCreation
		{
			get
			{
				return this._characterCreation;
			}
			private set
			{
				this._characterCreation = value;
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06001C2D RID: 7213 RVA: 0x0007E43F File Offset: 0x0007C63F
		// (set) Token: 0x06001C2E RID: 7214 RVA: 0x0007E447 File Offset: 0x0007C647
		public ICharacterCreationStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06001C2F RID: 7215 RVA: 0x0007E450 File Offset: 0x0007C650
		// (set) Token: 0x06001C30 RID: 7216 RVA: 0x0007E458 File Offset: 0x0007C658
		public CharacterCreationStageBase CurrentStage { get; private set; }

		// Token: 0x06001C31 RID: 7217 RVA: 0x0007E464 File Offset: 0x0007C664
		public CharacterCreationState(CharacterCreationContentBase baseContent)
		{
			this.CharacterCreation = new CharacterCreation();
			this.CurrentCharacterCreationContent = baseContent;
			this.CurrentCharacterCreationContent.Initialize(this.CharacterCreation);
			this._stages = new List<KeyValuePair<int, Type>>();
			int num = 0;
			foreach (Type type in this.CurrentCharacterCreationContent.CharacterCreationStages)
			{
				if (type.IsSubclassOf(typeof(CharacterCreationStageBase)))
				{
					this._stages.Add(new KeyValuePair<int, Type>(num, type));
				}
				else
				{
					Debug.FailedAssert("Invalid character creation stage type: " + type.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CharacterCreationContent\\CharacterCreationState.cs", ".ctor", 54);
				}
			}
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x0007E534 File Offset: 0x0007C734
		public CharacterCreationState()
		{
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x0007E543 File Offset: 0x0007C743
		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x0007E55B File Offset: 0x0007C75B
		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._stageIndex == -1 && this.CharacterCreation != null)
			{
				this.NextStage();
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x0007E57C File Offset: 0x0007C77C
		public void FinalizeCharacterCreation()
		{
			this.CharacterCreation.ApplyFinalEffects();
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
			Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<MapState>(), 0);
			PartyBase.MainParty.Visuals.SetMapIconAsDirty();
			ICharacterCreationStateHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnCharacterCreationFinalized();
			}
			this.CurrentCharacterCreationContent.OnCharacterCreationFinalized();
			CampaignEventDispatcher.Instance.OnCharacterCreationIsOver();
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x0007E5F8 File Offset: 0x0007C7F8
		public void NextStage()
		{
			this._stageIndex++;
			CharacterCreationStageBase currentStage = this.CurrentStage;
			if (currentStage != null)
			{
				currentStage.OnFinalize();
			}
			this._furthestStageIndex = MathF.Max(this._furthestStageIndex, this._stageIndex);
			if (this._stageIndex == this._stages.Count)
			{
				this.FinalizeCharacterCreation();
				return;
			}
			Type value = this._stages[this._stageIndex].Value;
			this.CreateStage(value);
			this.Refresh();
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x0007E67C File Offset: 0x0007C87C
		public void PreviousStage()
		{
			CharacterCreationStageBase currentStage = this.CurrentStage;
			if (currentStage != null)
			{
				currentStage.OnFinalize();
			}
			this._stageIndex--;
			Type value = this._stages[this._stageIndex].Value;
			this.CreateStage(value);
			this.Refresh();
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x0007E6CF File Offset: 0x0007C8CF
		private void CreateStage(Type type)
		{
			this.CurrentStage = Activator.CreateInstance(type, new object[] { this }) as CharacterCreationStageBase;
			ICharacterCreationStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnStageCreated(this.CurrentStage);
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x0007E704 File Offset: 0x0007C904
		public void Refresh()
		{
			if (this.CurrentStage == null || this._stageIndex < 0 || this._stageIndex >= this._stages.Count)
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CharacterCreationContent\\CharacterCreationState.cs", "Refresh", 139);
				return;
			}
			ICharacterCreationStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRefresh();
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x0007E75F File Offset: 0x0007C95F
		public int GetTotalStagesCount()
		{
			return this._stages.Count;
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x0007E76C File Offset: 0x0007C96C
		public int GetIndexOfCurrentStage()
		{
			return this._stageIndex;
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x0007E774 File Offset: 0x0007C974
		public int GetFurthestIndex()
		{
			return this._furthestStageIndex;
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x0007E77C File Offset: 0x0007C97C
		public void GoToStage(int stageIndex)
		{
			if (stageIndex >= 0 && stageIndex < this._stages.Count && stageIndex != this._stageIndex && stageIndex <= this._furthestStageIndex)
			{
				this._stageIndex = stageIndex + 1;
				this.PreviousStage();
			}
		}

		// Token: 0x040008E9 RID: 2281
		private CharacterCreation _characterCreation;

		// Token: 0x040008EA RID: 2282
		private ICharacterCreationStateHandler _handler;

		// Token: 0x040008EB RID: 2283
		private readonly List<KeyValuePair<int, Type>> _stages;

		// Token: 0x040008EC RID: 2284
		private int _stageIndex = -1;

		// Token: 0x040008EE RID: 2286
		private int _furthestStageIndex;

		// Token: 0x040008EF RID: 2287
		public readonly CharacterCreationContentBase CurrentCharacterCreationContent;
	}
}
