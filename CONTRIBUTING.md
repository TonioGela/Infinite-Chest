# Contributing

## Building

Requires the .NET 8 SDK and `zip` (on Nix, `package.sh` enters a `nix-shell` with both automatically), Valheim, and BepInEx + [Jötunn](https://thunderstore.io/c/valheim/p/ValheimModding/Jotunn/) installed.

```sh
./package.sh
```

This compiles the mod, copies the DLL into `BepInEx/plugins/InfiniteChest-dev/` so you can launch the game straight away, and writes to `build/`:

- `thunderstore/BrugherheimFolks-Infinite_Chest-<version>.zip`: Thunderstore upload, or r2modman "Import local mod".
- `nexus/InfiniteChest-<version>.zip`: Nexus Mods upload, `BepInEx/plugins/...` to extract into the game folder.

By default the build looks for Valheim in Steam's standard location and for BepInEx inside the game folder. To use other paths, e.g. an r2modman profile, create a `local.props` (gitignored) next to the project:

```xml
<Project>
  <PropertyGroup>
    <BepInExDir>$(HOME)/.config/r2modmanPlus-local/Valheim/profiles/YOUR_PROFILE/BepInEx</BepInExDir>
    <!-- <ValheimDir>/path/to/Valheim</ValheimDir> -->
  </PropertyGroup>
</Project>
```

Delete `BepInEx/plugins/InfiniteChest-dev/` before playing with the released version, so the two don't both load.

## Versions and releases

Local and branch builds are versioned as the latest git tag with the minor bumped, e.g. `v1.0.1` → `1.1.0-SNAPSHOT`. Pushing a tag like `v1.2.0` makes CI build that exact version and publish it to Thunderstore and Nexus Mods.
