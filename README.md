# Unity-Prefab-Placement-Editor

![ScreenShot](https://i.imgur.com/1NWvEeg.png)

## Install:

Import everything to your Assets folder, PrefabPlacementEditor must be inside Editor folder.
Create a empty game object and put the component PlacementSystem script.

## Prefab configuration:

All prefabs that you want to paint need to have separated Layer and Tag in order to make the tool work properly. For example, set the oject tag to "Grass" and the layer to "PrefabPlacement". You can use the tag to separete trees, grass, rocks, bushes and so on. That's the way I found to avoid ambiguities when using the erase option.

## Usage:

Select the game object with PlacementSystem to enter in paint mode. A blue circle will follow you mouse through the ground surface(the ground also need to have a collider).

**Prefab** is a array of game objects that you want to paint, you can paint multiples at once.

**Layer Mask** is the layer of the surface that you want to fill, no needed to create one, just let as default.

**Prefab tag** is the tag of the objects that you want to erase(hold left shift).

**Randomize prefab** let you paint multiples objects in certain array range. If it's off, you can choose by array index.

**Override** let you put one prefab over another, doesn't works very well. Turn it off.

**Spread, radius and amount** are the fill options. 

**Aling with normal** let your prefab follow the ground rotation. It's useful if you gonna paint in planet surface.

**yOffset** is useful to avoid misplacing, very rare glitch. 2 is a fine value.

**Randomize size and rotation**, options to avoid bad tiling. Useful when you gonna paint some trees.

**Hide in Hierarchy** is just to avoid hundreds of prefabs showing in your scene hierarchy.

**Left mouse button to paint, shift + left mouse button to erase.**

## Licence:

**Do not sell or distribute as if it were your own.**



