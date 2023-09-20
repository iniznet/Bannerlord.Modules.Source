using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Conversation
{
	// Token: 0x020000F4 RID: 244
	public class MissionConversationVM : ViewModel
	{
		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x060016BC RID: 5820 RVA: 0x0005462D File Offset: 0x0005282D
		// (set) Token: 0x060016BD RID: 5821 RVA: 0x00054635 File Offset: 0x00052835
		public bool SelectedAnOptionOrLinkThisFrame { get; set; }

		// Token: 0x060016BE RID: 5822 RVA: 0x00054640 File Offset: 0x00052840
		public MissionConversationVM(Func<string> getContinueInputText, bool isLinksDisabled = false)
		{
			this.AnswerList = new MBBindingList<ConversationItemVM>();
			this.AttackerParties = new MBBindingList<ConversationAggressivePartyItemVM>();
			this.DefenderParties = new MBBindingList<ConversationAggressivePartyItemVM>();
			this._conversationManager = Campaign.Current.ConversationManager;
			this._getContinueInputText = getContinueInputText;
			this._isLinksDisabled = isLinksDisabled;
			CampaignEvents.PersuasionProgressCommittedEvent.AddNonSerializedListener(this, new Action<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>(this.OnPersuasionProgress));
			this.Persuasion = new PersuasionVM(this._conversationManager);
			this.IsAggressive = Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter && this._conversationManager.ConversationParty != null && FactionManager.IsAtWarAgainstFaction(this._conversationManager.ConversationParty.MapFaction, Hero.MainHero.MapFaction);
			if (this.IsAggressive)
			{
				List<MobileParty> list = new List<MobileParty>();
				List<MobileParty> list2 = new List<MobileParty>();
				MobileParty conversationParty = this._conversationManager.ConversationParty;
				MobileParty mainParty = MobileParty.MainParty;
				if (PlayerEncounter.PlayerIsAttacker)
				{
					list2.Add(mainParty);
					list.Add(conversationParty);
					PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(ref list2, ref list);
				}
				else
				{
					list2.Add(conversationParty);
					list.Add(mainParty);
					PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(ref list, ref list2);
				}
				this.AttackerLeader = new ConversationAggressivePartyItemVM(PlayerEncounter.PlayerIsAttacker ? mainParty : conversationParty, null);
				this.DefenderLeader = new ConversationAggressivePartyItemVM(PlayerEncounter.PlayerIsAttacker ? conversationParty : mainParty, null);
				double num = 0.0;
				double num2 = 0.0;
				num += (double)this.DefenderLeader.Party.Party.TotalStrength;
				num2 += (double)this.AttackerLeader.Party.Party.TotalStrength;
				foreach (MobileParty mobileParty in list)
				{
					if (mobileParty != conversationParty && mobileParty != mainParty)
					{
						num += (double)mobileParty.Party.TotalStrength;
						this.DefenderParties.Add(new ConversationAggressivePartyItemVM(mobileParty, null));
					}
				}
				foreach (MobileParty mobileParty2 in list2)
				{
					if (mobileParty2 != conversationParty && mobileParty2 != mainParty)
					{
						num2 += (double)mobileParty2.Party.TotalStrength;
						this.AttackerParties.Add(new ConversationAggressivePartyItemVM(mobileParty2, null));
					}
				}
				string text;
				if (this.DefenderLeader.Party.MapFaction != null && this.DefenderLeader.Party.MapFaction is Kingdom)
				{
					text = Color.FromUint(((Kingdom)this.DefenderLeader.Party.MapFaction).PrimaryBannerColor).ToString();
				}
				else
				{
					text = Color.FromUint(this.DefenderLeader.Party.MapFaction.Banner.GetPrimaryColor()).ToString();
				}
				string text2;
				if (this.AttackerLeader.Party.MapFaction != null && this.AttackerLeader.Party.MapFaction is Kingdom)
				{
					text2 = Color.FromUint(((Kingdom)this.AttackerLeader.Party.MapFaction).PrimaryBannerColor).ToString();
				}
				else
				{
					text2 = Color.FromUint(this.AttackerLeader.Party.MapFaction.Banner.GetPrimaryColor()).ToString();
				}
				this.PowerComparer = new PowerLevelComparer(num, num2);
				this.PowerComparer.SetColors(text, text2);
			}
			else
			{
				this.DefenderLeader = new ConversationAggressivePartyItemVM(null, null);
				this.AttackerLeader = new ConversationAggressivePartyItemVM(null, null);
			}
			if (this._conversationManager.SpeakerAgent != null && (CharacterObject)this._conversationManager.SpeakerAgent.Character != null && ((CharacterObject)this._conversationManager.SpeakerAgent.Character).IsHero && this._conversationManager.SpeakerAgent.Character != CharacterObject.PlayerCharacter)
			{
				Hero heroObject = ((CharacterObject)this._conversationManager.SpeakerAgent.Character).HeroObject;
				this.Relation = (int)heroObject.GetRelationWithPlayer();
			}
			this.ExecuteSetCurrentAnswer(null);
			this.RefreshValues();
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x00054A90 File Offset: 0x00052C90
		private void OnPersuasionProgress(Tuple<PersuasionOptionArgs, PersuasionOptionResult> result)
		{
			PersuasionVM persuasion = this.Persuasion;
			if (persuasion != null)
			{
				persuasion.OnPersuasionProgress(result);
			}
			this.AnswerList.ApplyActionOnAllItems(delegate(ConversationItemVM a)
			{
				a.OnPersuasionProgress(result);
			});
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x00054AD8 File Offset: 0x00052CD8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ContinueText = this._getContinueInputText();
			this.MoreOptionText = GameTexts.FindText("str_more_brackets", null).ToString();
			this.PersuasionText = GameTexts.FindText("str_persuasion", null).ToString();
			this.RelationHint = new HintViewModel(GameTexts.FindText("str_tooltip_label_relation", null), null);
			this.GoldHint = new HintViewModel(new TextObject("{=o5G8A8ZH}Your Denars", null), null);
			this._answerList.ApplyActionOnAllItems(delegate(ConversationItemVM x)
			{
				x.RefreshValues();
			});
			this._defenderParties.ApplyActionOnAllItems(delegate(ConversationAggressivePartyItemVM x)
			{
				x.RefreshValues();
			});
			this._attackerParties.ApplyActionOnAllItems(delegate(ConversationAggressivePartyItemVM x)
			{
				x.RefreshValues();
			});
			this._defenderLeader.RefreshValues();
			this._attackerLeader.RefreshValues();
			this._currentSelectedAnswer.RefreshValues();
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00054BF5 File Offset: 0x00052DF5
		public void OnConversationContinue()
		{
			if (ConversationManager.GetPersuasionIsActive() && (!ConversationManager.GetPersuasionIsActive() || this.IsPersuading))
			{
				List<ConversationSentenceOption> curOptions = this._conversationManager.CurOptions;
				if (((curOptions != null) ? curOptions.Count : 0) > 1)
				{
					return;
				}
			}
			this.Refresh();
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00054C2D File Offset: 0x00052E2D
		public void ExecuteLink(string link)
		{
			if (!this._isLinksDisabled)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(link);
			}
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x00054C48 File Offset: 0x00052E48
		public void ExecuteConversedHeroLink()
		{
			CharacterObject characterObject;
			if (!this._isLinksDisabled && (characterObject = this._currentDialogCharacter as CharacterObject) != null)
			{
				EncyclopediaManager encyclopediaManager = Campaign.Current.EncyclopediaManager;
				Hero heroObject = characterObject.HeroObject;
				encyclopediaManager.GoToLink(((heroObject != null) ? heroObject.EncyclopediaLink : null) ?? characterObject.EncyclopediaLink);
				this.SelectedAnOptionOrLinkThisFrame = true;
			}
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x00054CA0 File Offset: 0x00052EA0
		public void Refresh()
		{
			this.ExecuteCloseTooltip();
			this._isProcessingOption = false;
			this.IsLoadingOver = false;
			IReadOnlyList<IAgent> conversationAgents = this._conversationManager.ConversationAgents;
			if (conversationAgents != null && conversationAgents.Count > 0)
			{
				this._currentDialogCharacter = this._conversationManager.SpeakerAgent.Character;
				this.CurrentCharacterNameLbl = this._currentDialogCharacter.Name.ToString();
				this.IsCurrentCharacterValidInEncyclopedia = false;
				if (((CharacterObject)this._currentDialogCharacter).IsHero && this._currentDialogCharacter != CharacterObject.PlayerCharacter)
				{
					this.MinRelation = Campaign.Current.Models.DiplomacyModel.MinRelationLimit;
					this.MaxRelation = Campaign.Current.Models.DiplomacyModel.MaxRelationLimit;
					Hero heroObject = ((CharacterObject)this._currentDialogCharacter).HeroObject;
					if (heroObject.IsLord && !heroObject.IsMinorFactionHero)
					{
						Clan clan = heroObject.Clan;
						if (((clan != null) ? clan.Leader : null) == heroObject)
						{
							Clan clan2 = heroObject.Clan;
							if (((clan2 != null) ? clan2.Kingdom : null) != null)
							{
								string stringId = heroObject.MapFaction.Culture.StringId;
								TextObject textObject;
								if (GameTexts.TryGetText("str_faction_noble_name_with_title", out textObject, stringId))
								{
									if (heroObject.Clan.Kingdom.Leader == heroObject)
									{
										textObject = GameTexts.FindText("str_faction_ruler_name_with_title", stringId);
									}
									StringHelpers.SetCharacterProperties("RULER", (CharacterObject)this._currentDialogCharacter, null, false);
									this.CurrentCharacterNameLbl = textObject.ToString();
								}
							}
						}
					}
					this.IsRelationEnabled = true;
					this.Relation = Hero.MainHero.GetRelation(heroObject);
					GameTexts.SetVariable("NUM", this.Relation.ToString());
					if (this.Relation > 0)
					{
						this.RelationText = "+" + this.Relation;
					}
					else if (this.Relation < 0)
					{
						this.RelationText = "-" + MathF.Abs(this.Relation);
					}
					else
					{
						this.RelationText = this.Relation.ToString();
					}
					if (heroObject.Clan == null || heroObject.Clan == CampaignData.NeutralFaction)
					{
						this.ConversedHeroBanner = new ImageIdentifierVM(ImageIdentifierType.Null);
						this.IsRelationEnabled = false;
						this.IsBannerEnabled = false;
					}
					else
					{
						this.ConversedHeroBanner = ((heroObject != null) ? new ImageIdentifierVM(heroObject.ClanBanner) : new ImageIdentifierVM(ImageIdentifierType.Null));
						TextObject textObject2 = ((heroObject != null) ? heroObject.Clan.Name : TextObject.Empty);
						this.FactionHint = new HintViewModel(textObject2, null);
						this.IsBannerEnabled = true;
					}
					this.IsCurrentCharacterValidInEncyclopedia = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero)).IsValidEncyclopediaItem(heroObject);
				}
				else
				{
					this.ConversedHeroBanner = new ImageIdentifierVM(ImageIdentifierType.Null);
					this.IsRelationEnabled = false;
					this.IsBannerEnabled = false;
					this.IsCurrentCharacterValidInEncyclopedia = Campaign.Current.EncyclopediaManager.GetPageOf(typeof(CharacterObject)).IsValidEncyclopediaItem((CharacterObject)this._conversationManager.SpeakerAgent.Character);
				}
			}
			this.DialogText = this._conversationManager.CurrentSentenceText;
			this.AnswerList.Clear();
			MissionConversationVM._isCurrentlyPlayerSpeaking = this._conversationManager.SpeakerAgent.Character == Hero.MainHero.CharacterObject;
			this._conversationManager.GetPlayerSentenceOptions();
			List<ConversationSentenceOption> curOptions = this._conversationManager.CurOptions;
			int num = ((curOptions != null) ? curOptions.Count : 0);
			if (num > 0 && !MissionConversationVM._isCurrentlyPlayerSpeaking)
			{
				for (int i = 0; i < num; i++)
				{
					this.AnswerList.Add(new ConversationItemVM(new Action<int>(this.OnSelectOption), new Action(this.OnReadyToContinue), new Action<ConversationItemVM>(this.ExecuteSetCurrentAnswer), i, this._conversationManager.CurOptions[i]));
				}
			}
			this.GoldText = CampaignUIHelper.GetAbbreviatedValueTextFromValue(Hero.MainHero.Gold);
			this.IsPersuading = ConversationManager.GetPersuasionIsActive();
			if (this.IsPersuading)
			{
				this.CurrentSelectedAnswer = new ConversationItemVM();
			}
			this.IsLoadingOver = true;
			this.Persuasion.RefreshPersusasion();
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x000550B1 File Offset: 0x000532B1
		private void OnReadyToContinue()
		{
			this.Refresh();
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x000550BC File Offset: 0x000532BC
		private void ExecuteDefenderTooltip()
		{
			if (PlayerEncounter.PlayerIsDefender)
			{
				InformationManager.ShowTooltip(typeof(List<MobileParty>), new object[] { 0 });
				return;
			}
			InformationManager.ShowTooltip(typeof(List<MobileParty>), new object[] { 1 });
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x0005510D File Offset: 0x0005330D
		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x00055114 File Offset: 0x00053314
		public void ExecuteHeroTooltip()
		{
			CharacterObject characterObject = (CharacterObject)this._currentDialogCharacter;
			if (characterObject != null && characterObject.IsHero)
			{
				InformationManager.ShowTooltip(typeof(Hero), new object[] { characterObject.HeroObject, true });
			}
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00055160 File Offset: 0x00053360
		private void ExecuteAttackerTooltip()
		{
			if (PlayerEncounter.PlayerIsAttacker)
			{
				InformationManager.ShowTooltip(typeof(List<MobileParty>), new object[] { 0 });
				return;
			}
			InformationManager.ShowTooltip(typeof(List<MobileParty>), new object[] { 1 });
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x000551B4 File Offset: 0x000533B4
		private void ExecuteHeroInfo()
		{
			if (this._conversationManager.ListenerAgent.Character == Hero.MainHero.CharacterObject)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.EncyclopediaLink);
				return;
			}
			if (CharacterObject.OneToOneConversationCharacter.IsHero)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(CharacterObject.OneToOneConversationCharacter.HeroObject.EncyclopediaLink);
				return;
			}
			Campaign.Current.EncyclopediaManager.GoToLink(CharacterObject.OneToOneConversationCharacter.EncyclopediaLink);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x0005523B File Offset: 0x0005343B
		private void OnSelectOption(int optionIndex)
		{
			if (!this._isProcessingOption)
			{
				this._isProcessingOption = true;
				this._conversationManager.DoOption(optionIndex);
				PersuasionVM persuasion = this.Persuasion;
				if (persuasion != null)
				{
					persuasion.RefreshPersusasion();
				}
				this.SelectedAnOptionOrLinkThisFrame = true;
			}
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x00055270 File Offset: 0x00053470
		public void ExecuteFinalizeSelection()
		{
			this.Refresh();
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x00055278 File Offset: 0x00053478
		public void ExecuteContinue()
		{
			Debug.Print("ExecuteContinue", 0, Debug.DebugColor.White, 17592186044416UL);
			this._conversationManager.ContinueConversation();
			this._isProcessingOption = false;
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x000552A2 File Offset: 0x000534A2
		private void ExecuteSetCurrentAnswer(ConversationItemVM _answer)
		{
			this.Persuasion.SetCurrentOption((_answer != null) ? _answer.PersuasionItem : null);
			if (_answer != null)
			{
				this.CurrentSelectedAnswer = _answer;
				return;
			}
			this.CurrentSelectedAnswer = new ConversationItemVM();
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x000552D1 File Offset: 0x000534D1
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.PersuasionProgressCommittedEvent.ClearListeners(this);
			PersuasionVM persuasion = this.Persuasion;
			if (persuasion == null)
			{
				return;
			}
			persuasion.OnFinalize();
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x060016D0 RID: 5840 RVA: 0x000552F4 File Offset: 0x000534F4
		// (set) Token: 0x060016D1 RID: 5841 RVA: 0x000552FC File Offset: 0x000534FC
		[DataSourceProperty]
		public PersuasionVM Persuasion
		{
			get
			{
				return this._persuasion;
			}
			set
			{
				if (value != this._persuasion)
				{
					this._persuasion = value;
					base.OnPropertyChangedWithValue<PersuasionVM>(value, "Persuasion");
				}
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x060016D2 RID: 5842 RVA: 0x0005531A File Offset: 0x0005351A
		// (set) Token: 0x060016D3 RID: 5843 RVA: 0x00055322 File Offset: 0x00053522
		[DataSourceProperty]
		public PowerLevelComparer PowerComparer
		{
			get
			{
				return this._powerComparer;
			}
			set
			{
				if (value != this._powerComparer)
				{
					this._powerComparer = value;
					base.OnPropertyChangedWithValue<PowerLevelComparer>(value, "PowerComparer");
				}
			}
		}

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x060016D4 RID: 5844 RVA: 0x00055340 File Offset: 0x00053540
		// (set) Token: 0x060016D5 RID: 5845 RVA: 0x00055348 File Offset: 0x00053548
		[DataSourceProperty]
		public int Relation
		{
			get
			{
				return this._relation;
			}
			set
			{
				if (this._relation != value)
				{
					this._relation = value;
					base.OnPropertyChangedWithValue(value, "Relation");
				}
			}
		}

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x060016D6 RID: 5846 RVA: 0x00055366 File Offset: 0x00053566
		// (set) Token: 0x060016D7 RID: 5847 RVA: 0x0005536E File Offset: 0x0005356E
		[DataSourceProperty]
		public int MinRelation
		{
			get
			{
				return this._minRelation;
			}
			set
			{
				if (this._minRelation != value)
				{
					this._minRelation = value;
					base.OnPropertyChangedWithValue(value, "MinRelation");
				}
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x060016D8 RID: 5848 RVA: 0x0005538C File Offset: 0x0005358C
		// (set) Token: 0x060016D9 RID: 5849 RVA: 0x00055394 File Offset: 0x00053594
		[DataSourceProperty]
		public int MaxRelation
		{
			get
			{
				return this._maxRelation;
			}
			set
			{
				if (this._maxRelation != value)
				{
					this._maxRelation = value;
					base.OnPropertyChangedWithValue(value, "MaxRelation");
				}
			}
		}

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x060016DA RID: 5850 RVA: 0x000553B2 File Offset: 0x000535B2
		// (set) Token: 0x060016DB RID: 5851 RVA: 0x000553BA File Offset: 0x000535BA
		[DataSourceProperty]
		public ConversationAggressivePartyItemVM DefenderLeader
		{
			get
			{
				return this._defenderLeader;
			}
			set
			{
				if (value != this._defenderLeader)
				{
					this._defenderLeader = value;
					base.OnPropertyChangedWithValue<ConversationAggressivePartyItemVM>(value, "DefenderLeader");
				}
			}
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x060016DC RID: 5852 RVA: 0x000553D8 File Offset: 0x000535D8
		// (set) Token: 0x060016DD RID: 5853 RVA: 0x000553E0 File Offset: 0x000535E0
		[DataSourceProperty]
		public ConversationAggressivePartyItemVM AttackerLeader
		{
			get
			{
				return this._attackerLeader;
			}
			set
			{
				if (value != this._attackerLeader)
				{
					this._attackerLeader = value;
					base.OnPropertyChangedWithValue<ConversationAggressivePartyItemVM>(value, "AttackerLeader");
				}
			}
		}

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x060016DE RID: 5854 RVA: 0x000553FE File Offset: 0x000535FE
		// (set) Token: 0x060016DF RID: 5855 RVA: 0x00055406 File Offset: 0x00053606
		[DataSourceProperty]
		public MBBindingList<ConversationAggressivePartyItemVM> AttackerParties
		{
			get
			{
				return this._attackerParties;
			}
			set
			{
				if (value != this._attackerParties)
				{
					this._attackerParties = value;
					base.OnPropertyChangedWithValue<MBBindingList<ConversationAggressivePartyItemVM>>(value, "AttackerParties");
				}
			}
		}

		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x060016E0 RID: 5856 RVA: 0x00055424 File Offset: 0x00053624
		// (set) Token: 0x060016E1 RID: 5857 RVA: 0x0005542C File Offset: 0x0005362C
		[DataSourceProperty]
		public MBBindingList<ConversationAggressivePartyItemVM> DefenderParties
		{
			get
			{
				return this._defenderParties;
			}
			set
			{
				if (value != this._defenderParties)
				{
					this._defenderParties = value;
					base.OnPropertyChangedWithValue<MBBindingList<ConversationAggressivePartyItemVM>>(value, "DefenderParties");
				}
			}
		}

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x060016E2 RID: 5858 RVA: 0x0005544A File Offset: 0x0005364A
		// (set) Token: 0x060016E3 RID: 5859 RVA: 0x00055452 File Offset: 0x00053652
		[DataSourceProperty]
		public string MoreOptionText
		{
			get
			{
				return this._moreOptionText;
			}
			set
			{
				if (this._moreOptionText != value)
				{
					this._moreOptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "MoreOptionText");
				}
			}
		}

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x060016E4 RID: 5860 RVA: 0x00055475 File Offset: 0x00053675
		// (set) Token: 0x060016E5 RID: 5861 RVA: 0x0005547D File Offset: 0x0005367D
		[DataSourceProperty]
		public string GoldText
		{
			get
			{
				return this._goldText;
			}
			set
			{
				if (this._goldText != value)
				{
					this._goldText = value;
					base.OnPropertyChangedWithValue<string>(value, "GoldText");
				}
			}
		}

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x060016E6 RID: 5862 RVA: 0x000554A0 File Offset: 0x000536A0
		// (set) Token: 0x060016E7 RID: 5863 RVA: 0x000554A8 File Offset: 0x000536A8
		[DataSourceProperty]
		public string PersuasionText
		{
			get
			{
				return this._persuasionText;
			}
			set
			{
				if (this._persuasionText != value)
				{
					this._persuasionText = value;
					base.OnPropertyChangedWithValue<string>(value, "PersuasionText");
				}
			}
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x060016E8 RID: 5864 RVA: 0x000554CB File Offset: 0x000536CB
		// (set) Token: 0x060016E9 RID: 5865 RVA: 0x000554D3 File Offset: 0x000536D3
		[DataSourceProperty]
		public bool IsCurrentCharacterValidInEncyclopedia
		{
			get
			{
				return this._isCurrentCharacterValidInEncyclopedia;
			}
			set
			{
				if (this._isCurrentCharacterValidInEncyclopedia != value)
				{
					this._isCurrentCharacterValidInEncyclopedia = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentCharacterValidInEncyclopedia");
				}
			}
		}

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x060016EA RID: 5866 RVA: 0x000554F1 File Offset: 0x000536F1
		// (set) Token: 0x060016EB RID: 5867 RVA: 0x000554F9 File Offset: 0x000536F9
		[DataSourceProperty]
		public bool IsLoadingOver
		{
			get
			{
				return this._isLoadingOver;
			}
			set
			{
				if (this._isLoadingOver != value)
				{
					this._isLoadingOver = value;
					base.OnPropertyChangedWithValue(value, "IsLoadingOver");
				}
			}
		}

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x060016EC RID: 5868 RVA: 0x00055517 File Offset: 0x00053717
		// (set) Token: 0x060016ED RID: 5869 RVA: 0x0005551F File Offset: 0x0005371F
		[DataSourceProperty]
		public bool IsPersuading
		{
			get
			{
				return this._isPersuading;
			}
			set
			{
				if (this._isPersuading != value)
				{
					this._isPersuading = value;
					base.OnPropertyChangedWithValue(value, "IsPersuading");
				}
			}
		}

		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x060016EE RID: 5870 RVA: 0x0005553D File Offset: 0x0005373D
		// (set) Token: 0x060016EF RID: 5871 RVA: 0x00055545 File Offset: 0x00053745
		[DataSourceProperty]
		public string ContinueText
		{
			get
			{
				return this._continueText;
			}
			set
			{
				if (this._continueText != value)
				{
					this._continueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ContinueText");
				}
			}
		}

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x060016F0 RID: 5872 RVA: 0x00055568 File Offset: 0x00053768
		// (set) Token: 0x060016F1 RID: 5873 RVA: 0x00055570 File Offset: 0x00053770
		[DataSourceProperty]
		public string CurrentCharacterNameLbl
		{
			get
			{
				return this._currentCharacterNameLbl;
			}
			set
			{
				if (this._currentCharacterNameLbl != value)
				{
					this._currentCharacterNameLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCharacterNameLbl");
				}
			}
		}

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x060016F2 RID: 5874 RVA: 0x00055593 File Offset: 0x00053793
		// (set) Token: 0x060016F3 RID: 5875 RVA: 0x0005559B File Offset: 0x0005379B
		[DataSourceProperty]
		public MBBindingList<ConversationItemVM> AnswerList
		{
			get
			{
				return this._answerList;
			}
			set
			{
				if (this._answerList != value)
				{
					this._answerList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ConversationItemVM>>(value, "AnswerList");
				}
			}
		}

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x060016F4 RID: 5876 RVA: 0x000555B9 File Offset: 0x000537B9
		// (set) Token: 0x060016F5 RID: 5877 RVA: 0x000555C1 File Offset: 0x000537C1
		[DataSourceProperty]
		public string DialogText
		{
			get
			{
				return this._dialogText;
			}
			set
			{
				if (this._dialogText != value)
				{
					this._dialogText = value;
					base.OnPropertyChangedWithValue<string>(value, "DialogText");
				}
			}
		}

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x060016F6 RID: 5878 RVA: 0x000555E4 File Offset: 0x000537E4
		// (set) Token: 0x060016F7 RID: 5879 RVA: 0x000555EC File Offset: 0x000537EC
		[DataSourceProperty]
		public bool IsAggressive
		{
			get
			{
				return this._isAggressive;
			}
			set
			{
				if (value != this._isAggressive)
				{
					this._isAggressive = value;
					base.OnPropertyChangedWithValue(value, "IsAggressive");
				}
			}
		}

		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x060016F8 RID: 5880 RVA: 0x0005560A File Offset: 0x0005380A
		// (set) Token: 0x060016F9 RID: 5881 RVA: 0x00055612 File Offset: 0x00053812
		[DataSourceProperty]
		public int SelectedSide
		{
			get
			{
				return this._selectedSide;
			}
			set
			{
				if (value != this._selectedSide)
				{
					this._selectedSide = value;
					base.OnPropertyChangedWithValue(value, "SelectedSide");
				}
			}
		}

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x060016FA RID: 5882 RVA: 0x00055630 File Offset: 0x00053830
		// (set) Token: 0x060016FB RID: 5883 RVA: 0x00055638 File Offset: 0x00053838
		[DataSourceProperty]
		public string RelationText
		{
			get
			{
				return this._relationText;
			}
			set
			{
				if (this._relationText != value)
				{
					this._relationText = value;
					base.OnPropertyChangedWithValue<string>(value, "RelationText");
				}
			}
		}

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x060016FC RID: 5884 RVA: 0x0005565B File Offset: 0x0005385B
		// (set) Token: 0x060016FD RID: 5885 RVA: 0x00055663 File Offset: 0x00053863
		[DataSourceProperty]
		public bool IsRelationEnabled
		{
			get
			{
				return this._isRelationEnabled;
			}
			set
			{
				if (value != this._isRelationEnabled)
				{
					this._isRelationEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsRelationEnabled");
				}
			}
		}

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x060016FE RID: 5886 RVA: 0x00055681 File Offset: 0x00053881
		// (set) Token: 0x060016FF RID: 5887 RVA: 0x00055689 File Offset: 0x00053889
		[DataSourceProperty]
		public bool IsBannerEnabled
		{
			get
			{
				return this._isBannerEnabled;
			}
			set
			{
				if (value != this._isBannerEnabled)
				{
					this._isBannerEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsBannerEnabled");
				}
			}
		}

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06001700 RID: 5888 RVA: 0x000556A7 File Offset: 0x000538A7
		// (set) Token: 0x06001701 RID: 5889 RVA: 0x000556AF File Offset: 0x000538AF
		[DataSourceProperty]
		public ConversationItemVM CurrentSelectedAnswer
		{
			get
			{
				return this._currentSelectedAnswer;
			}
			set
			{
				if (this._currentSelectedAnswer != value)
				{
					this._currentSelectedAnswer = value;
					base.OnPropertyChangedWithValue<ConversationItemVM>(value, "CurrentSelectedAnswer");
				}
			}
		}

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06001702 RID: 5890 RVA: 0x000556CD File Offset: 0x000538CD
		// (set) Token: 0x06001703 RID: 5891 RVA: 0x000556D5 File Offset: 0x000538D5
		[DataSourceProperty]
		public ImageIdentifierVM ConversedHeroBanner
		{
			get
			{
				return this._conversedHeroBanner;
			}
			set
			{
				if (this._conversedHeroBanner != value)
				{
					this._conversedHeroBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ConversedHeroBanner");
				}
			}
		}

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06001704 RID: 5892 RVA: 0x000556F3 File Offset: 0x000538F3
		// (set) Token: 0x06001705 RID: 5893 RVA: 0x000556FB File Offset: 0x000538FB
		[DataSourceProperty]
		public HintViewModel RelationHint
		{
			get
			{
				return this._relationHint;
			}
			set
			{
				if (this._relationHint != value)
				{
					this._relationHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RelationHint");
				}
			}
		}

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06001706 RID: 5894 RVA: 0x00055719 File Offset: 0x00053919
		// (set) Token: 0x06001707 RID: 5895 RVA: 0x00055721 File Offset: 0x00053921
		[DataSourceProperty]
		public HintViewModel FactionHint
		{
			get
			{
				return this._factionHint;
			}
			set
			{
				if (this._factionHint != value)
				{
					this._factionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FactionHint");
				}
			}
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x0005573F File Offset: 0x0005393F
		// (set) Token: 0x06001709 RID: 5897 RVA: 0x00055747 File Offset: 0x00053947
		[DataSourceProperty]
		public HintViewModel GoldHint
		{
			get
			{
				return this._goldHint;
			}
			set
			{
				if (this._goldHint != value)
				{
					this._goldHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "GoldHint");
				}
			}
		}

		// Token: 0x04000AA8 RID: 2728
		private readonly ConversationManager _conversationManager;

		// Token: 0x04000AA9 RID: 2729
		private readonly bool _isLinksDisabled;

		// Token: 0x04000AAA RID: 2730
		private static bool _isCurrentlyPlayerSpeaking;

		// Token: 0x04000AAB RID: 2731
		private bool _isProcessingOption;

		// Token: 0x04000AAC RID: 2732
		private BasicCharacterObject _currentDialogCharacter;

		// Token: 0x04000AAD RID: 2733
		private Func<string> _getContinueInputText;

		// Token: 0x04000AAE RID: 2734
		private MBBindingList<ConversationItemVM> _answerList;

		// Token: 0x04000AAF RID: 2735
		private string _dialogText;

		// Token: 0x04000AB0 RID: 2736
		private string _currentCharacterNameLbl;

		// Token: 0x04000AB1 RID: 2737
		private string _continueText;

		// Token: 0x04000AB2 RID: 2738
		private string _relationText;

		// Token: 0x04000AB3 RID: 2739
		private string _persuasionText;

		// Token: 0x04000AB4 RID: 2740
		private bool _isLoadingOver;

		// Token: 0x04000AB5 RID: 2741
		private string _moreOptionText;

		// Token: 0x04000AB6 RID: 2742
		private string _goldText;

		// Token: 0x04000AB7 RID: 2743
		private ConversationAggressivePartyItemVM _defenderLeader;

		// Token: 0x04000AB8 RID: 2744
		private ConversationAggressivePartyItemVM _attackerLeader;

		// Token: 0x04000AB9 RID: 2745
		private MBBindingList<ConversationAggressivePartyItemVM> _defenderParties;

		// Token: 0x04000ABA RID: 2746
		private MBBindingList<ConversationAggressivePartyItemVM> _attackerParties;

		// Token: 0x04000ABB RID: 2747
		private ImageIdentifierVM _conversedHeroBanner;

		// Token: 0x04000ABC RID: 2748
		private bool _isAggressive;

		// Token: 0x04000ABD RID: 2749
		private bool _isRelationEnabled;

		// Token: 0x04000ABE RID: 2750
		private bool _isBannerEnabled;

		// Token: 0x04000ABF RID: 2751
		private bool _isPersuading;

		// Token: 0x04000AC0 RID: 2752
		private bool _isCurrentCharacterValidInEncyclopedia;

		// Token: 0x04000AC1 RID: 2753
		private int _selectedSide;

		// Token: 0x04000AC2 RID: 2754
		private int _relation;

		// Token: 0x04000AC3 RID: 2755
		private int _minRelation;

		// Token: 0x04000AC4 RID: 2756
		private int _maxRelation;

		// Token: 0x04000AC5 RID: 2757
		private PowerLevelComparer _powerComparer;

		// Token: 0x04000AC6 RID: 2758
		private ConversationItemVM _currentSelectedAnswer;

		// Token: 0x04000AC7 RID: 2759
		private PersuasionVM _persuasion;

		// Token: 0x04000AC8 RID: 2760
		private HintViewModel _relationHint;

		// Token: 0x04000AC9 RID: 2761
		private HintViewModel _factionHint;

		// Token: 0x04000ACA RID: 2762
		private HintViewModel _goldHint;
	}
}
