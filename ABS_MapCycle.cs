using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace ABS_MapCycle;

public class ABS_MapCycleConfig : BasePluginConfig
{
	[JsonPropertyName("PluginEnabled")] public bool PluginEnabled { get; set; } = true;
	[JsonPropertyName("EnableRandomMaps")] public bool EnableRandomMaps { get; set; } = false;
	[JsonPropertyName("EnableNoDuplicateRandomMaps")] public bool EnableNoDuplicateRandomMaps { get; set; } = true;
	[JsonPropertyName("MapCycleFile")] public string MapCycleFile { get; set; } = "mapcyclecustom.txt";
}

public class ABS_MapCycle : BasePlugin, IPluginConfig<ABS_MapCycleConfig>
{
	public override string ModuleName => "ABS_MapCycle";
	public override string ModuleVersion => "1.2";
	public override string ModuleAuthor => "austin";
	public override string ModuleDescription => "Cycles server maps listed in a text file.";
	public required ABS_MapCycleConfig Config {get; set;}

	public List<string> MapCycleList;
	public List<string> MapCycleInUseList;

	public string MapCycleFile;
	public int MapCycleIdx = 0;

	public override void	Load(bool hotReload)
	{
		Console.WriteLine($"Plugin: {this.ModuleName} - {ModuleDescription}  - Version: {this.ModuleVersion} by {this.ModuleAuthor}");

		if(!Config.PluginEnabled)
		{
			Console.WriteLine($"Alert: ABS_MapCycle is set to Disabled. Map Cycle will be disabled.");
			return;
		}

		MapCycleFile	= Path.Combine(Server.GameDirectory, "csgo", Config.MapCycleFile);
		if (!File.Exists(MapCycleFile))
		{
			Console.WriteLine($"WARNING: File not found = {MapCycleFile}\nABS_MapCycle will be disabled.");
			return;
		}
		MapCycleList = File.ReadAllLines(MapCycleFile).
			Where(s => !string.IsNullOrWhiteSpace(s)).
			Select(s => s.Trim()).
			ToList().
			ConvertAll(d => d.ToLower());

		Console.WriteLine($"----------- Loaded {MapCycleFile} -----------");
		Console.WriteLine($"EnableRandomMaps = {Config.EnableRandomMaps}");
		Console.WriteLine($"EnableNoDuplicateRandomMaps = {Config.EnableNoDuplicateRandomMaps}");
		foreach (string Map in MapCycleList)
			Console.WriteLine($"{Map}");
		Console.WriteLine("------------------------------------------------------------------");

		MapCycleList.RemoveAll(s => s.StartsWith(@"//"));

		if (Config.EnableNoDuplicateRandomMaps)
			MapCycleInUseList = MapCycleList.Distinct().ToList();
		else
			MapCycleInUseList = MapCycleList.ToList();

		if (MapCycleInUseList.Count == 0)
    {
			Console.WriteLine($"WARNING: No maps defined in {MapCycleFile}\nABS_MapCycle will be disabled.");
      return;
    }
		RegisterEventHandler<EventCsWinPanelMatch>(EventCsWinPanelMatchHandler);
	}

	public void OnConfigParsed(ABS_MapCycleConfig config)
	{
		Config = config;
	}

	public HookResult EventCsWinPanelMatchHandler(EventCsWinPanelMatch @event, GameEventInfo info)
	{
		string NextMap;
		int idx;
		float Delay;

		var convar = ConVar.Find("mp_match_restart_delay");
		if (convar is null)
			Delay = 5.0f;
		else
			Delay = convar.GetPrimitiveValue<int>();

		if(Config.EnableRandomMaps)
		{
			idx = new Random().Next(0, MapCycleInUseList.Count - 1);
			NextMap = MapCycleInUseList.ElementAtOrDefault(idx);

			if (Config.EnableNoDuplicateRandomMaps)
				MapCycleInUseList.RemoveAt(idx);

			if (MapCycleInUseList.Count() == 0)
				if (Config.EnableNoDuplicateRandomMaps)
					MapCycleInUseList = MapCycleList.Distinct().ToList();
				else
					MapCycleInUseList = MapCycleList.ToList();
		}
		else
		{
			NextMap = MapCycleInUseList[MapCycleIdx];
			if (++MapCycleIdx >= MapCycleInUseList.Count())
				MapCycleIdx = 0;
		}

		// changing to a blank map will mess up the server
		if (NextMap != "")
		{
			AddTimer(Delay, () =>
			{
				Console.WriteLine($"ABS_MapCycle Changing map to = {NextMap}");
				// if ulong parm assume workshop id
				if (ulong.TryParse(NextMap, out _))
					Server.ExecuteCommand($"host_workshop_map {NextMap}");
				else
					Server.ExecuteCommand($"map {NextMap}");
			});
		}
		else
			Console.WriteLine($"ERROR: ABS_MapCycle - Found Blank map. Not changing map.");
		return HookResult.Continue;
	}
}


