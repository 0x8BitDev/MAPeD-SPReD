Hi retro game developers! 

Here you can find **NES/SMS** development tools:


# MAPeD-NES - Game maps editor
**The main features are:**
- tiles drawing/composing tools
- building a game map using 2x2 or/and 4x4 tiles
- data optimization tool
- several game maps in one project
- detachable UI
- entities editor
- tile properties editing ( can be used as collisions data etc )
- palette per 1x1 tile support ( MMC5 )
- import tiles from images
- export to **CA65/NESasm** with wide variety of options:
	- 2x2/4x4 tiles
	- column/row data order
	- RLE compression
	- modes: multidirectional / bidirectional scrolling, static screens switching
	- attributes per 1x1/2x2 tile
	- tiles properties per 1x1/2x2 tile
	- level topology options
	- entities
	- etc...
- built-in **Python** script editor for writing custom data export scripts
- export to **SjASMPlus** ( ZX Spectrum assembler )
- etc...

**Quick Guide:** `./doc/MAPeD/Quick_Guide.html`

# SPReD-NES/SMS - Sprites editor
**Some features are:**

- handy drawing mode
- group operations on sprites
- sprites data packing ( 1/2/4 KB ) and optimization
- 8x16 mode support
- images import/export
- export to **NES: CA65/NESasm** / **SMS: WLA-DX**
- built-in **Python** script editor for writing custom data export scripts
- **NES <-> SMS** data conversion*
- etc...

\* You can load a **NES** project into **SMS** editor and vice versa.

**Quick Guide:** `./doc/SPReD/Quick_Guide.html`

# Python script editor
If you need to export specific data which are not supported by the tools, you can write your own script using simple built-in Python script editor ( **SPSeD** ). Press **Alt+X** ( **ALT**ernative e**X**port ) to open the editor.

Data export APIs were designed to work with a current application data state. So you can retrieve data, but you can't modify it. Application data will always remain unaltered.

Press **Shift+F1** (in-app doc) or **F1** (in-browser doc) to open an API documentation. Or you can find it here:
`./doc/MAPeD_Data_Export_Python_API.html`
`./doc/SPReD_Data_Export_Python_API.html`

Sample scripts of using the APIs:
`./scripts/MAPeD_project_stats.py`
`./scripts/SPReD_project_stats.py`

**Warning:** It's not recommended to use the editor for script editing under **Mono** due to buggy implementation of RichTextBox. You can load your script into the editor and to any other editor where you can edit it. Then you can press **Ctrl+R** (reload script) and **F5** (run script).


# Compiling
The solution files you can find here:
 `./src/MAPeD/MAPeD-NES.sln`
 `./src/SPReD/SPReD-NES.sln`
 `./src/SPReD/SPReD-SMS.sln`

**Note:** **.Net 4.5.1** profile is required to build the tools.

## Windows
The tools were developed using **SharpDevelop 4.4.2.** 
You can download it at: http://www.icsharpcode.net/OpenSource/SD/Download/Default.aspx

1) Run SharpDevelop.
2) Open solution file.
3) Menu: `Build -> Build Solution`.
4) `./bin` will contains output executable file.

## Linux
The tools were tested on **Ubuntu 16.4.6** and **Debian 9.9.0** with **Mono v5.20.1.19**.

To compile/run the applications on Linux you need to install Mono on your computer. The latest stable release can be found here: https://www.mono-project.com/download/stable/

The Linux versions were tested using **MonoDevelop 7.8.2 (build 2)**. The latest stable release can be found here: https://www.monodevelop.com/download/

1) Run MonoDevelop.
2) Open solution file.
3) Menu: `Build -> Build All`.
4) `./bin` will contains output executable file.

**Warning:** It's not necessary to rebuild the tools on Linux if you don't want to make any changes in sources. You can use the same executables files on Windows and Linux ( with Mono ).


# Releases
## Windows
**.Net 4.5.1** is required to run the applications.
https://www.microsoft.com/en-us/download/details.aspx?id=40779

## Linux
As mentioned before, you need to install Mono on your computer. To run the tools you may need to configure your environment to allow it to automatically run .exe files through Mono, or manually run the tools in terminal: `mono MAPeD-NES.exe`, `mono SPReD-NES.exe`, `mono SPReD-SMS.exe`.


# Samples
## MAPeD-NES
**NES assembly sources of tilemap renderers:**

 `./samples/nes/tilemap_render/bidir_scroll_MMC1/`	- bidirectional scroller with dynamic mirroring and CHR bank switching
 `./samples/nes/tilemap_render/multidir_scroll_MMC3/`	- multidirectional scroller
 `./samples/nes/tilemap_render/static_screens_MMC5/`	- static screens switching with MMC5 extended attributes support

**ZX Spectrum sample sources ( can be compiled on Windows only ):**
 `./samples/zx/tilemap_render/`


## SPReD-NES
**NES assembly sources of simple character controllers:**

`./samples/nes/player_cntrl_NROM-128`	- character controller: idle, running and jumping; 8x8/8x16 mode
`./samples/nes/player_cntrl_MMC3`	- the same as above, but using big sprites with MMC3 1KB CHR bank switching 
( +shooting and ducking animations )

## SPReD-SMS
**Coming soon...**


---
The NES samples were created using **CA65 assembler (v2.17 - GIT 7445550)**. 
Github page: https://github.com/cc65/cc65 Or you can download the latest CC65 snapshots at: https://sourceforge.net/projects/cc65/files/cc65-snapshot-win32.zip

**CC65** installing instruction: http://wiki.nesdev.com/w/index.php/Installing_CC65

The ZX Spectrum sample uses **SjASMPlus - Z80 Assembly Cross-Compiler (v1.07 RC7)**. 
The latest version can be downloaded at: https://sourceforge.net/projects/sjasmplus/

To compile the NES samples on Windows you can run **'compile.bat'** which contains in all samples directories.

To compile them on Linux you can run **'makefile'** which also contains in all samples directories.

- `make` to compile binaries to the `./samples/nes/bin` directory;
- `make run` to run compiled sample with **FCEUX** emulator;
- `make clean` to remove compiled binaries form the `./samples/nes/bin` directory;

---
**Warning:** The sample codes coming with the tools were made just for testing of exported data. 
You can use/modify them for free at your own risk without any warranties.

---


# Third-party libraries

The MAPeD/SPReD use IronPython for custom data export scripts. Copyright (c) .NET Foundation and Contributors ( Apache License, Version 2.0 )

https://ironpython.net

The SPReD uses the third-party library `./src/SPReD/lib/Pngcs.dll` to read PNG images.
Developed by Hernan J Gonzalez Copyright 2012 ( Apache License, Version 2.0 )

https://github.com/leonbloy/pngcs

---
The tools sources: 
https://github.com/0x8BitDev/MAPeD-SPReD

Compiled executables, example projects files and compiled NES/ZX samples you can get in the latest release: 
https://github.com/0x8BitDev/MAPeD-SPReD/releases

The latest development build can be found **[here](https://drive.google.com/open?id=1Jopms8ENPrjTktrt_V36TQC2FZT2agId)**.

---
Developed by 0x8BitDev Copyright © 2017-2019

Released under the MIT license. See LICENSE.txt

mail: 0x8bitdev[at]gmail.com
