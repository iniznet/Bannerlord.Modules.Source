using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001D9 RID: 473
	public class CharacterCreation
	{
		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06001BCD RID: 7117 RVA: 0x0007DAB7 File Offset: 0x0007BCB7
		// (set) Token: 0x06001BCE RID: 7118 RVA: 0x0007DABF File Offset: 0x0007BCBF
		public bool IsPlayerAlone { get; set; }

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06001BCF RID: 7119 RVA: 0x0007DAC8 File Offset: 0x0007BCC8
		// (set) Token: 0x06001BD0 RID: 7120 RVA: 0x0007DAD0 File Offset: 0x0007BCD0
		public bool HasSecondaryCharacter { get; set; }

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06001BD1 RID: 7121 RVA: 0x0007DAD9 File Offset: 0x0007BCD9
		// (set) Token: 0x06001BD2 RID: 7122 RVA: 0x0007DAE1 File Offset: 0x0007BCE1
		public string PrefabId { get; private set; }

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06001BD3 RID: 7123 RVA: 0x0007DAEA File Offset: 0x0007BCEA
		// (set) Token: 0x06001BD4 RID: 7124 RVA: 0x0007DAF2 File Offset: 0x0007BCF2
		public sbyte PrefabBoneUsage { get; private set; }

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06001BD5 RID: 7125 RVA: 0x0007DAFB File Offset: 0x0007BCFB
		public MBReadOnlyList<FaceGenChar> FaceGenChars
		{
			get
			{
				return this._faceGenChars;
			}
		}

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06001BD6 RID: 7126 RVA: 0x0007DB03 File Offset: 0x0007BD03
		// (set) Token: 0x06001BD7 RID: 7127 RVA: 0x0007DB0B File Offset: 0x0007BD0B
		public FaceGenMount FaceGenMount { get; private set; }

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06001BD8 RID: 7128 RVA: 0x0007DB14 File Offset: 0x0007BD14
		// (set) Token: 0x06001BD9 RID: 7129 RVA: 0x0007DB1C File Offset: 0x0007BD1C
		public bool CharsEquipmentNeedsRefresh { get; private set; }

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06001BDA RID: 7130 RVA: 0x0007DB25 File Offset: 0x0007BD25
		// (set) Token: 0x06001BDB RID: 7131 RVA: 0x0007DB2D File Offset: 0x0007BD2D
		public bool CharsNeedsRefresh { get; set; }

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06001BDC RID: 7132 RVA: 0x0007DB36 File Offset: 0x0007BD36
		// (set) Token: 0x06001BDD RID: 7133 RVA: 0x0007DB3E File Offset: 0x0007BD3E
		public bool MountsNeedsRefresh { get; set; }

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06001BDE RID: 7134 RVA: 0x0007DB47 File Offset: 0x0007BD47
		// (set) Token: 0x06001BDF RID: 7135 RVA: 0x0007DB4F File Offset: 0x0007BD4F
		public string Name { get; set; }

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06001BE0 RID: 7136 RVA: 0x0007DB58 File Offset: 0x0007BD58
		public int CharacterCreationMenuCount
		{
			get
			{
				return this.CharacterCreationMenus.Count;
			}
		}

		// Token: 0x06001BE1 RID: 7137 RVA: 0x0007DB68 File Offset: 0x0007BD68
		public void ChangeFaceGenChars(List<FaceGenChar> newChars)
		{
			this._faceGenChars.Clear();
			foreach (FaceGenChar faceGenChar in newChars)
			{
				this._faceGenChars.Add(faceGenChar);
			}
			this.CharsNeedsRefresh = true;
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x0007DBD0 File Offset: 0x0007BDD0
		public void SetFaceGenMount(FaceGenMount newMount)
		{
			this.FaceGenMount = null;
			if (newMount != null)
			{
				this.FaceGenMount = newMount;
			}
			this.MountsNeedsRefresh = true;
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x0007DBEA File Offset: 0x0007BDEA
		public void ClearFaceGenMounts()
		{
			this.FaceGenMount = null;
			this.MountsNeedsRefresh = true;
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x0007DBFA File Offset: 0x0007BDFA
		public void ClearFaceGenChars()
		{
			this._faceGenChars.Clear();
			this.CharsNeedsRefresh = true;
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x0007DC0E File Offset: 0x0007BE0E
		public void ClearFaceGenPrefab()
		{
			this.PrefabId = "";
			this.PrefabBoneUsage = 0;
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x0007DC24 File Offset: 0x0007BE24
		public void ChangeCharactersEquipment(List<Equipment> equipmentList)
		{
			for (int i = 0; i < equipmentList.Count; i++)
			{
				this._faceGenChars[i].Equipment.FillFrom(equipmentList[i], true);
			}
			this.CharsEquipmentNeedsRefresh = true;
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x0007DC68 File Offset: 0x0007BE68
		public void ClearCharactersEquipment()
		{
			for (int i = 0; i < this._faceGenChars.Count; i++)
			{
				this._faceGenChars[i].Equipment.FillFrom(new Equipment(), true);
			}
			this.CharsEquipmentNeedsRefresh = true;
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x0007DCAE File Offset: 0x0007BEAE
		public void ChangeCharacterPrefab(string id, sbyte boneUsage)
		{
			this.PrefabId = id;
			this.PrefabBoneUsage = boneUsage;
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x0007DCC0 File Offset: 0x0007BEC0
		public void ChangeCharsAnimation(List<string> actionList)
		{
			for (int i = 0; i < actionList.Count; i++)
			{
				this._faceGenChars[i].ActionName = actionList[i];
			}
			this.CharsEquipmentNeedsRefresh = true;
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x0007DCFD File Offset: 0x0007BEFD
		public void ChangeMountsAnimation(string action)
		{
			this.FaceGenMount.ActionName = action;
			this.MountsNeedsRefresh = true;
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x0007DD12 File Offset: 0x0007BF12
		public CharacterCreation()
		{
			this._faceGenChars = new MBList<FaceGenChar>();
			this.CharacterCreationMenus = new List<CharacterCreationMenu>();
			this.CharsEquipmentNeedsRefresh = false;
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x0007DD37 File Offset: 0x0007BF37
		public void AddNewMenu(CharacterCreationMenu menu)
		{
			this.CharacterCreationMenus.Add(menu);
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x0007DD45 File Offset: 0x0007BF45
		public CharacterCreationMenu GetCurrentMenu(int index)
		{
			if (index >= 0 && index < this.CharacterCreationMenus.Count)
			{
				return this.CharacterCreationMenus[index];
			}
			return null;
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x0007DD68 File Offset: 0x0007BF68
		public IEnumerable<CharacterCreationOption> GetCurrentMenuOptions(int index)
		{
			List<CharacterCreationOption> list = new List<CharacterCreationOption>();
			CharacterCreationMenu currentMenu = this.GetCurrentMenu(index);
			if (currentMenu != null)
			{
				foreach (CharacterCreationCategory characterCreationCategory in currentMenu.CharacterCreationCategories)
				{
					CharacterCreationOnCondition categoryCondition = characterCreationCategory.CategoryCondition;
					if (categoryCondition == null || categoryCondition())
					{
						foreach (CharacterCreationOption characterCreationOption in characterCreationCategory.CharacterCreationOptions)
						{
							if (characterCreationOption.OnCondition == null || characterCreationOption.OnCondition())
							{
								list.Add(characterCreationOption);
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x0007DE3C File Offset: 0x0007C03C
		public void ResetMenuOptions()
		{
			for (int i = 0; i < this.CharacterCreationMenus.Count; i++)
			{
				this.CharacterCreationMenus[i].SelectedOptions.Clear();
			}
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x0007DE75 File Offset: 0x0007C075
		public void OnInit(int stage)
		{
			if (this.CharacterCreationMenus[stage].OnInit != null)
			{
				this.CharacterCreationMenus[stage].OnInit(this);
			}
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x0007DEA1 File Offset: 0x0007C0A1
		public TextObject GetCurrentMenuText(int stage)
		{
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			return this.CharacterCreationMenus[stage].Text;
		}

		// Token: 0x06001BF2 RID: 7154 RVA: 0x0007DEC6 File Offset: 0x0007C0C6
		public TextObject GetCurrentMenuTitle(int stage)
		{
			return this.CharacterCreationMenus[stage].Title;
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x0007DEDC File Offset: 0x0007C0DC
		public void RunConsequence(CharacterCreationOption option, int stage, bool fromInit)
		{
			if (this.CharacterCreationMenus[stage].MenuType == CharacterCreationMenu.MenuTypes.MultipleChoice)
			{
				this.CharacterCreationMenus[stage].SelectedOptions.Clear();
			}
			if (!fromInit && this.CharacterCreationMenus[stage].SelectedOptions.Contains(option.Id))
			{
				this.CharacterCreationMenus[stage].SelectedOptions.Remove(option.Id);
				return;
			}
			this.CharacterCreationMenus[stage].SelectedOptions.Add(option.Id);
			if (option.OnSelect != null)
			{
				option.OnSelect(this);
			}
		}

		// Token: 0x06001BF4 RID: 7156 RVA: 0x0007DF81 File Offset: 0x0007C181
		public IEnumerable<int> GetSelectedOptions(int stage)
		{
			return this.CharacterCreationMenus[stage].SelectedOptions;
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x0007DF94 File Offset: 0x0007C194
		public void ApplyFinalEffects()
		{
			Clan.PlayerClan.Renown = 0f;
			CharacterCreationContentBase.Instance.ApplyCulture(this);
			foreach (CharacterCreationMenu characterCreationMenu in this.CharacterCreationMenus)
			{
				characterCreationMenu.ApplyFinalEffect(this);
			}
			Campaign.Current.PlayerTraitDeveloper.UpdateTraitXPAccordingToTraitLevels();
		}

		// Token: 0x040008D5 RID: 2261
		private readonly MBList<FaceGenChar> _faceGenChars;

		// Token: 0x040008D9 RID: 2265
		public readonly List<CharacterCreationMenu> CharacterCreationMenus;
	}
}
