> [!NOTE]
> This is the `.github/README.md` file. We choose to leave it outside of the root directory. You can move it to the root directory or edit it in `.github/`.

# template-mod

This is the Team Catalyst Template Mod, a template repository for quick-starting development with [terraria-catalyst/catalyst](https://github.com/terraria-catalyst/catalyst).

Catalyst is a build toolchain and extension to the tModLoader ecosystem that facilitates monolithic/multi-project repositories that may contain multiple projects, including multiple mods.

## About Catalyst

Catalyst is a set of projects and MSBuild targets/props that make mod development quicker and easier. It provides features like access transformers, allows for non-standard project organization, features source generators, and more.

This template is specifically designed to integrate with it, so it isn't optional.

## Cloning

The repository should be cloned into the `tModLoader/ModSources/` directory, such that the path looks like: `tModLoader/ModSources/template-mod`. This requirement may change in the future as Catalyst develops.

## Building

Once cloned, one should run the `scripts/setup.sh` script. This script generates symlinks, initializes and updates submodules, and builds Catalyst.

After running the setup script, you can build any mod project with `dotnet build` and it will produce binaries in the `bin/` directory and a built `.tmod` archive in `tModLoader/Mods`, just like regular tModLoader.

## Symbolic Linking

As mentioned in [Building](#building), symbolic links are used. These are used to mimic the expected tModLoader mod directory setup. Your cloned repository is included in `ModSources/` but isn't actually a mod. Normally, any mods within `src/` would go unrecognized. To fix this, symbolic links are generated that lie within `ModSources/` and point to the projects in `src/`. The allows for tML to detect your mods, reference assets like icons for publishing, and properly update and hot reload localization files.

Generating symbolic links may require administrator/root permissions.

## Generating Projects

You can generate projects with the `gen-mod.js` and `gen-library.js` scripts, which will create a new mod or library project respectively.

When generating a mod, you should pass in both the namespace and internal mod name (assembly name) as arguments. When generating a library, you should just pass in the namespace (assembly name).

To generate the solution file, run `gen-sln.js` with the name as input. **Generating mods and libraries will not automatically add them to the solution.**
