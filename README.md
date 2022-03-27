<img src="./game/icon.png" width="128" align="right" alt="Package Resolved icon"/>

# 📦 Package Resolved

It's another day at the Swifty Package Factory, and you have a long list of orders to
deliver, with no time to spare! Can you collect all of the packages to deliver in the
time allotted? Watch out for palettes and wet floors, or you'll get really injured!

**Package Resolved** is a short game written in Godot, originally submitted for the
132nd Trijam game jam.

> Note: This version of the game provides enhancements and fixes that the original
> game did not have. Go to [alicerunsonfedora/trijam-132][gamejam-version] to view
> the source code for the original game jam version.

[gamejam-version]: https://github.com/alicerunsonfedora/trijam-132

## 🕹 How to play

Use the arrow keys or the A and D keys to move left and right; for touchscreen
devices, tap and hold on the left and right edges. Run into packages to collect them,
and avoid running into palettes. You must collect all of the required packages (seen
in top left) before time runs out (seen top right). You can choose between the
regular arcade game with levels, or you can play the endless mode and see how far you
can get.

### Powerups

- Packages with a red border act as the equivalent of collecting two packages.
- Pocket watches with the plus sign add time to your timer.

### Hazards

- Water puddles marked with wet floor signs cause you to speed up.
- Palettes marked with wet floor signs cause an instant game over on contact.

## 🛠 Build from source

### Developer Tools

For this project, you will need the following tools installed:

- Godot 3.3 or later
- .NET SDK 6.0 or later

The following tools are not required to build the game, but are useful for certain
variants or other source purposes:

- Xcode 12 or later, for signing certificates
- iconutil, for creating the Mac icon file
- Aseprite, for making the sprite files
- clickable, for making the Ubuntu Touch variant
- snapcraft, for making the Snapcraft variant
- Salmon font family, for achieving the font look\*

> \*The game will render just fine without these fonts and use the fallbacks present
> in the original game. 

### Additional Setup on Apple Silicon Macs

At the time of writing this documentation, Apple Silicon Macs do not support the Mono
framework from Homebrew. To ensure the project builds correctly on Apple Silicon
Macs, the following changes need to be made:

- In the **Editor Settings > Mono > Build Tools**, change the build tool to dotnet
  CLI.
- Depending on the IDE you are using to write C# scripts, you may need to run
  additional configuration changes.

### Using the Salmon font family

Package Resolved uses the Salmon font family but can use the original Inter and
JetBrains Mono fonts from the original game jam version.

If you have a copy of the Salmon font family that you have purchased and want to
use it in this game, place the following fonts into their respective paths:

- **Salmon 9 Sans Regular**: `game/assets/fonts/s_regular.ttf`
- **Salmon 9 Sans Bold**: `game/assets/fonts/s_bold.ttf`
- **Salmon 9 Mono Regular**: `game/assets/fonts/s_mono.ttf`

### Export the project

Clone the repository code from GitHub via `git clone`, then open the project in
Godot. To export the projects, go to **Project > Export**
and then create the export settings for the platforms you want to target.

> Note: You will need to make sure the export configurations also export JSON files
> in the "Features" tab to ensure the dialogue appears correctly.

Additional build information such as building for Ubuntu Touch and/or a Linux Snap
package can be found in the [BUILD.md](BUILD.md) file of this project.

## 💬 Bug reporting and feature requests

If you find any bugs in the game or want to request a feature to make it better,
please consider filing a report on the project's issue tracker at
https://youtrack.marquiskurt.net/youtrack/newIssue?project=SPR.

## 📃 License

Package Resolved is free and open-source software, licensed under the Mozilla Public
License, v2.0. You can read your usage and distribution rights in the LICENSE file,
or at https://mozilla.org/MPL/2.0/.

## 📑 Credits

- Swift bird costume sprite based off of character template created by LimeZu.
- Main menu artwork created by Raseruuu.
- Music and SFX created with JSFXr and BeepBox.
- Inter font created by Rasmus Andersson.
- JetBrains Mono font created by JetBrains s.r.o.

This project is neither affiliated with nor endorsed by Apple Inc. Swift (programming
language) is a trademark of Apple Inc.
