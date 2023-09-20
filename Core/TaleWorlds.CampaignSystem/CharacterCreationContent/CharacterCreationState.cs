using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	public class CharacterCreationState : PlayerGameState
	{
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

		public CharacterCreationStageBase CurrentStage { get; private set; }

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

		public CharacterCreationState()
		{
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._stageIndex == -1 && this.CharacterCreation != null)
			{
				this.NextStage();
			}
		}

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

		public int GetTotalStagesCount()
		{
			return this._stages.Count;
		}

		public int GetIndexOfCurrentStage()
		{
			return this._stageIndex;
		}

		public int GetFurthestIndex()
		{
			return this._furthestStageIndex;
		}

		public void GoToStage(int stageIndex)
		{
			if (stageIndex >= 0 && stageIndex < this._stages.Count && stageIndex != this._stageIndex && stageIndex <= this._furthestStageIndex)
			{
				this._stageIndex = stageIndex + 1;
				this.PreviousStage();
			}
		}

		private CharacterCreation _characterCreation;

		private ICharacterCreationStateHandler _handler;

		private readonly List<KeyValuePair<int, Type>> _stages;

		private int _stageIndex = -1;

		private int _furthestStageIndex;

		public readonly CharacterCreationContentBase CurrentCharacterCreationContent;
	}
}
