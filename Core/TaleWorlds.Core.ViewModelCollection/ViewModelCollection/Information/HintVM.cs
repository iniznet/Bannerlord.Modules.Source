using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class HintVM : TooltipVM
	{
		public override void OnShowTooltip(Type type, object[] args)
		{
			if (type == this._registeredType)
			{
				this.Text = args[0] as string;
				this._tryShowHint = true;
			}
		}

		public override void OnHideTooltip()
		{
			this._tryShowHint = false;
			base.IsActive = false;
			this._currentDelay = 0f;
		}

		public override void Tick(float dt)
		{
			if (this._tryShowHint)
			{
				if (this._currentDelay > 0f)
				{
					base.IsActive = true;
					return;
				}
				this._currentDelay += dt;
			}
		}

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

		private readonly Type _registeredType = typeof(string);

		private const float _delay = 0f;

		private float _currentDelay;

		private bool _tryShowHint;

		private string _text = "";
	}
}
