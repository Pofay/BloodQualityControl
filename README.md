# BloodQualityControl

A VRising mod to control the blood quality values of all creatures and mobs in the game that have a blood source.


# Commands

*Note: All commands need admin priveleges*

This mod will be enabled once you run any of the following commands to change blood quality settings:

- To set the blood quality range of all spawned units:
```
.bloodquality .range <MINIMUM> <MAXIMUM>

# e.g
.bloodquality .range 30 80 
```

- To set the minimum blood quality range of all spawned units:
```
.bloodquality .min <MINIMUM>

# e.g
.bloodquality .min 35
```

- To set the maximum blood quality range of all spawned units:
```
.bloodquality .max <MAXIMUM>

# e.g 
.bloodquality .max 70
```

- To disable the mod
```
.bloodquality .disable
```

These commands will save to the config file so that you don't have to rerun them every time the server is restarted:

```
[Main]

## Determines whether the mod is enabled or not.
# Setting type: Boolean
# Default value: false
Enabled = true

## The minimum blood quality that units will spawn with. Should be a value between 5-100 and must not be higher than the MaxBloodQuality.
# Setting type: Single
# Default value: 5
MinimumBloodQuality = 50

## The maximum blood quality that units will spawn with. Should be a value between 5-100 and must not be lower than the MinBloodQuality.
# Setting type: Single
# Default value: 100
MaximumBloodQuality = 100
```

## Important Note:

The mod will only change the blood quality of newly spawned mobs. It will not affect the mobs already spawned by the game.

So for servers that just installed this mod, you may need to perform a cull (kill on sight) so that the mobs will respawn with your applied blood quality settings the next time you see them.