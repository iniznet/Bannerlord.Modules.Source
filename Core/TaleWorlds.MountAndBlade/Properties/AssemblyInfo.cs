﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using TaleWorlds.Core;
using TaleWorlds.DotNet;

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: InternalsVisibleTo("TaleWorlds.MountAndBlade.AutoGenerated")]
[assembly: InternalsVisibleTo("TaleWorlds.MountAndBlade.Multiplayer")]
[assembly: InternalsVisibleTo("TaleWorlds.Generator.Bannerlord")]
[assembly: DefineAsEngineStruct(typeof(AgentState), "Agent_state", false)]
[assembly: DefineAsEngineStruct(typeof(DamageTypes), "Damage_type", false)]
[assembly: DefineAsEngineStruct(typeof(AgentAttackType), "Agent_attack_type", false)]
[assembly: DefineAsEngineStruct(typeof(StrikeType), "Strike_type", false)]
[assembly: DefineAsEngineStruct(typeof(ItemFlags), "Item_flags", false)]
[assembly: DefineAsEngineStruct(typeof(WeaponFlags), "Weapon_flags", false)]
[assembly: DefineAsEngineStruct(typeof(WeaponClass), "Weapon_class", false)]
[assembly: DefineAsEngineStruct(typeof(Equipment.UnderwearTypes), "Underwear_mesh_type", false)]
[assembly: DefineAsEngineStruct(typeof(BodyProperties), "Body_properties", false)]
[assembly: DefineAsEngineStruct(typeof(DynamicBodyProperties), "Dynamic_body_properties", false)]
[assembly: DefineAsEngineStruct(typeof(StaticBodyProperties), "Static_body_properties", true)]
[assembly: DefineAsEngineStruct(typeof(MissionInitializerRecord), "Mission_initializer_record", false)]
[assembly: DefineAsEngineStruct(typeof(EquipmentIndex), "Weapon_slots_in_equipment", false)]
[assembly: AssemblyCompany("TaleWorlds.MountAndBlade")]
[assembly: AssemblyConfiguration("Shipping_Client")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("TaleWorlds.MountAndBlade")]
[assembly: AssemblyTitle("TaleWorlds.MountAndBlade")]
