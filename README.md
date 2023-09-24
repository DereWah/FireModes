# FireModes ![Downloads](https://img.shields.io/github/downloads/DereWah/FireModes/total)
Single, Burst and Auto FireModes for SCP:SL weapons! 

Simply press the button "ALT" to switch between fire modes.
(If it doesn't work, use the button you mapped to No-Clip-Toggle in the settings)

## Config

The configuration is really simple. Simply add in the list the weapons you'd like FireModes to work with and you're set!

## Concept

For anyone interested, here's how the Plugin works: basically, in SCP:SL there is no clean way of preventing a player from shooting.
It is possible to cancel the shooting event from the server, but in high-latency-conditions that would end up with player sometimes being able to shoot multiple rounds or other syncing issues.

To avoid this, I decided to manually edit the amount of ammos in the weapon magazine to match the fire mode. Therefore, when you switch ammos, your gun will have 1, 3, or all of the ammos in the current mag.

However, by doing this, it would screw up the mag capacity. If a player fires 1 shot and reloads, their ammo is set back to 1 (because of the fire mode being set to single), therefore wasting the 30 or so bullets they put in the gun.
To prevent this I made a completely virtual magazine. It tracks all of the ammo in the gun, updates whenever a player shoots, and is emptied / filled whenever the player reloads or unloads the gun. By doing so, we can track all of the storage
of the gun in the plugin's memory, and leave to the game the task to just give to the player the bullets they have in the chamber.

Hope this explanation was exhaustive enough, if you have any doubt or think you found a bug feel free to open an issue on github ;)
