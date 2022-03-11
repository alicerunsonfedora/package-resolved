# Additional Build Instructions

This document will cover additional instructions and tips to build the game for
various platforms not typically covered by the Godot export template system.

### Ubuntu Touch click packages

> :warning: The following instructions have _not_ been tested with the new codebase
> written in C#. Additionally, you may need to run additional scripts to ensure you
> have a version of Godot with the Mono framework built in for the Ubuntu Touch
> platform.

Start by building the project as you would for standard, x86-64 Linux\*. Copy the
resulting .pck file from the `dist/linux` directory (or whereever you specified the
Linux export) into the `clickable` directory and rename it to `package-resolved.pck`.
In the terminal, run the following:

```
$ cd clickable
$ clickable build
```

The resulting click file should be present in the `build` directory inside of
`clickable`, which can be installed on an Ubuntu Touch device by copying the file
over.

> \*You may also use the "Export PCK/ZIP" option instead of "Export Project", as the
> binary file(s) is/are not necessary for building the click package. Clickable will
> download a custom Godot binary.

#### Multiple architectures

Note: To build for different architectures, pass in the `CLICKABLE_ARCH `environment
variable.

For example, to build for armhf and arm64:

```
$ CLICKABLE_ARCH=armhf clickable build
$ CLICKABLE_ARCH=arm64 clickable build
```

> Note: It is recommended that you provide packages for at least the `armhf` and
> `arm64` architectures.

### Snap packages

After exporting the project, create a ZIP file of the exported `linux` directory and
name it `package-resolved_linux.zip`. Move the ZIP file to the root of the project
and run `snapcraft` to make a snap.
