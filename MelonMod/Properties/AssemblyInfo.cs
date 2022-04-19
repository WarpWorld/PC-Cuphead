using System;
using System.Reflection;
using ML_CrowdControl;

[assembly: AssemblyTitle(BuildInfo.Description)]
[assembly: AssemblyDescription(BuildInfo.Description)]
[assembly: AssemblyCompany(BuildInfo.Company)]
[assembly: AssemblyProduct(BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BuildInfo.Author)]
[assembly: AssemblyTrademark(BuildInfo.Company)]
[assembly: AssemblyVersion(BuildInfo.Version)]
[assembly: AssemblyFileVersion(BuildInfo.Version)]
[assembly: MelonLoader.MelonInfo(typeof(ModCore), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author, BuildInfo.DownloadLink)]
[assembly: MelonLoader.MelonColor(ConsoleColor.Magenta)]
[assembly: MelonLoader.MelonGame(null, null)]
[assembly: MelonLoader.MelonPriority(-int.MaxValue)]