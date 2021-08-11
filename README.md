# Aeris
Aeris FF7 Background Tool is a tool thought for work with Final Fantasy VII (FF7) Backgrounds, old version (1998/Steam/SquareEnix Store)

I decided to do this tool at the beginning to make available the upscaling of the hashes that outputs FFNx (https://github.com/julianxhokaxhiu/FFNx) driver for "emulate" the animated textures that the orignal game has.

But as I advanced in doing things, I saw that the game has more things that need to be fixed, or even it could be possible to use the tool to arrange unusued fields from the Final Fantasy VII international version.

To use this tool, you need to have the field extracted from the big file called flevel.lgp from the old Final Fantasy VII. You have this file in PC'98/Steam/SquareEnix versions.
You can do that by means of using a tool called Makou Reactor (https://github.com/myst6re/makoureactor).

Once you have the field extracted (you can import again with the previous mentioned tool), you can:

- Swizzle* or Unswizzle* the base images/textures of a field (Backgrounds). You can Swizzle upscaled external base image files.
- Swizzle or Unswizzle the hashed images/textures of a field dumped with FFNx (Animated Textures). You can Swizzle upscaled external hashed image files.
- Modify Data/Image of a given tile.
- See the scene of the field and check the diferent layers activating or deactivating them, or even the sublayers.
- Import new textures, for direct images or palettized (the colors of the palete MUST coincide).
- Export all the base textures of a field directly.
- Export/Import the palette/s of the field.
- Save the field with the changes in uncompressed format.
- You can render the effects of the field (lights, doors opening/closing, etc...).
- You can mark on the stage the tile selected in the texture panel.

Well, I think that this is enough for working with backgrounds. Maybe there could be added some more things like open compressed fields (and save them compressed also), or even open the fields directly from flevel.lgp.

I hope you enjoy this tool. ;)
