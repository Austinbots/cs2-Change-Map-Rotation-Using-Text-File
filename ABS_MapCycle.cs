using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
namespace ABS_MapCycle;
public class ABS_MapCycle : BasePlugin
{
	public override string ModuleName => "ABS_MapCycle";
	public override string ModuleVersion => "1.1";
	public override string ModuleAuthor => "austin";
	public override string ModuleDescription => "Cycle server maps with .\\csgo\\mapcycle.txt";

	public List<string> MapCycle;
	public int MapCycleIdx = 0;

	public override void	Load(bool hotReload)
	{
		Console.WriteLine($"Plugin: {this.ModuleName} - {ModuleDescription}  - Version: {this.ModuleVersion} by {this.ModuleAuthor}");
		RegisterEventHandler<EventCsWinPanelMatch>(EventCsWinPanelMatchHandler);
		MapCycle = File.ReadAllLines(Server.GameDirectory+"\\csgo\\mapcycle.txt").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
		Console.WriteLine("----------- Loaded .\\csgo\\MapCycle.txt -----------");
		foreach (string map in MapCycle)
			Console.WriteLine($"{map}");
		Console.WriteLine("--------------------------------------------------");
		MapCycle.RemoveAll(x => x.Trim(' ').StartsWith(@"//"));
	}
	public HookResult EventCsWinPanelMatchHandler(EventCsWinPanelMatch @event, GameEventInfo info)
	{
		// if ulong parm assume workshop id
		if (ulong.TryParse(MapCycle[MapCycleIdx], out _))
			Server.ExecuteCommand("host_workshop_map " + MapCycle[MapCycleIdx].Trim(' '));
		else
			Server.ExecuteCommand("map " + MapCycle[MapCycleIdx].Trim(' '));
		if (++MapCycleIdx >= MapCycle.Count())
			MapCycleIdx = 0;
		return HookResult.Handled;
	}
}
