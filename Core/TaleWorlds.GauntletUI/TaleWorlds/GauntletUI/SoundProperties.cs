using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000029 RID: 41
	public class SoundProperties
	{
		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000DDB2 File Offset: 0x0000BFB2
		public IEnumerable<KeyValuePair<string, AudioProperty>> RegisteredStateSounds
		{
			get
			{
				foreach (KeyValuePair<string, AudioProperty> keyValuePair in this._stateSounds)
				{
					yield return keyValuePair;
				}
				Dictionary<string, AudioProperty>.Enumerator enumerator = default(Dictionary<string, AudioProperty>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0000DDC2 File Offset: 0x0000BFC2
		public IEnumerable<KeyValuePair<string, AudioProperty>> RegisteredEventSounds
		{
			get
			{
				foreach (KeyValuePair<string, AudioProperty> keyValuePair in this._eventSounds)
				{
					yield return keyValuePair;
				}
				Dictionary<string, AudioProperty>.Enumerator enumerator = default(Dictionary<string, AudioProperty>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000DDD2 File Offset: 0x0000BFD2
		public SoundProperties()
		{
			this._stateSounds = new Dictionary<string, AudioProperty>();
			this._eventSounds = new Dictionary<string, AudioProperty>();
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000DDF0 File Offset: 0x0000BFF0
		public void AddStateSound(string state, AudioProperty audioProperty)
		{
			this._stateSounds.Add(state, audioProperty);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000DDFF File Offset: 0x0000BFFF
		public void AddEventSound(string state, AudioProperty audioProperty)
		{
			if (this._eventSounds.ContainsKey(state))
			{
				this._eventSounds[state] = audioProperty;
				return;
			}
			this._eventSounds.Add(state, audioProperty);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000DE2C File Offset: 0x0000C02C
		public void FillFrom(SoundProperties soundProperties)
		{
			this._stateSounds = new Dictionary<string, AudioProperty>();
			this._eventSounds = new Dictionary<string, AudioProperty>();
			foreach (KeyValuePair<string, AudioProperty> keyValuePair in soundProperties._stateSounds)
			{
				string key = keyValuePair.Key;
				AudioProperty value = keyValuePair.Value;
				AudioProperty audioProperty = new AudioProperty();
				audioProperty.FillFrom(value);
				this._stateSounds.Add(key, audioProperty);
			}
			foreach (KeyValuePair<string, AudioProperty> keyValuePair2 in soundProperties._eventSounds)
			{
				string key2 = keyValuePair2.Key;
				AudioProperty value2 = keyValuePair2.Value;
				AudioProperty audioProperty2 = new AudioProperty();
				audioProperty2.FillFrom(value2);
				this._eventSounds.Add(key2, audioProperty2);
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000DF28 File Offset: 0x0000C128
		public AudioProperty GetEventAudioProperty(string eventName)
		{
			if (this._eventSounds.ContainsKey(eventName))
			{
				return this._eventSounds[eventName];
			}
			return null;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000DF46 File Offset: 0x0000C146
		public AudioProperty GetStateAudioProperty(string stateName)
		{
			if (this._stateSounds.ContainsKey(stateName))
			{
				return this._stateSounds[stateName];
			}
			return null;
		}

		// Token: 0x04000179 RID: 377
		private Dictionary<string, AudioProperty> _stateSounds;

		// Token: 0x0400017A RID: 378
		private Dictionary<string, AudioProperty> _eventSounds;
	}
}
