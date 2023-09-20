using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class KingdomTabControlListPanel : ListPanel
	{
		public KingdomTabControlListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.FiefsButton.IsSelected = this.FiefsPanel.IsVisible;
			this.PoliciesButton.IsSelected = this.PoliciesPanel.IsVisible;
			this.ClansButton.IsSelected = this.ClansPanel.IsVisible;
			this.ArmiesButton.IsSelected = this.ArmiesPanel.IsVisible;
			this.DiplomacyButton.IsSelected = this.DiplomacyPanel.IsVisible;
		}

		[Editor(false)]
		public Widget DiplomacyPanel
		{
			get
			{
				return this._diplomacyPanel;
			}
			set
			{
				if (this._diplomacyPanel != value)
				{
					this._diplomacyPanel = value;
					base.OnPropertyChanged<Widget>(value, "DiplomacyPanel");
				}
			}
		}

		[Editor(false)]
		public Widget ArmiesPanel
		{
			get
			{
				return this._armiesPanel;
			}
			set
			{
				if (this._armiesPanel != value)
				{
					this._armiesPanel = value;
					base.OnPropertyChanged<Widget>(value, "ArmiesPanel");
				}
			}
		}

		[Editor(false)]
		public Widget ClansPanel
		{
			get
			{
				return this._clansPanel;
			}
			set
			{
				if (this._clansPanel != value)
				{
					this._clansPanel = value;
					base.OnPropertyChanged<Widget>(value, "ClansPanel");
				}
			}
		}

		[Editor(false)]
		public Widget PoliciesPanel
		{
			get
			{
				return this._policiesPanel;
			}
			set
			{
				if (this._policiesPanel != value)
				{
					this._policiesPanel = value;
					base.OnPropertyChanged<Widget>(value, "PoliciesPanel");
				}
			}
		}

		[Editor(false)]
		public Widget FiefsPanel
		{
			get
			{
				return this._fiefsPanel;
			}
			set
			{
				if (this._fiefsPanel != value)
				{
					this._fiefsPanel = value;
					base.OnPropertyChanged<Widget>(value, "FiefsPanel");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget FiefsButton
		{
			get
			{
				return this._fiefsButton;
			}
			set
			{
				if (this._fiefsButton != value)
				{
					this._fiefsButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FiefsButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget PoliciesButton
		{
			get
			{
				return this._policiesButton;
			}
			set
			{
				if (this._policiesButton != value)
				{
					this._policiesButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "PoliciesButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget ClansButton
		{
			get
			{
				return this._clansButton;
			}
			set
			{
				if (this._clansButton != value)
				{
					this._clansButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ClansButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget ArmiesButton
		{
			get
			{
				return this._armiesButton;
			}
			set
			{
				if (this._armiesButton != value)
				{
					this._armiesButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ArmiesButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget DiplomacyButton
		{
			get
			{
				return this._diplomacyButton;
			}
			set
			{
				if (this._diplomacyButton != value)
				{
					this._diplomacyButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "DiplomacyButton");
				}
			}
		}

		private Widget _armiesPanel;

		private Widget _clansPanel;

		private Widget _policiesPanel;

		private Widget _fiefsPanel;

		private Widget _diplomacyPanel;

		private ButtonWidget _fiefsButton;

		private ButtonWidget _clansButton;

		private ButtonWidget _policiesButton;

		private ButtonWidget _armiesButton;

		private ButtonWidget _diplomacyButton;
	}
}
