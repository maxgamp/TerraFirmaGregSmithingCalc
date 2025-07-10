# TerraFirmaGregSmithingCalc

A simple, overengineered, console tool to calculate which smithing moves you have to do.\
Best used with the Texturepack that gives you the ability to read the required pointnumber for your desired end product.\
It's [found here](https://www.curseforge.com/minecraft/texture-packs/tfc-anvil-helper), outdated but still works for me!

## How to use:
By default the application assumes the initial point value is 0 and you are only asked for the point target you want to reach, this behaviour can be changed by setting "AssumeStartingPointsZero" to false in the config.json.\
Then you are asked to select the finishing moves from last to third last (from left to right as displayed in the game) and hit C to \[C\]alculate.\
You should <sub>almost</sub> instantly get a list of moves which bring you to your target points, this is formatted like so:


|Count|MoveName   |Change|Points|
|-----|-----------|------|------|
| 01  | Punch     | +2   | 002  |
| 02  | Upset     | +13  | 28   |
| 01  | Medium Hit| -6   | 22   |

Just slightly less formatted in the console output... But here are the explanations for each column:
> "Count": Tells you how often you should use this move.\
> "MoveName": The name of the move in game.\
> "Change": The point value of this move.\
> "Points": The sum of the point values of the previous moves.

> [!WARNING]
> Beware that sometimes the order of the displayed finishing moves in TerraFirmaGreg isn't easy to tell, when nothing happens even though you used the exact moves displayed, hover over the last moves in game and read what the tooltip says to be certain!\
> For example sometimes it says "Hit not last" which can change the order of the displayed moves!

## Known issues (so far they have been pretty rare!):
Currently None!

> [!NOTE]
> Please don't hesitate to open issues if you encounter one (Be sure to add relevant info)
