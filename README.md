# cs2-Change-Maps-with-mapcycle.txt
This plugin will cycle the server maps from the list of maps in .\csgo\maplist.txt file.

It supports standard map names workshop map ids and comment lines.
For example:
cs_italy
de_nuke
// de_subzero
3270832263
cs_office

When using this plugin turn off end match map voting with these cvars.

mp_endmatch_votenextmap 0
mp_endmatch_votenextleveltime 0

If you have end match voting enabled it will be ignored and after the vote it will still switch to the next map in the mapcycle.txt file.
