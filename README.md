# TerraFirmaGregSmithingCalc

A simple, overengineered, console tool to calculate which smithing moves you have to do.\
Best used with the Texturepack that gives you the ability to read the required pointnumber for your desired end product.\
It's [found here](https://www.curseforge.com/minecraft/texture-packs/tfc-anvil-helper), outdated but still works for me!

## How to use:
The application always assumes you start at 0 and want to reach a point target (if you aren't starting from 0 just input the difference between your current points and your target points.\
To use simply input the goal sum of points, then select the moves from last to third last (from left to right as displayed in the game) and hit E to calculate.\
If it doesn't immediately find a solution it might take a few seconds because it's brute forcing all combinations to find the shortest/best one.

> [!WARNING]
> Beware that sometimes the order of the displayed finishing moves isn't correct, when nothing happens even though you did everything right, hover over them and read what the tooltip says to be certain!

## Known issues (so far they have been pretty rare!):
I have had some edge cases where the points go negative(destroying the item you're working on), if you find one such case please tell me the inputs! (Target points and finishing moves)\
If you find a combination that ends up with the wrong finishing moves or too high/too low points in the end please open an issue with the relevant info and I'll look into it!
