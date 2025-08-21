# cs2-Change-Maps-with-mapcycle.txt
This is a Counter Strike Sharp plugin for Counter Strike 2 dedicated servers.<br>
https://developer.valvesoftware.com/wiki/Counter-Strike_2/Dedicated_Servers<br>
https://github.com/roflmuffin/CounterStrikeSharp<br>
<br>
This plugin will cycle the cs2 server maps from a list of maps in a text file.
<br>
It supports standard map names workshop map ids and comment lines.<br>
For example:<br>
cs_italy<br>
de_nuke<br>
// de_subzero<br>
3270832263<br>
de_nuke
cs_office<br>
<br>
When using this plugin turn off end match map voting with these cvars.<br>
<br>
mp_endmatch_votenextmap 0<br>
mp_endmatch_votenextleveltime 0<br>
<br>
If you have end match voting enabled it will be ignored and after the vote it will still switch to the next map in the map cycle file.<br>
<br>
# Installing
The dll will run on both Linux and Windows.<br>
Once you have Counter Strike Sharp installed extract the files in the zip keeping the folder structure in the zip starting from your .\csgo folder.<br>
<br>
.\csgo\<br>
.\csgo\mapcyclecustom.txt<br>
<br>
.\csgo\addons\CounterStrikeSharp\configs\plugins<br>
.\csgo\addons\CounterStrikeSharp\configs\plugins\ABS_MapCycle<br>
.\csgo\addons\CounterStrikeSharp\configs\plugins\ABS_MapCycle\ABS_MapCycle.json<br>
<br>
.\csgo\addons\CounterStrikeSharp\plugins\ABS_MapCycle<br>
.\csgo\addons\CounterStrikeSharp\plugins\ABS_MapCycle\ABS_MapCycle.deps.json<br>
.\csgo\addons\CounterStrikeSharp\plugins\ABS_MapCycle\ABS_MapCycle.dll<br>
.\csgo\addons\CounterStrikeSharp\plugins\ABS_MapCycle\ABS_MapCycle.pdb<br>
<br>
# Settings
The ABS_MapCycle config file<br>
.\csgo\addons\CounterStrikeSharp\configs\plugins\ABS_MapCycle\ABS_MapCycle.json<br>
can be edited with a text editor and has these settings.<br>
<br>
You can disable the plugin without uninstalling it by setting this to false.<br>
"PluginEnabled": true,<br>
<br>
This setting specifies the name of the map cycle file used. You can specify any file name you want but the file has to be placed in the .csgo folder<br>
<br>
Note: cs# does things with the mapcycle.txt file so we recommend you do not use this name. If you do you will see warning messages from cs# about invalid map names for your lines that are comments or workshop map ids.<br>
"MapCycleFile": "mapcyclecustom.txt",<br>
<br>
When this is set to true instead of changing the maps in the order in the map cycle file it randomizes the next map.<br>
"EnableRandomMaps": false,<br>
<br>
When this is set to true it keeps maps that have already been played from being choses again in the random pick. All of the files in your map cycle file will be played without duplicates and then the full list will be used over.<br>
"EnableNoDuplicateRandomMaps": true,<br>
<br>
Note: This does not prevent you from putting the same map in your map cycle file more than once. It only works with EnableRandomMaps = true;<br>
This gives you the most flexibility.<br>
<br>
You can have random maps and duplicates picked in the random selection.<br>
"EnableRandomMaps": true,<br>
"EnableNoDuplicateRandomMaps": false,<br>
<br>
You can have random maps and no duplicates picked in the random selection.<br>
"EnableRandomMaps": true,<br>
"EnableNoDuplicateRandomMaps": true,<br>
<br>
And you can turn off random selection and put duplicate map names and ws id in your map list or not. This way if you want to do something like this you can. <br>
de_nuke<br>
cs_italy<br>
de_nuke<br>
// de_subzero<br>
3270832263<br>
cs_office<br>
<br>
Note: for a cs# pluings to pick up changes to their config files they have to be reloaded.<br>
THey will not get the changes with just a map change.<br>
In your server console type:<br>
css_plugins reload ABS_MapCycle<br>
<br>
The cs# config files are json format files and must have exact syntax or they will be broken and not work. Be sure to keep the formatting with quotes and commas in them.<br>
<br>
# Delay before Map Changes
After all of the rounds in a match are played and the match ends cs2 switches to the win panel where it shows players and some stats on them. The amount of time this screens displays is determined by this setting.<br>
mp_win_panel_display_time 10<br>
<br>
After this cs2 switches to the map vote screen. Even if you have mp_endmatch_votenextmap 0 it will still switch to the vote screen and (if your mapgroup is correctly configured) it will show the map tiles and look like it is allowing the vote but with mp_endmatch_votenextmap 0 any map choices will be ignored and with endmatch_votenextmap 1 and ABS_MapCycle enabled any map vote will still be ignored and ABS_MapCycle will still pick the next map to switch to.<br>
<br>
How long this vote screen displays for is set by this cvar.<br>
mp_match_restart_delay<br>
<br>
Also note this setting does not extend any time. It only sets how long you can click on a map tile to vote for it -during- the mp_match_restart_delay time.<br>
mp_endmatch_votenextleveltime<br>
<br>
ABS_MapCycle does not switch to the next map until this total time has expired.<br>
mp_win_panel_display_time + mp_match_restart_delay<br>
<br>
When using ABS_MapCycle to switch your maps it makes the most sense to have these settings.<br>
<br>
Set this time for how long you want to see the end of match screen showing players and starts<br>
mp_win_panel_display_time<br>
And then set this time to 0 so you do not see anything with the map voting and have the map switch right after mp_win_panel_display_time time expires.<br>
mp_match_restart_delay 0<br>
<br>
