Hi retro game developers! 

Here you can find the **NES/SMS/PCE/ZX/SMD** development tools:


# MAPeD-NES/SMS/PCE/ZX/SMD - Game maps editor
The MAPeD is a tool for building a game levels from scratch.

**The main features are:**
- tiles drawing/composing tools
- building a game map using 2x2 or/and 4x4 tiles
- entities editor
- tile patterns manager
- data optimization tool
- several game maps in one project
- detachable UI
- tile properties editing ( can be used as collisions data etc )
- **NES:** palette per 1x1 tile support ( MMC5 )
- import of tiles and game maps from images*
- export to **NES: CA65/NESasm / SMS: WLA-DX / PCE: CA65/PCEAS/HuC / ZX: SjASMPlus / SMD: vasm/SGDK** with wide variety of options:
	- 2x2/4x4 tiles
	- column/row data order
	- RLE compression
	- modes: multidirectional / bidirectional scrolling, static screens switching
	- **NES:** attributes per 1x1/2x2 tile
	- tiles properties per 1x1/2x2 tile
	- level topology options
	- entities
	- etc...
- export to the **JSON** format
- export to the **SjASMPlus** (ZX-Spectrum)**
- built-in **Python** script editor for writing custom data export scripts
- easy data conversion***
- etc...

\* Smart import of images with checking of duplicate CHRs/blocks/tiles and flipped CHRs. Automatic palettes applying is supported for all platforms.

\** Each MAPeD can export data to the **SjASMPlus** with additional options.

\*** You can load any platform project into any MAPeD editor with automatic data conversion. So it's easy to adapt your graphics and data when creating a cross-platform project.

**Examples of tiles and maps images:** `./data/tiles-maps`

**Example projects:** `./data`

**Quick Guide:** `./doc/MAPeD/Quick_Guide.html`


# SPReD-NES/SMS/PCE - Sprites editor
The SPReD is a tool for converting prepared sprite images into a NES/SMS/PCE compatible format. Drawing mode is also available.

**Some features are:**

- handy drawing mode
- group operations on sprites
- sprites data packing and optimization
- **NES/SMS:** 8x16 mode support
- images import/export
- export to **NES: CA65/NESasm/CC65** / **SMS: WLA-DX/SDCC** / **PCE: CA65/PCEAS/HuC(PCX)**
- built-in **Python** script editor for writing custom data export scripts
- **NES <=> SMS** data conversion*
- etc...

\* You can load a **NES** project into **SMS** editor and vice versa.

**Example projects:** `./data`

**Quick Guide:** `./doc/SPReD/Quick_Guide.html`


# Python script editor
If you need to export specific data which are not supported by the tools, you can write your own script using a simple built-in Python script editor ( **SPSeD** ). Press **Alt+X** ( **ALT**ernative e**X**port ) to open the editor.

Data export APIs were designed to work with a current application data state. So you can retrieve data, but you can't modify it. Application data will always remain unchanged.

Press **Shift+F1** (in-app doc) or **F1** (in-browser doc) to open the API documentation. Or you can find it here:
- `./doc/MAPeD_Data_Export_Python_API.html`
- `./doc/SPReD_Data_Export_Python_API.html`

Sample scripts of using the APIs:
- `./scripts/MAPeD_project_stats.py`
- `./scripts/SPReD_project_stats.py`

**Note:** It's not recommended to use the buit-in editor for script editing under **Mono** due to buggy implementation of RichTextBox. You can load your script into the editor and into any other editor where you can edit it. Then press **Ctrl+R** (reload script) and **F5** (run script).


# Compiling (Win/Linux)
The solution files you can find here:
- `./src/MAPeD/`
- `./src/SPReD/`

To compile the tools on Windows you can use the Microsoft Visual Studio.

To compile/run the tools on Linux you need to install Mono on your computer. The latest stable release can be found here: https://www.mono-project.com/download/stable/ To run the tools you may need to configure your environment to allow it to automatically run .exe files through Mono, or manually run the tools in terminal: `mono <app.exe>`.

After loading the solution you need to reinstall all NuGet packages using the following command:

`Update-Package -reinstall`

After that you can compile the project.

The tools use the following NuGet packages:

**MAPeD:**
- `OpenTK -Version 3.1.0`
- `SkiaSharp.Views.WindowsForms -Version 2.88.1`
- `SkiaSharp.NativeAssets.Linux.NoDependencies -Version 2.88.1`

**SPSeD:**
- `IronPython -Version 2.7.9`


# Samples
## MAPeD-NES

 - `./samples/nes/tilemap_render/bidir_scroll_MMC1_dyn_mirr/`	- bidirectional scroller with dynamic mirroring and CHR bank switching 
 - `./samples/nes/tilemap_render/bidir_scroll_MMC1_vert_mirr/`	- bidirectional scroller with vertical mirroring
 - `./samples/nes/tilemap_render/multidir_scroll_MMC3_4scr_mirr/`	- multidirectional scroller with 4-screen mirroring
 - `./samples/nes/tilemap_render/multidir_scroll_MMC3_horiz_mirr/`	- the same as above, but with horizontal mirroring 
 - `./samples/nes/tilemap_render/multidir_scroll_MMC3_vert_mirr/`	- the same as above, but with vertical mirroring 
 - `./samples/nes/tilemap_render/multidir_scroll_MMC3_vert_mirr_sbar/`	- the same as above, but with MMC3 IRQ status bar 
 - `./samples/nes/tilemap_render/static_screens_MMC5/`	- static screens switching with MMC5 extended attributes support

## MAPeD-SMS

 - `./samples/sms/tilemap_render/multidir_scroll/`	- multidirectional scroller 
 - `./samples/sms/tilemap_render/bidir_scroll/`		- bidirectional scroller 
 - `./samples/sms/tilemap_render/bidir_scroll_ADJ_SCR_MASKS/`	- bidirectional scroller with a complex map topology
 - `./samples/sms/tilemap_render/bidir_stat_scr/`		- the same as above, but with static screens switching
 - `./samples/sms/tilemap_render/bidir_stat_scr_ADJ_SCR_MASKS/`	- the same as above, but with static screens switching
 - `./samples/sms/tilemap_render/stat_scr_VDP_data/`	- static screens switching using VDP-ready screens data

## MAPeD-PCE

 - `./samples/pce/tilemap_render/bidir_scroll_multimap/huc/`	- bidirectional scroller
 - `./samples/pce/tilemap_render/bidir_stat_scr_multimap/huc/`	- the same as above, but with static screens switching
 - `./samples/pce/tilemap_render/bidir_scroll_ADJ_SCR_MASKS_multimap/huc/`	- bidirectional scroller with a complex map topology
 - `./samples/pce/tilemap_render/multidir_scroll_multimap/huc/`	- multidirectional scroller
 - `./samples/pce/tilemap_render/multidir_stat_scr_multimap/huc/`	- screens switching with double-buffering for multidirectional maps
 - `./samples/pce/tilemap_render/stat_scr_VDC_data_multimap/huc/`	- static screens switching using VDC-ready data (raw BAT data)
 - `./samples/pce/tilemap_render/multidir_scroll_map_editor/huc/`	- dynamic map: simple map editor
 - `./samples/pce/tilemap_render/multidir_scroll_maze_generator/huc/`	- dynamic map: procedural generation of a random maze (3x3 screens)
 - `./samples/pce/game_prototype/huc/` - fully playable game prototype demo, run'n'jump platformer, 40 screens multidir scrollable map, 200+ entities +bonus level

* All samples are written in HuC (C-like language) and support all export options

## MAPeD-ZX

 - `./samples/zx/tilemap_render/multidir_scroll/` - multidirectional tilemap scroller with wide variety of settings (see 'settings.asm' for details); Keys: Q,A,O,P
 - `./samples/zx/tilemap_render/bidir_scroll/` - bidirectional scroller; Keys: Q,A,O,P
 - `./samples/zx/tilemap_render/bidir_stat_scr/` - static screens switching; Keys: Q,A,O,P

## MAPeD-SMD

coming soon...

## SPReD-NES

- `./samples/nes/player_cntrl_NROM-128`	- character controller: idle, running and jumping; 8x8/8x16 mode
- `./samples/nes/player_cntrl_MMC3`	- the same as previous, but using big meta-sprites with MMC3 1KB CHR bank switching 
( +shooting and ducking animations )

## SPReD-SMS

- `./samples/sms/show_sprite`		- simple show sprite program
- `./samples/sms/player_cntrl_dog`	- character controller: idle, running and jumping; 8x16 mode
- `./samples/sms/player_cntrl_marco`	- the same as previous, but using big meta-sprites 
( +shooting and ducking animations )
- `./samples/sms/diff_bpp_test`		- test program for testing the same image with different exported BPP values

## SPReD-PCE

- `./samples/pce/sprite_render/show_sprites/asm/`	- simple demo that shows test meta-sprites exported as asm data
- `./samples/pce/sprite_render/show_sprites/huc/`	- simple meta-sprites demo written in HuC
- `./samples/pce/sprite_render/show_sprites/huc2/`	- the same as previous, but with data exported as PCX images
- `./samples/pce/sprite_render/animation_test/asm/`	- simple animation demo with big meta-sprites in assembly
- `./samples/pce/sprite_render/animation_test/huc/`	- the same as previous, but written in HuC
- `./samples/pce/sprite_render/animation_test/huc2/`	- animation demo that shows 3 independent, dynamic meta-sprite sets with optional double-buffering
- `./samples/pce/sprite_render/animation_test/huc3/`	- meta-sprite instancing demo; each meta-sprite has its own unique behaviour, palette and share the same graphics data
- `./samples/pce/sprite_render/player_cntrl/asm/`	- simple character controller with a big meta-sprite character and dynamic sprite data: idle, move, kick; Controls: LEFT, RIGHT, UP
- `./samples/pce/sprite_render/player_cntrl/huc/`	- simple character controller written in HuC with a big meta-sprite character, dynamic sprite data and optional double-buffering; Controls: LEFT, RIGHT, UP

---
**Used tools:**

**NES: CA65 assembler (v2.17 - GIT 7445550)**
- Github: https://github.com/cc65/cc65
- The latest snapshots: https://sourceforge.net/projects/cc65/files/cc65-snapshot-win32.zip
- Installing instruction: http://wiki.nesdev.com/w/index.php/Installing_CC65

**SMS: WLA-DX assembler v9.10**
- Github: https://github.com/vhelin/wla-dx

**PCE: HuC - PC Engine C development toolkit (v4.00.4.gb95184f)**
- Github: https://github.com/jbrandwood/huc

**ZX Spectrum: SjASMPlus - Z80 Assembly Cross-Compiler (v1.18.3)**
- The latest version: https://github.com/z00m128/sjasmplus

To compile the samples on Windows you can run **'compile.bat'** which contains in all sample directories.

To compile them on Linux you can use **'makefile'** which also contains in all sample directories.

- `make` to compile binaries to the `./samples/<platform>/bin` directory;
- `make run` to run compiled sample with **FCEUX**/**Mednafen**/**Fuse** emulators;
- `make clean` to remove compiled binaries;

---
**Warning:** The sample codes coming with the tools were made just for testing of exported data. 
You can use/modify them for free at your own risk without any warranties.

---


# Third-party libraries

**MAPeD:**
+ **OpenTK** - Copyright (c) The Open Toolkit Team ( MIT License )
+ https://opentk.net/

- **SkiaSharp** - Copyright (c) Xamarin Inc., Microsoft Corporation ( MIT License )
- https://github.com/mono/SkiaSharp

**SPReD:**
- **PNGCS** - Copyright (c) Hernan J Gonzalez ( Apache License, Version 2.0 )
- https://github.com/leonbloy/pngcs

**SPSeD:**
- **IronPython** - Copyright (c) .NET Foundation and Contributors ( Apache License, Version 2.0 )
- https://ironpython.net

---
The tools sources: 
https://github.com/0x8BitDev/MAPeD-SPReD

Compiled executables and compiled samples you can get in the latest release:
https://github.com/0x8BitDev/MAPeD-SPReD/releases

The latest development build can be found **[here](https://drive.google.com/open?id=1Jopms8ENPrjTktrt_V36TQC2FZT2agId)**.

---
- **NES:** Nintendo Entertainment System / Nintendo
- **SMS:** Sega Master System / Sega Corp.
- **PCE:** PC Engine/TurboGrafx-16 / NEC
- **ZX:** ZX-Spectrum / Sinclair Research Ltd
- **SMD:** Sega Mega Drive/Genesis / Sega Corp.

Are trademarks of their respective owners and I am not affiliated with these companies in any way. Therefore, you can use this software at your own risk and without warranty of any kind.

---
Developed by 0x8BitDev Copyright � 2017-present

Released under the MIT license. See LICENSE.txt

mail: 0x8bitdev[at]gmail.com