using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Extensions;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace ABS_MapCycle;

public class ABS_MapCycleConfig : BasePluginConfig
{
	[JsonPropertyName("ConfigVersion")] public override int Version {get;set;} = 1;
	[JsonPropertyName("PluginEnabled")] public bool PluginEnabled { get; set; } = true;
	[JsonPropertyName("EnableRandomMaps")] public bool EnableRandomMaps { get; set; } = false;
	[JsonPropertyName("EnableNoDuplicateRandomMaps")] public bool EnableNoDuplicateRandomMaps { get; set; } = true;
	[JsonPropertyName("EnableNextMapMessage")] public bool EnableNextMapMessage { get; set; } = true;
	[JsonPropertyName("MapCycleFile")] public string MapCycleFile { get; set; } = "mapcyclecustom.txt";	
}

public class ABS_MapCycle : BasePlugin, IPluginConfig<ABS_MapCycleConfig>
{
	public override string ModuleName => "ABS_MapCycle";
	public override string ModuleVersion => "1.4";
	public override string ModuleAuthor => "austin";
	public override string ModuleDescription => "Cycles server maps listed in a text file";
	public required ABS_MapCycleConfig Config {get; set;} = new();

	public string MapCycleFile;
	public List<string> MapCycleList;
	public List<string> MapCycleInUseList;
	public int MapCycleIdx = 0;

	public override void	Load(bool hotReload)
	{
		Console.WriteLine("------------------------------------------------------------------");
		Console.WriteLine($"Plugin: {this.ModuleName} - {ModuleDescription}  - Version: {this.ModuleVersion} by {this.ModuleAuthor}");

		if(!Config.PluginEnabled)
		{
			Console.WriteLine($"Alert: ABS_MapCycle is set to Disabled. Map Cycle will be disabled.");
			// returning before the RegisterEventHandler below effectively disables the plugin
			return;
		}

		MapCycleFile	= Path.Combine(Server.GameDirectory, "csgo", Config.MapCycleFile);
		if (!File.Exists(MapCycleFile))
		{
			Console.WriteLine($"WARNING: File not found = {MapCycleFile}\nABS_MapCycle will be disabled.");
			return;
		}

		// create a list for the map cycle from the map cycle file
		// remove blank lines and leading and trailing white spaces
		// and for no reason lower case everything
		MapCycleList = File.ReadAllLines(MapCycleFile).
			Where(s => !string.IsNullOrWhiteSpace(s)).
			Select(s => s.Trim()).
			ToList().
			ConvertAll(d => d.ToLower());

		// show the map list including any comment lines
		Console.WriteLine("---------------------------------");
		Console.WriteLine($"EnableRandomMaps = {Config.EnableRandomMaps}");
		Console.WriteLine($"EnableNoDuplicateRandomMaps = {Config.EnableNoDuplicateRandomMaps}");
		Console.WriteLine($"EnableNextMapMessage = {Config.EnableNextMapMessage}");
		Console.WriteLine($"MapCycleFile = {Config.MapCycleFile}");
		Console.WriteLine($"Loaded {MapCycleFile}");
		Console.WriteLine("---------------------------------");
		foreach (string Map in MapCycleList)
			Console.WriteLine($"{Map}");
		Console.WriteLine("------------------------------------------------------------------");

		// remove the comment lines
		MapCycleList.RemoveAll(s => s.StartsWith(@"//"));

		// if we are in random map switching mode and disallowing dupliates
		// remove any duplicate maps in the list
		// then set the InUse list to this filtered list
		// otherwise set the InUse list to the full list which allows duplicates
		//
		// we keep the full MapCycleList and the InUseList
		// because if we are in random mode and no duplicates
		// it is very easy to just remove the switched to maps 
		// from the list to avoid picking them agian
		// once all maps are run and the list is empty we "refill it"
		// from the mapCycleList 
		//
		// if we are not in random mode the InUse is just cycled over and over
		//
		if (Config.EnableRandomMaps && Config.EnableNoDuplicateRandomMaps)
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
		try
		{
			if (config.Version < Config.Version)
			{
				int NewVersion = Config.Version;
				Config = config;
				Config.Version = NewVersion;
				Config.Update();
			}
			else
				Config = config;
		}
		catch(Exception e)
		{
			var st = new StackTrace(e, true);
			var frame = st.GetFrame(0);
			var line = frame.GetFileLineNumber();
			Console.WriteLine($"!EXCEPTION - ABS_MapCycle - OnConfigParsed() \nLine {line}\n{e.Message}",true);
		}
	}
	public HookResult EventCsWinPanelMatchHandler(EventCsWinPanelMatch @event, GameEventInfo info)
	{
		string NextMap;
		int idx;

		/*
			after all rounds have played and the match has ended 
			two time delays are used before the game switches the map
			mp_win_panel_display_time
			mp_match_restart_delay

			if mp_win_panel_display_time > mp_match_restart_delay
			You will see the win panel screen the entire time
			and you will not see the map vote screen
			Total delay in seconds after match end before the map switch will be
			mp_win_panel_display_time 

			if mp_match_restart_delay > mp_win_panel_display_time
			You will see the win panel for mp_win_panel_display time
			then you will see the map vote screen for match_restart_delay -  mp_win_panel_display_time
			Total delay in seconds after match end before the map switch will be
			mp_match_restart_delay 

			We have to switch the map before this time expires because
			if they have mp_endmatch_votenextmap 0
			after the time expires the game will try to switch to a blank map 
			and the server will either crash or go into a strange nonfunctioning state.
		*/
		
		float WinPanelDelay;
		int MatchRestartDelay;
		float Delay;
		
		var mp_win_panel_display_time = ConVar.Find("mp_win_panel_display_time");
		if (mp_win_panel_display_time is null)
			WinPanelDelay = 1;
		else
			WinPanelDelay = mp_win_panel_display_time.GetPrimitiveValue<float>();

		var mp_match_restart_delay = ConVar.Find("mp_match_restart_delay");
		if (mp_match_restart_delay is null)
			MatchRestartDelay = 1;
		else
			MatchRestartDelay = mp_match_restart_delay.GetPrimitiveValue<int>();

		Delay = Math.Max(WinPanelDelay, MatchRestartDelay);
		// switch map 5 seconds before game tries to switch map or immediately if game is for no delay
		Delay = Math.Max(0, Delay-5);
	
		if(Config.EnableRandomMaps)
		{
			idx = new Random().Next(0, MapCycleInUseList.Count - 1);
			NextMap = MapCycleInUseList.ElementAtOrDefault(idx);

			// to avoid changing to duplicate maps we remove maps from the UseList as they are used
			if (Config.EnableNoDuplicateRandomMaps)
				MapCycleInUseList.RemoveAt(idx);

			// once we have been through all maps the list will be empty
			// and we set it back to the full list read in orginally (that hasn't been modified)
			if (MapCycleInUseList.Count() == 0)
				if (Config.EnableNoDuplicateRandomMaps)
					// we strip out dupes from the list for EnableNoDuplicateRandomMaps = true
					MapCycleInUseList = MapCycleList.Distinct().ToList();
				else
					// we allow duplicate maps in the list even for random for EnableNoDuplicateRandomMaps = false
					MapCycleInUseList = MapCycleList.ToList();
		}
		else
		{
			// we are not in ramdom. just keep cycling the InUseList in order
			NextMap = MapCycleInUseList[MapCycleIdx];
			if (++MapCycleIdx >= MapCycleInUseList.Count())
				MapCycleIdx = 0;
		}

		// changing to a blank map will mess up the server
		if (NextMap != "")
		{
			if (Config.EnableNextMapMessage)
				Server.PrintToChatAll($"Next Map {ChatColors.Green}{NextMap}");
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
