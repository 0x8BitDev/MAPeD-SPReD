Hi retro game developers! 

Here you can find NES development tools:


### MAPeD(NES) - Game maps editor
---
**The main features are:**
- tiles drawing\composing tools
- building a game map using 2x2 or\and 4x4 tiles
- data optimization tool
- several game maps in one project
- detachable UI
- entities editor
- tile properties editing ( can be used as collisions data etc )
- palette per 1x1 tile support ( MMC5 )
- import tiles from images
- export to CA65\NESasm with wide variety of options:
	- 2x2\4x4 tiles
	- column\row data order
	- RLE compression
	- modes: multidirectional \ bidirectional scrolling, static screens switching
	- attributes per 1x1\2x2 tile
	- tiles properties per 1x1\2x2 tile
	- level topology options
	- entities
	- etc...
- export to SjASMPlus ( ZX Spectrum assembler )
- etc...


**SharpDevelop solution:**
 `.\src\MAPeD\MAPeD(NES).sln`

**NES assembly sources of tilemap renderers:**

 `.\samples\nes\tilemap_render\bidir_scroll_MMC1\`	- bidirectional scroller with dynamic mirroring and CHR bank switching
 `.\samples\nes\tilemap_render\multidir_scroll_MMC3\`	- multidirectional scroller
 `.\samples\nes\tilemap_render\static_screens_MMC5\`	- static screens switching with MMC5 extended attributes support

**ZX Spectrum sample sources:**
 `.\samples\zx\tilemap_render\`


### SPReD(NES) - Sprites editor
---
**Some features are:**

- handy drawing mode
- group operations on sprites
- sprites data packing ( 1/2/4 KB ) and optimization
- 8x16 mode support
- images import\export
- export to CA65\NESasm
- etc...


**SharpDevelop solution:**
 `.\src\SPReD\SPReD(NES).sln`

**NES assembly sources of simple character controllers:**

`.\samples\nes\player_cntrl_NROM-128`	- character controller: idle, running and jumping; 8x8\8x16 mode
`.\samples\nes\player_cntrl_MMC3`	- the same as above, but using big sprites with MMC3 1KB CHR bank switching 
( +shooting and ducking animations )


### Third-party libraries:
---
The SPReD(NES) uses the third-party library `.\src\SPReD\lib\Pngcs.dll` to read\write PNG images.
Developed by Hernan J Gonzalez Copyright 2012 ( Apache License, Version 2.0 )

https://github.com/leonbloy/pngcs

---
**WARNING:** The sample codes coming with the tools were made just for testing of exported data. 
You can use\modify them for free at your own risk without any warranties.

---
The tools sources: 
https://github.com/0x8BitDev/MAPeD-SPReD

Compiled executables, example projects files and compiled NES/ZX samples you can get in the latest release: 
https://github.com/0x8BitDev/MAPeD-SPReD/releases

The latest development build can be found **[here](https://drive.google.com/open?id=1Jopms8ENPrjTktrt_V36TQC2FZT2agId)**.

The tools were developed using **SharpDevelop 4.4.2.** 
You can download it at: http://www.icsharpcode.net/OpenSource/SD/Download/Default.aspx

The NES samples were created using **CA65 assembler (v2.17 - GIT 7445550)**. 
You can download the latest CC65 snapshots at: https://sourceforge.net/projects/cc65/files/cc65-snapshot-win32.zip/download

The ZX Spectrum sample uses **SjASMPlus - Z80 Assembly Cross-Compiler (v1.07 RC7)**. 
The latest version can be downloaded at: https://sourceforge.net/projects/sjasmplus/

---
Developed by 0x8BitDev Copyright © 2017-2019

Released under the MIT license. See LICENSE.txt

mail: 0x8bitdev[at]gmail.com
