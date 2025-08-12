# cs2-Change-Maps-with-mapcycle.txt
This plugin will cycle the cs2 server maps from the list of maps in .\csgo\mapcycle.txt<br>
<br>
It supports standard map names workshop map ids and comment lines.<br>
For example:<br>
cs_italy<br>
de_nuke<br>
// de_subzero<br>
3270832263<br>
cs_office<br>
<br>
When using this plugin turn off end match map voting with these cvars.<br>
<br>
mp_endmatch_votenextmap 0<br>
mp_endmatch_votenextleveltime 0<br>
<br>
If you have end match voting enabled it will be ignored and after the vote it will still switch to the next map in the mapcycle.txt file.<br>
