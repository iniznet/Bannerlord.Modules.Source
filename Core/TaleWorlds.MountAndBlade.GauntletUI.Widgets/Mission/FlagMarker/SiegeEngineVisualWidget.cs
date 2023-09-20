using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.FlagMarker
{
	// Token: 0x020000E3 RID: 227
	public class SiegeEngineVisualWidget : Widget
	{
		// Token: 0x06000BE1 RID: 3041 RVA: 0x000212DE File Offset: 0x0001F4DE
		public SiegeEngineVisualWidget(UIContext context)
			: base(context)
		{
			this._fallbackSprite = this.GetSprite("BlankWhiteCircle");
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00021304 File Offset: 0x0001F504
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._hasVisualSet && this.EngineID != string.Empty && this.OutlineWidget != null && this.IconWidget != null)
			{
				string text = string.Empty;
				string engineID = this.EngineID;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(engineID);
				if (num <= 1241455715U)
				{
					if (num <= 712590611U)
					{
						if (num != 6339497U)
						{
							if (num != 695812992U)
							{
								if (num != 712590611U)
								{
									goto IL_1F8;
								}
								if (!(engineID == "siege_tower_level2"))
								{
									goto IL_1F8;
								}
							}
							else if (!(engineID == "siege_tower_level3"))
							{
								goto IL_1F8;
							}
						}
						else
						{
							if (!(engineID == "ladder"))
							{
								goto IL_1F8;
							}
							text = "ladder";
							goto IL_1F8;
						}
					}
					else if (num != 729368230U)
					{
						if (num != 808481256U)
						{
							if (num != 1241455715U)
							{
								goto IL_1F8;
							}
							if (!(engineID == "ram"))
							{
								goto IL_1F8;
							}
							text = "battering_ram";
							goto IL_1F8;
						}
						else
						{
							if (!(engineID == "fire_ballista"))
							{
								goto IL_1F8;
							}
							goto IL_1D2;
						}
					}
					else if (!(engineID == "siege_tower_level1"))
					{
						goto IL_1F8;
					}
					text = "siege_tower";
					goto IL_1F8;
				}
				if (num <= 1839032341U)
				{
					if (num != 1748194790U)
					{
						if (num != 1820818168U)
						{
							if (num != 1839032341U)
							{
								goto IL_1F8;
							}
							if (!(engineID == "trebuchet"))
							{
								goto IL_1F8;
							}
							text = "trebuchet";
							goto IL_1F8;
						}
						else if (!(engineID == "fire_onager"))
						{
							goto IL_1F8;
						}
					}
					else if (!(engineID == "fire_catapult"))
					{
						goto IL_1F8;
					}
				}
				else if (num != 1898442385U)
				{
					if (num != 2806198843U)
					{
						if (num != 4036530155U)
						{
							goto IL_1F8;
						}
						if (!(engineID == "ballista"))
						{
							goto IL_1F8;
						}
						goto IL_1D2;
					}
					else if (!(engineID == "onager"))
					{
						goto IL_1F8;
					}
				}
				else if (!(engineID == "catapult"))
				{
					goto IL_1F8;
				}
				text = "catapult";
				goto IL_1F8;
				IL_1D2:
				text = "ballista";
				IL_1F8:
				this.OutlineWidget.Sprite = ((text == string.Empty) ? this._fallbackSprite : this.GetSprite("MPHud\\SiegeMarkers\\" + text + "_outline"));
				this.IconWidget.Sprite = ((text == string.Empty) ? this._fallbackSprite : this.GetSprite("MPHud\\SiegeMarkers\\" + text));
				this._hasVisualSet = true;
			}
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x00021577 File Offset: 0x0001F777
		private Sprite GetSprite(string path)
		{
			return base.Context.SpriteData.GetSprite(path);
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06000BE4 RID: 3044 RVA: 0x0002158A File Offset: 0x0001F78A
		// (set) Token: 0x06000BE5 RID: 3045 RVA: 0x00021592 File Offset: 0x0001F792
		[Editor(false)]
		public string EngineID
		{
			get
			{
				return this._engineID;
			}
			set
			{
				if (value != this._engineID)
				{
					this._engineID = value;
					base.OnPropertyChanged<string>(value, "EngineID");
				}
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x000215B5 File Offset: 0x0001F7B5
		// (set) Token: 0x06000BE7 RID: 3047 RVA: 0x000215BD File Offset: 0x0001F7BD
		public Widget OutlineWidget
		{
			get
			{
				return this._outlineWidget;
			}
			set
			{
				if (this._outlineWidget != value)
				{
					this._outlineWidget = value;
					base.OnPropertyChanged<Widget>(value, "OutlineWidget");
				}
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06000BE8 RID: 3048 RVA: 0x000215DB File Offset: 0x0001F7DB
		// (set) Token: 0x06000BE9 RID: 3049 RVA: 0x000215E3 File Offset: 0x0001F7E3
		public Widget IconWidget
		{
			get
			{
				return this._iconWidget;
			}
			set
			{
				if (this._iconWidget != value)
				{
					this._iconWidget = value;
					base.OnPropertyChanged<Widget>(value, "IconWidget");
				}
			}
		}

		// Token: 0x04000579 RID: 1401
		private bool _hasVisualSet;

		// Token: 0x0400057A RID: 1402
		private Sprite _fallbackSprite;

		// Token: 0x0400057B RID: 1403
		private const string SpritePathPrefix = "MPHud\\SiegeMarkers\\";

		// Token: 0x0400057C RID: 1404
		private string _engineID = string.Empty;

		// Token: 0x0400057D RID: 1405
		private Widget _outlineWidget;

		// Token: 0x0400057E RID: 1406
		private Widget _iconWidget;
	}
}
