using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class HintVM : TooltipBaseVM
	{
		public HintVM(Type type, object[] args)
			: base(type, args)
		{
			base.InvokeRefreshData<HintVM>(this);
			base.IsActive = true;
		}

		protected override void OnFinalizeInternal()
		{
			base.IsActive = false;
		}

		public static void RefreshGenericHintTooltip(HintVM hint, object[] args)
		{
			string text = args[0] as string;
			hint.Text = text;
		}

		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text != value)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		private string _text = "";
	}
}
