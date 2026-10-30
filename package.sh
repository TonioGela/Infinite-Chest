#!/usr/bin/env bash
# Builds the mod and writes build/BrugherheimFolks-Infinite_Chest-<version>.zip (Thunderstore upload or r2modman "Import local mod").
# The version comes from $VERSION (CI sets it to the tag); otherwise it's the latest tag with the minor bumped, as a snapshot.
set -euo pipefail
cd "$(dirname "$0")"
command -v dotnet >/dev/null || exec nix-shell -p dotnet-sdk_8 --run ./package.sh
if [ -n "${VERSION:-}" ]; then
  v="${VERSION#v}" snapshot=""
else
  last=$(git describe --tags --abbrev=0 2>/dev/null || echo v0.0.0)
  IFS=. read -r major minor _ <<< "${last#v}"
  v="$major.$((minor + 1)).0" snapshot="-SNAPSHOT"
fi
# tcli and BepInEx's [BepInPlugin] only accept plain x.y.z, so -SNAPSHOT only goes in the DLL's informational version and the zip name
export DOTNET_ROLL_FORWARD=Major # tcli targets net6
dotnet build -c Release -nologo -p:Version="$v" -p:InformationalVersion="$v$snapshot"
dotnet tool restore
dotnet tcli build --package-version "$v"
[ -z "$snapshot" ] || mv "build/BrugherheimFolks-Infinite_Chest-$v.zip" "build/BrugherheimFolks-Infinite_Chest-$v$snapshot.zip"
