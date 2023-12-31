﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: DefineAsEngineStruct(typeof(AtmosphereInfo), "rglAtmosphere_info", false)]
[assembly: DefineAsEngineStruct(typeof(SunInformation), "sun_information", false)]
[assembly: DefineAsEngineStruct(typeof(RainInformation), "rain_information", false)]
[assembly: DefineAsEngineStruct(typeof(SnowInformation), "snow_information", false)]
[assembly: DefineAsEngineStruct(typeof(AmbientInformation), "ambient_information", false)]
[assembly: DefineAsEngineStruct(typeof(FogInformation), "fog_information", false)]
[assembly: DefineAsEngineStruct(typeof(SkyInformation), "sky_information", false)]
[assembly: DefineAsEngineStruct(typeof(TimeInformation), "time_information", false)]
[assembly: DefineAsEngineStruct(typeof(AreaInformation), "area_information", false)]
[assembly: DefineAsEngineStruct(typeof(PostProcessInformation), "post_process_information", false)]
[assembly: DefineAsEngineStruct(typeof(InputKey), "rglInput_key", false)]
[assembly: DefineAsEngineStruct(typeof(BodyFlags), "rglBody_flags", false)]
[assembly: InternalsVisibleTo("TaleWorlds.Generator.Library")]
[assembly: InternalsVisibleTo("TaleWorlds.Generator.Library.Engine")]
[assembly: InternalsVisibleTo("TaleWorlds.Engine.AutoGenerated")]
[assembly: InternalsVisibleTo("TaleWorlds.MountAndBlade")]
[assembly: InternalsVisibleTo("TaleWorlds.MountAndBlade.AutoGenerated")]
[assembly: InternalsVisibleTo("TaleWorlds.WarRide.Game")]
[assembly: InternalsVisibleTo("TaleWorlds.WarRide.AutoGenerated")]
[assembly: InternalsVisibleTo("TaleWorlds.WarRide.Diamond.Client")]
[assembly: InternalsVisibleTo("TaleWorlds.WarRide.Diamond.Client.AutoGenerated")]
[assembly: InternalsVisibleTo("TaleWorlds.WarRide.Diamond.BattleServer")]
[assembly: InternalsVisibleTo("TaleWorlds.WarRide.Diamond.BattleServer.AutoGenerated")]
[assembly: AssemblyCompany("TaleWorlds.Engine")]
[assembly: AssemblyConfiguration("Shipping_Client")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("TaleWorlds.Engine")]
[assembly: AssemblyTitle("TaleWorlds.Engine")]
