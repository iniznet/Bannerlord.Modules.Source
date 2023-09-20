using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.FlagMarker
{
	public class SiegeEngineVisualWidget : Widget
	{
		public SiegeEngineVisualWidget(UIContext context)
			: base(context)
		{
			this._fallbackSprite = this.GetSprite("BlankWhiteCircle");
		}

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

		private Sprite GetSprite(string path)
		{
			return base.Context.SpriteData.GetSprite(path);
		}

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

		private bool _hasVisualSet;

		private Sprite _fallbackSprite;

		private const string SpritePathPrefix = "MPHud\\SiegeMarkers\\";

		private string _engineID = string.Empty;

		private Widget _outlineWidget;

		private Widget _iconWidget;
	}
}
