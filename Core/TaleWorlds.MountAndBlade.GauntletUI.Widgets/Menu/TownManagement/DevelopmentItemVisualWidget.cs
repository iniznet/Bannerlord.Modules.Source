using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000EB RID: 235
	public class DevelopmentItemVisualWidget : Widget
	{
		// Token: 0x06000C40 RID: 3136 RVA: 0x00022222 File Offset: 0x00020422
		public DevelopmentItemVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x0002222C File Offset: 0x0002042C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._changedVisualToSmallVariant)
			{
				string text = this.DetermineSpriteImageFromSpriteCode(this.SpriteCode, this.UseSmallVariant);
				base.Sprite = base.Context.SpriteData.GetSprite((!string.IsNullOrEmpty(text)) ? text : "building_default");
				if (!this.IsDaily && this.DevelopmentFrontVisualWidget != null)
				{
					this.DevelopmentFrontVisualWidget.Sprite = base.Sprite;
				}
				this._changedVisualToSmallVariant = true;
			}
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x000222AC File Offset: 0x000204AC
		private string DetermineSpriteImageFromSpriteCode(string spriteCode, bool useSmallVariant)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(spriteCode);
			string text;
			if (num <= 1519127731U)
			{
				if (num > 867141850U)
				{
					if (num <= 997573159U)
					{
						if (num != 876418704U)
						{
							if (num != 909051374U)
							{
								if (num != 997573159U)
								{
									goto IL_48A;
								}
								if (!(spriteCode == "building_castle_training_fields"))
								{
									goto IL_48A;
								}
								goto IL_40A;
							}
							else
							{
								if (!(spriteCode == "building_wall"))
								{
									goto IL_48A;
								}
								goto IL_3F4;
							}
						}
						else if (!(spriteCode == "building_settlement_fairgrounds"))
						{
							goto IL_48A;
						}
					}
					else if (num <= 1198366207U)
					{
						if (num != 1162521335U)
						{
							if (num != 1198366207U)
							{
								goto IL_48A;
							}
							if (!(spriteCode == "building_castle_workshops"))
							{
								goto IL_48A;
							}
							goto IL_43A;
						}
						else if (!(spriteCode == "building_castle_fairgrounds"))
						{
							goto IL_48A;
						}
					}
					else if (num != 1286788355U)
					{
						if (num != 1519127731U)
						{
							goto IL_48A;
						}
						if (!(spriteCode == "building_siege_workshop"))
						{
							goto IL_48A;
						}
						goto IL_44A;
					}
					else
					{
						if (!(spriteCode == "building_settlement_aquaducts"))
						{
							goto IL_48A;
						}
						text = "building_aquaduct";
						goto IL_490;
					}
					text = "building_settlement_fairgrounds";
					goto IL_490;
				}
				if (num <= 379847634U)
				{
					if (num != 14721173U)
					{
						if (num != 105185459U)
						{
							if (num != 379847634U)
							{
								goto IL_48A;
							}
							if (!(spriteCode == "building_irrigation"))
							{
								goto IL_48A;
							}
							text = "building_daily_irrigation";
							goto IL_490;
						}
						else
						{
							if (!(spriteCode == "building_settlement_lime_kilns"))
							{
								goto IL_48A;
							}
							goto IL_45A;
						}
					}
					else
					{
						if (!(spriteCode == "building_castle_granary"))
						{
							goto IL_48A;
						}
						goto IL_412;
					}
				}
				else if (num != 478130347U)
				{
					if (num != 550169199U)
					{
						if (num != 867141850U)
						{
							goto IL_48A;
						}
						if (!(spriteCode == "building_fortifications"))
						{
							goto IL_48A;
						}
					}
					else
					{
						if (!(spriteCode == "building_settlement_garrison_barracks"))
						{
							goto IL_48A;
						}
						goto IL_3FF;
					}
				}
				else
				{
					if (!(spriteCode == "building_settlement_workshop"))
					{
						goto IL_48A;
					}
					goto IL_43A;
				}
				IL_3F4:
				text = "building_fortifications";
				goto IL_490;
				IL_43A:
				text = "building_workshop";
				goto IL_490;
			}
			if (num > 2962793043U)
			{
				if (num <= 3330616303U)
				{
					if (num != 2990460566U)
					{
						if (num != 3125219538U)
						{
							if (num != 3330616303U)
							{
								goto IL_48A;
							}
							if (!(spriteCode == "building_daily_build_house"))
							{
								goto IL_48A;
							}
							text = "building_daily_build_house";
							goto IL_490;
						}
						else if (!(spriteCode == "building_castle_militia_barracks"))
						{
							goto IL_48A;
						}
					}
					else
					{
						if (!(spriteCode == "building_castle_lime_kilns"))
						{
							goto IL_48A;
						}
						text = "building_lime_kilns";
						goto IL_490;
					}
				}
				else if (num <= 4039166522U)
				{
					if (num != 3869985029U)
					{
						if (num != 4039166522U)
						{
							goto IL_48A;
						}
						if (!(spriteCode == "building_settlement_granary"))
						{
							goto IL_48A;
						}
						goto IL_412;
					}
					else
					{
						if (!(spriteCode == "building_festivals_and_games"))
						{
							goto IL_48A;
						}
						text = "building_daily_festivals_and_games";
						goto IL_490;
					}
				}
				else if (num != 4086265432U)
				{
					if (num != 4093017515U)
					{
						goto IL_48A;
					}
					if (!(spriteCode == "building_settlement_militia_barracks"))
					{
						goto IL_48A;
					}
				}
				else
				{
					if (!(spriteCode == "building_castle_barracks"))
					{
						goto IL_48A;
					}
					goto IL_3FF;
				}
				text = "building_militia_barracks";
				goto IL_490;
			}
			if (num <= 2376782232U)
			{
				if (num != 1738509422U)
				{
					if (num != 2168957445U)
					{
						if (num != 2376782232U)
						{
							goto IL_48A;
						}
						if (!(spriteCode == "building_daily_train_militia"))
						{
							goto IL_48A;
						}
						text = "building_daily_train_militia";
						goto IL_490;
					}
					else
					{
						if (!(spriteCode == "building_settlement_marketplace"))
						{
							goto IL_48A;
						}
						text = "building_marketplace";
						goto IL_490;
					}
				}
				else
				{
					if (!(spriteCode == "building_castle_siege_workshop"))
					{
						goto IL_48A;
					}
					goto IL_44A;
				}
			}
			else if (num <= 2764915242U)
			{
				if (num != 2731049836U)
				{
					if (num != 2764915242U)
					{
						goto IL_48A;
					}
					if (!(spriteCode == "building_castle_castallans_office"))
					{
						goto IL_48A;
					}
					text = "building_wardens_office";
					goto IL_490;
				}
				else
				{
					if (!(spriteCode == "building_settlement_training_fields"))
					{
						goto IL_48A;
					}
					goto IL_40A;
				}
			}
			else if (num != 2795610243U)
			{
				if (num != 2962793043U)
				{
					goto IL_48A;
				}
				if (!(spriteCode == "building_castle_gardens"))
				{
					goto IL_48A;
				}
				goto IL_45A;
			}
			else
			{
				if (!(spriteCode == "building_settlement_forum"))
				{
					goto IL_48A;
				}
				text = "building_forum";
				goto IL_490;
			}
			IL_3FF:
			text = "building_garrison_barracks";
			goto IL_490;
			IL_40A:
			text = "building_training_fields";
			goto IL_490;
			IL_412:
			text = "building_granary";
			goto IL_490;
			IL_44A:
			text = "building_siege_workshop";
			goto IL_490;
			IL_45A:
			text = "building_gardens";
			goto IL_490;
			IL_48A:
			return "";
			IL_490:
			if (useSmallVariant)
			{
				text += "_t";
			}
			return text;
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06000C43 RID: 3139 RVA: 0x00022759 File Offset: 0x00020959
		// (set) Token: 0x06000C44 RID: 3140 RVA: 0x00022761 File Offset: 0x00020961
		[Editor(false)]
		public bool UseSmallVariant
		{
			get
			{
				return this._useSmallVariant;
			}
			set
			{
				if (this._useSmallVariant != value)
				{
					this._useSmallVariant = value;
					base.OnPropertyChanged(value, "UseSmallVariant");
				}
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06000C45 RID: 3141 RVA: 0x0002277F File Offset: 0x0002097F
		// (set) Token: 0x06000C46 RID: 3142 RVA: 0x00022787 File Offset: 0x00020987
		[Editor(false)]
		public bool IsDaily
		{
			get
			{
				return this._isDaily;
			}
			set
			{
				if (this._isDaily != value)
				{
					this._isDaily = value;
					base.OnPropertyChanged(value, "IsDaily");
				}
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06000C47 RID: 3143 RVA: 0x000227A5 File Offset: 0x000209A5
		// (set) Token: 0x06000C48 RID: 3144 RVA: 0x000227AD File Offset: 0x000209AD
		[Editor(false)]
		public string SpriteCode
		{
			get
			{
				return this._spriteCode;
			}
			set
			{
				if (this._spriteCode != value)
				{
					this._spriteCode = value;
					base.OnPropertyChanged<string>(value, "SpriteCode");
					this._changedVisualToSmallVariant = false;
				}
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06000C49 RID: 3145 RVA: 0x000227D7 File Offset: 0x000209D7
		// (set) Token: 0x06000C4A RID: 3146 RVA: 0x000227DF File Offset: 0x000209DF
		[Editor(false)]
		public Widget DevelopmentFrontVisualWidget
		{
			get
			{
				return this._developmentFrontVisualWidget;
			}
			set
			{
				if (this._developmentFrontVisualWidget != value)
				{
					this._developmentFrontVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "DevelopmentFrontVisualWidget");
				}
			}
		}

		// Token: 0x040005A5 RID: 1445
		private const string _defaultBuildingSpriteName = "building_default";

		// Token: 0x040005A6 RID: 1446
		private bool _changedVisualToSmallVariant;

		// Token: 0x040005A7 RID: 1447
		private bool _useSmallVariant;

		// Token: 0x040005A8 RID: 1448
		private bool _isDaily;

		// Token: 0x040005A9 RID: 1449
		private string _spriteCode;

		// Token: 0x040005AA RID: 1450
		private Widget _developmentFrontVisualWidget;
	}
}
