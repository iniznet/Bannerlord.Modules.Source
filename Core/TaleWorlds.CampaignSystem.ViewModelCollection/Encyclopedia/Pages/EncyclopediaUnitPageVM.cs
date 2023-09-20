using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000BA RID: 186
	[EncyclopediaViewModel(typeof(CharacterObject))]
	public class EncyclopediaUnitPageVM : EncyclopediaContentPageVM
	{
		// Token: 0x0600127A RID: 4730 RVA: 0x00047D88 File Offset: 0x00045F88
		public EncyclopediaUnitPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._character = base.Obj as CharacterObject;
			this.UnitCharacter = new CharacterViewModel(CharacterViewModel.StanceTypes.OnMount);
			this.UnitCharacter.FillFrom(this._character, -1);
			this.HasErrors = this.DoesCharacterHaveCircularUpgradePaths(this._character, null);
			if (!this.HasErrors)
			{
				CharacterObject characterObject = CharacterHelper.FindUpgradeRootOf(this._character);
				this.Tree = new EncyclopediaTroopTreeNodeVM(characterObject, this._character, false, null);
			}
			this.PropertiesList = new MBBindingList<StringItemWithHintVM>();
			this.EquipmentSetSelector = new SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM>(0, new Action<SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM>>(this.OnEquipmentSetChange));
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._character);
			this.RefreshValues();
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x00047E50 File Offset: 0x00046050
		private bool DoesCharacterHaveCircularUpgradePaths(CharacterObject baseCharacter, CharacterObject character = null)
		{
			bool flag = false;
			if (character == null)
			{
				character = baseCharacter;
			}
			for (int i = 0; i < character.UpgradeTargets.Length; i++)
			{
				if (character.UpgradeTargets[i] == baseCharacter)
				{
					Debug.FailedAssert(string.Format("Circular dependency on troop upgrade paths: {0} --> {1}", character.Name, baseCharacter.Name), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Encyclopedia\\Pages\\EncyclopediaUnitPageVM.cs", "DoesCharacterHaveCircularUpgradePaths", 56);
					flag = true;
					break;
				}
				flag = this.DoesCharacterHaveCircularUpgradePaths(baseCharacter, character.UpgradeTargets[i]);
			}
			return flag;
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x00047EC0 File Offset: 0x000460C0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._equipmentSetTextObj = new TextObject("{=vggt7exj}Set {CURINDEX}/{COUNT}", null);
			this.PropertiesList.Clear();
			this.PropertiesList.Add(CampaignUIHelper.GetCharacterTierData(this._character, true));
			this.PropertiesList.Add(CampaignUIHelper.GetCharacterTypeData(this._character, true));
			this.EquipmentSetSelector.ItemList.Clear();
			using (IEnumerator<Equipment> enumerator = this._character.BattleEquipments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Equipment equipment = enumerator.Current;
					if (!this.EquipmentSetSelector.ItemList.Any((EncyclopediaUnitEquipmentSetSelectorItemVM x) => x.EquipmentSet.IsEquipmentEqualTo(equipment)))
					{
						this.EquipmentSetSelector.AddItem(new EncyclopediaUnitEquipmentSetSelectorItemVM(equipment, ""));
					}
				}
			}
			if (this.EquipmentSetSelector.ItemList.Count > 0)
			{
				this.EquipmentSetSelector.SelectedIndex = 0;
			}
			this._equipmentSetTextObj.SetTextVariable("CURINDEX", this.EquipmentSetSelector.SelectedIndex + 1);
			this._equipmentSetTextObj.SetTextVariable("COUNT", this.EquipmentSetSelector.ItemList.Count);
			this.EquipmentSetText = this._equipmentSetTextObj.ToString();
			this.TreeDisplayErrorText = new TextObject("{=BkDycbdq}Error while displaying the troop tree", null).ToString();
			this.Skills = new MBBindingList<EncyclopediaSkillVM>();
			foreach (SkillObject skillObject in TaleWorlds.CampaignSystem.Extensions.Skills.All)
			{
				if (this._character.GetSkillValue(skillObject) > 0)
				{
					this.Skills.Add(new EncyclopediaSkillVM(skillObject, this._character.GetSkillValue(skillObject)));
				}
			}
			this.DescriptionText = GameTexts.FindText("str_encyclopedia_unit_description", this._character.StringId).ToString();
			this.NameText = this._character.Name.ToString();
			EncyclopediaTroopTreeNodeVM tree = this.Tree;
			if (tree != null)
			{
				tree.RefreshValues();
			}
			CharacterViewModel unitCharacter = this.UnitCharacter;
			if (unitCharacter != null)
			{
				unitCharacter.RefreshValues();
			}
			base.UpdateBookmarkHintText();
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x00048104 File Offset: 0x00046304
		private void OnEquipmentSetChange(SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> selector)
		{
			this.CurrentSelectedEquipmentSet = selector.SelectedItem;
			this.UnitCharacter.SetEquipment(this.CurrentSelectedEquipmentSet.EquipmentSet);
			this._equipmentSetTextObj.SetTextVariable("CURINDEX", selector.SelectedIndex + 1);
			this._equipmentSetTextObj.SetTextVariable("COUNT", selector.ItemList.Count);
			this.EquipmentSetText = this._equipmentSetTextObj.ToString();
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x00048179 File Offset: 0x00046379
		public override string GetName()
		{
			return this._character.Name.ToString();
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x0004818C File Offset: 0x0004638C
		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Units", GameTexts.FindText("str_encyclopedia_troops", null).ToString()) + " \\ " + this.GetName();
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x000481F4 File Offset: 0x000463F4
		public override void ExecuteSwitchBookmarkedState()
		{
			base.ExecuteSwitchBookmarkedState();
			if (base.IsBookmarked)
			{
				Campaign.Current.EncyclopediaManager.ViewDataTracker.AddEncyclopediaBookmarkToItem(this._character);
				return;
			}
			Campaign.Current.EncyclopediaManager.ViewDataTracker.RemoveEncyclopediaBookmarkFromItem(this._character);
		}

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06001281 RID: 4737 RVA: 0x00048244 File Offset: 0x00046444
		// (set) Token: 0x06001282 RID: 4738 RVA: 0x0004824C File Offset: 0x0004644C
		[DataSourceProperty]
		public MBBindingList<EncyclopediaSkillVM> Skills
		{
			get
			{
				return this._skills;
			}
			set
			{
				if (value != this._skills)
				{
					this._skills = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSkillVM>>(value, "Skills");
				}
			}
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06001283 RID: 4739 RVA: 0x0004826A File Offset: 0x0004646A
		// (set) Token: 0x06001284 RID: 4740 RVA: 0x00048272 File Offset: 0x00046472
		[DataSourceProperty]
		public MBBindingList<StringItemWithHintVM> PropertiesList
		{
			get
			{
				return this._propertiesList;
			}
			set
			{
				if (value != this._propertiesList)
				{
					this._propertiesList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithHintVM>>(value, "PropertiesList");
				}
			}
		}

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06001285 RID: 4741 RVA: 0x00048290 File Offset: 0x00046490
		// (set) Token: 0x06001286 RID: 4742 RVA: 0x00048298 File Offset: 0x00046498
		[DataSourceProperty]
		public SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> EquipmentSetSelector
		{
			get
			{
				return this._equipmentSetSelector;
			}
			set
			{
				if (value != this._equipmentSetSelector)
				{
					this._equipmentSetSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM>>(value, "EquipmentSetSelector");
				}
			}
		}

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06001287 RID: 4743 RVA: 0x000482B6 File Offset: 0x000464B6
		// (set) Token: 0x06001288 RID: 4744 RVA: 0x000482BE File Offset: 0x000464BE
		[DataSourceProperty]
		public EncyclopediaUnitEquipmentSetSelectorItemVM CurrentSelectedEquipmentSet
		{
			get
			{
				return this._currentSelectedEquipmentSet;
			}
			set
			{
				if (value != this._currentSelectedEquipmentSet)
				{
					this._currentSelectedEquipmentSet = value;
					base.OnPropertyChangedWithValue<EncyclopediaUnitEquipmentSetSelectorItemVM>(value, "CurrentSelectedEquipmentSet");
				}
			}
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06001289 RID: 4745 RVA: 0x000482DC File Offset: 0x000464DC
		// (set) Token: 0x0600128A RID: 4746 RVA: 0x000482E4 File Offset: 0x000464E4
		[DataSourceProperty]
		public CharacterViewModel UnitCharacter
		{
			get
			{
				return this._unitCharacter;
			}
			set
			{
				if (value != this._unitCharacter)
				{
					this._unitCharacter = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "UnitCharacter");
				}
			}
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x0600128B RID: 4747 RVA: 0x00048302 File Offset: 0x00046502
		// (set) Token: 0x0600128C RID: 4748 RVA: 0x0004830A File Offset: 0x0004650A
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x0600128D RID: 4749 RVA: 0x0004832D File Offset: 0x0004652D
		// (set) Token: 0x0600128E RID: 4750 RVA: 0x00048335 File Offset: 0x00046535
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x0600128F RID: 4751 RVA: 0x00048358 File Offset: 0x00046558
		// (set) Token: 0x06001290 RID: 4752 RVA: 0x00048360 File Offset: 0x00046560
		[DataSourceProperty]
		public EncyclopediaTroopTreeNodeVM Tree
		{
			get
			{
				return this._tree;
			}
			set
			{
				if (value != this._tree)
				{
					this._tree = value;
					base.OnPropertyChangedWithValue<EncyclopediaTroopTreeNodeVM>(value, "Tree");
				}
			}
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06001291 RID: 4753 RVA: 0x0004837E File Offset: 0x0004657E
		// (set) Token: 0x06001292 RID: 4754 RVA: 0x00048386 File Offset: 0x00046586
		[DataSourceProperty]
		public string TreeDisplayErrorText
		{
			get
			{
				return this._treeDisplayErrorText;
			}
			set
			{
				if (value != this._treeDisplayErrorText)
				{
					this._treeDisplayErrorText = value;
					base.OnPropertyChangedWithValue<string>(value, "TreeDisplayErrorText");
				}
			}
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x000483A9 File Offset: 0x000465A9
		// (set) Token: 0x06001294 RID: 4756 RVA: 0x000483B1 File Offset: 0x000465B1
		[DataSourceProperty]
		public string EquipmentSetText
		{
			get
			{
				return this._equipmentSetText;
			}
			set
			{
				if (value != this._equipmentSetText)
				{
					this._equipmentSetText = value;
					base.OnPropertyChangedWithValue<string>(value, "EquipmentSetText");
				}
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06001295 RID: 4757 RVA: 0x000483D4 File Offset: 0x000465D4
		// (set) Token: 0x06001296 RID: 4758 RVA: 0x000483DC File Offset: 0x000465DC
		[DataSourceProperty]
		public bool HasErrors
		{
			get
			{
				return this._hasErrors;
			}
			set
			{
				if (value != this._hasErrors)
				{
					this._hasErrors = value;
					base.OnPropertyChangedWithValue(value, "HasErrors");
				}
			}
		}

		// Token: 0x04000895 RID: 2197
		private readonly CharacterObject _character;

		// Token: 0x04000896 RID: 2198
		private TextObject _equipmentSetTextObj;

		// Token: 0x04000897 RID: 2199
		private MBBindingList<EncyclopediaSkillVM> _skills;

		// Token: 0x04000898 RID: 2200
		private MBBindingList<StringItemWithHintVM> _propertiesList;

		// Token: 0x04000899 RID: 2201
		private SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> _equipmentSetSelector;

		// Token: 0x0400089A RID: 2202
		private EncyclopediaUnitEquipmentSetSelectorItemVM _currentSelectedEquipmentSet;

		// Token: 0x0400089B RID: 2203
		private EncyclopediaTroopTreeNodeVM _tree;

		// Token: 0x0400089C RID: 2204
		private string _descriptionText;

		// Token: 0x0400089D RID: 2205
		private CharacterViewModel _unitCharacter;

		// Token: 0x0400089E RID: 2206
		private string _nameText;

		// Token: 0x0400089F RID: 2207
		private string _treeDisplayErrorText;

		// Token: 0x040008A0 RID: 2208
		private string _equipmentSetText;

		// Token: 0x040008A1 RID: 2209
		private bool _hasErrors;
	}
}
