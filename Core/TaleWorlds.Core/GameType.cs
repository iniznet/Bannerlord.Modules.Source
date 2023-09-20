﻿using System;
using System.Collections.Generic;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x0200002B RID: 43
	public abstract class GameType
	{
		// Token: 0x060002C6 RID: 710 RVA: 0x0000C401 File Offset: 0x0000A601
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x0000C403 File Offset: 0x0000A603
		public virtual bool SupportsSaving
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0000C406 File Offset: 0x0000A606
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x0000C40E File Offset: 0x0000A60E
		public Game CurrentGame { get; internal set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000C417 File Offset: 0x0000A617
		public MBObjectManager ObjectManager
		{
			get
			{
				return this.CurrentGame.ObjectManager;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060002CB RID: 715 RVA: 0x0000C424 File Offset: 0x0000A624
		public GameManagerBase GameManager
		{
			get
			{
				return this.CurrentGame.GameManager;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000C431 File Offset: 0x0000A631
		public virtual bool IsInventoryAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060002CD RID: 717 RVA: 0x0000C434 File Offset: 0x0000A634
		public virtual bool IsQuestScreenAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000C437 File Offset: 0x0000A637
		public virtual bool IsCharacterWindowAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0000C43A File Offset: 0x0000A63A
		public virtual bool IsPartyWindowAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000C43D File Offset: 0x0000A63D
		public virtual bool IsKingdomWindowAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x0000C440 File Offset: 0x0000A640
		public virtual bool IsClanWindowAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0000C443 File Offset: 0x0000A643
		public virtual bool IsEncyclopediaWindowAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x0000C446 File Offset: 0x0000A646
		public virtual bool IsBannerWindowAccessibleAtMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000C449 File Offset: 0x0000A649
		public virtual bool IsDevelopment
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x0000C44C File Offset: 0x0000A64C
		public virtual bool IsCoreOnlyGameMode
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000C44F File Offset: 0x0000A64F
		public GameType()
		{
			this._stepNo = GameTypeLoadingStates.InitializeFirstStep;
		}

		// Token: 0x060002D7 RID: 727
		public abstract void OnStateChanged(GameState oldState);

		// Token: 0x060002D8 RID: 728
		protected internal abstract void BeforeRegisterTypes(MBObjectManager objectManager);

		// Token: 0x060002D9 RID: 729
		protected internal abstract void OnRegisterTypes(MBObjectManager objectManager);

		// Token: 0x060002DA RID: 730
		protected internal abstract void OnInitialize();

		// Token: 0x060002DB RID: 731
		protected abstract void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState);

		// Token: 0x060002DC RID: 732 RVA: 0x0000C460 File Offset: 0x0000A660
		public bool DoLoadingForGameType()
		{
			bool flag = false;
			GameTypeLoadingStates gameTypeLoadingStates = GameTypeLoadingStates.None;
			switch (this._stepNo)
			{
			case GameTypeLoadingStates.InitializeFirstStep:
				this.DoLoadingForGameType(GameTypeLoadingStates.InitializeFirstStep, out gameTypeLoadingStates);
				if (gameTypeLoadingStates == GameTypeLoadingStates.WaitSecondStep)
				{
					this._stepNo++;
				}
				break;
			case GameTypeLoadingStates.WaitSecondStep:
				this.DoLoadingForGameType(GameTypeLoadingStates.WaitSecondStep, out gameTypeLoadingStates);
				if (gameTypeLoadingStates == GameTypeLoadingStates.LoadVisualsThirdState)
				{
					this._stepNo++;
				}
				break;
			case GameTypeLoadingStates.LoadVisualsThirdState:
				this.DoLoadingForGameType(GameTypeLoadingStates.LoadVisualsThirdState, out gameTypeLoadingStates);
				if (gameTypeLoadingStates == GameTypeLoadingStates.PostInitializeFourthState)
				{
					this._stepNo++;
				}
				break;
			case GameTypeLoadingStates.PostInitializeFourthState:
				this.DoLoadingForGameType(GameTypeLoadingStates.PostInitializeFourthState, out gameTypeLoadingStates);
				if (gameTypeLoadingStates == GameTypeLoadingStates.None)
				{
					this._stepNo++;
					flag = true;
				}
				break;
			}
			return flag;
		}

		// Token: 0x060002DD RID: 733
		public abstract void OnDestroy();

		// Token: 0x060002DE RID: 734 RVA: 0x0000C505 File Offset: 0x0000A705
		public virtual void OnMissionIsStarting(string missionName, MissionInitializerRecord rec)
		{
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000C507 File Offset: 0x0000A707
		public virtual void InitializeParameters()
		{
		}

		// Token: 0x040001B3 RID: 435
		private GameTypeLoadingStates _stepNo;
	}
}
