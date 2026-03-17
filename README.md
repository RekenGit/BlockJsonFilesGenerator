# Block JSON Files Generator

WinForms app made for Resourcepack and Mod creators, to create json files much easier.

<img width="1282" height="657" alt="image" src="https://github.com/user-attachments/assets/2370fc31-3f3d-40c8-98a3-ad1621a685a8" />

## Block Settings

<img width="475" height="220" alt="image" src="https://github.com/user-attachments/assets/fbdb649b-663f-4dbc-9265-09beec3fc8ba" />

Fill block name input for your block that you will generate files for.

Variant Suffix is only for variants of your block.
For example if you want to have more block models, each with different texture, and when you place block in game the model will randomise.
`{variant}` will be replaced with variant number (starting from 0 to number you select).
Examples:
`test_block_broken0`.
`test_block_broken1`.
`test_block_broken2`.

Namespace for resourcepacks should stay as `minecraft`, if your making mods then put here your mod namespace, like `my_mod`.

Select your block type you want to generate files for, like `Slab`.

Compatible block types:
- Block
- Slab
- Stairs
- Wall
- Door
- Leaves
- Trapdoor
- Column
- Column 2
- Fence Gate
- Fence

Generate block set, select this option only if you wish to generate files for 2 variant of selected block. If not dont select it.
Generate block set will generate 2 blockstates files, one for a "normal" block with only one texture, and second for weighted model (with random variants)

Second block name should be used if you wish to generate files for any other type of block then just `Block` type.
This option tries to fix your block name to make sure your `Slab` block files wont over ride your `Block` files.
<img width="455" height="185" alt="image" src="https://github.com/user-attachments/assets/f3bab956-3c84-4528-ac00-b0fb8ceb2fe5" />
For example if this option is selected and block type is other then `Block`, then your file names should be changed to have "_slab" or "_stairs" names.


## Tekstury

You can select one of two options to select one texture for all faces or select each face texture individualy:
<img width="472" height="111" alt="image" src="https://github.com/user-attachments/assets/6707f2a1-df36-4cf7-a856-4d89541296fc" />

## Directory

You can chosse directory to your resourcepack, or any other folder you like.
If you want your textures previevs to be loaded in `Blockstate weights` tab, then make sure your `Block name`, `Namespace` are correct.
Also make sure your files in the directory are correct, they should be same as in any other resourcepacks.
Select the `assets` directory with those files inside:
<br>[`assets`]
<br>  ∟[`namespace`]
<br>    ∟[`textures`]
<br>      ∟[`block`]

## Output preview

1. Set texture weights for generated blocks and see the percentage of their appearance change while you do
<img width="776" height="495" alt="image" src="https://github.com/user-attachments/assets/49691196-c17e-4d49-9439-ecf9a901b607" />

2. See the preview of filles that will be created
<img width="449" height="336" alt="image" src="https://github.com/user-attachments/assets/4df8c032-e148-4b0f-b449-f48b971712d0" />

## Final result

You can easly create just some texture variants, put them in correct directry, then run this app and create blocks that will use those textures at ease.
<img width="1860" height="1010" alt="image" src="https://github.com/user-attachments/assets/633091df-cfad-46a9-b278-a9c8e18dfc2f" />
