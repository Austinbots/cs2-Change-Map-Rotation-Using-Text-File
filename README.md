# cs2-Change-Map-Rotation-Using-Text-File
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
de_nuke<br>
cs_office<br>

# Installing
The plugin will run on both Linux and Windows.<br>
Once you have Counter Strike Sharp installed extract the files in the zip keeping the folder structure in the zip starting from your .\csgo folder.<br>
<br>
.\csgo\mapcyclecustom.txt<br>
<br>
.\csgo\addons\counterstrikesharp\configs\plugins\ABS_MapCycle\ABS_MapCycle.json<br>
<br>
.\csgo\addons\ counterstrikesharp \plugins\ABS_MapCycle\ABS_MapCycle.deps.json<br>
.\csgo\addons\ counterstrikesharp \plugins\ABS_MapCycle\ABS_MapCycle.dll<br>
.\csgo\addons\ counterstrikesharp \plugins\ABS_MapCycle\ABS_MapCycle.pdb

# Settings
The ABS_MapCycle config file<br>
.\csgo\addons\counterstrikesharp\configs\plugins\ABS_MapCycle\\<b>ABS_MapCycle.json</b><br>
can be edited with a text editor and has these settings.<br>
<br>
You can disable the plugin without uninstalling it by setting this to false.<br>
<b>"PluginEnabled"</b>: true,<br>
<br>
This setting specifies the name of the map cycle file used. You can specify any file name you want but the file has to be placed in the .csgo folder<br>
<br>
Note: cs# does things with the mapcycle.txt file so we recommend you do not use this default name mapcycle.txt. If you do you will see warning messages from cs# about invalid map names for your lines that are comments or workshop map ids.<br>
<b>"MapCycleFile"</b>: "mapcyclecustom.txt",<br>
<br>
When this is set to true instead of changing the maps in the order in the map cycle file it randomizes the next map.<br>
<b>"EnableRandomMaps"</b>: false,<br>
<br>
When this is set to true it keeps maps that have already been played from being chosen again in the random pick. All of the files in your map cycle file will be played without duplicates and then the full list will be used over again.<br>
<b>"EnableNoDuplicateRandomMaps"</b>: true,<br>
<br>
Note: This does not prevent you from putting the same map in your map cycle file more than once. It only works with EnableRandomMaps = true;<br>
<br>
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
Note: for a cs# plugins to pick up changes to their config files they have to be reloaded. They will not get the changes with just a map change.<br>
In your server console type:<br>
<b>css_plugins reload ABS_MapCycle</b><br>
<br>
The cs# config files are json format files and must have exact syntax or they will be broken and not work. Be sure to keep the formatting with quotes and commas in them.<br>
<br>
# Delay before Map Changes<br>
After the last round of the match ends cs2 shows a "Round Won/Loss" alert on screen. The amount of time this displays is determined by this setting.<br>
mp_win_panel_display_time 10<br>
<br>
After this time expires cs2 switches to the map vote screen. Even if you have mp_endmatch_votenextmap 0 it will still switch to the vote screen and (if your mapgroup is correctly configured) it will show the map tiles and look like it is allowing the vote but with mp_endmatch_votenextmap 0 any map choices will be ignored and even with endmatch_votenextmap 1 and ABS_MapCycle enabled any map vote will still be ignored and ABS_MapCycle will still pick the next map to switch to.<br>
<br>
After all rounds have played and the match has ended two time delays determine how long before the map changes.<br>
mp_win_panel_display_time<br>
mp_match_restart_delay<br>
<br>
If mp_win_panel_display_time > mp_match_restart_delay<br>
You will see the win panel screen the entire time and you will not see the map vote screen. <br>
Total delay in seconds after match end before the map switches will be<br>
mp_win_panel_display_time<br>
<br>
If mp_match_restart_delay > mp_win_panel_display_time<br>
You will see the win panel for mp_win_panel_display time<br>
then you will see the map vote screen for match_restart_delay -  mp_win_panel_display_time<br>
Total delay in seconds after match end before the map switches will be<br>
mp_match_restart_delay<br>
<br>
We have to switch the map before this time expires because<br>
if this is set mp_endmatch_votenextmap 0<br>
after the time expires the game will try to switch to a blank map and the server will either crash or go into a strange nonfunctioning state. This is a strange thing with the game and not caused by this plugin.<br>
<br>
Also note this setting<br>
mp_endmatch_votenextleveltime<br>
does not extend any time. It only sets how long you can click on a map tile to vote for it -during- the mp_match_restart_delay time.<br>
<br>
When using ABS_MapCycle to switch your maps it makes the most sense to have these settings.<br>
<br>
Set this time for how long you want to see the round end screen. This delay happen after every round not just the last round in a match.<br>
mp_win_panel_display_time 5<br>
<br>
And then set this time to 0 so you do not see anything with the map voting and have the maps switch right after mp_win_panel_display_time time expires.<br>
mp_match_restart_delay 0<br>
<br>
When using this plugin map voting is ignored and the map is set to the next map in the rotation but we still recommend you set this to 1<br> 
mp_endmatch_votenextmap 1<br>
because in the event some settings of these delays cause us to not do the map switch before the game does the switch at least your server will switch to a map in your mapgoup instead of crashing or going into a strange non responsive state. (This is the game and not caused by this plugin). If you set this to 0<br>
mp_match_restart_delay 0 you will not see the vote screen even with mp_endmatch_votenextmap 1<br> 
<br>
If you enable the show next map message to have the message on the screen long enough to see it you will have to set this to something like 15-20 seconds.
mp_match_restart_delay 
