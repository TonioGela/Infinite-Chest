#!/usr/bin/env bash
# Builds the mod and writes InfiniteChest-<version>.zip, ready for Thunderstore upload or r2modman "Import local mod".
set -euo pipefail
cd "$(dirname "$0")"
nix-shell -p dotnet-sdk_8 --run "dotnet build -c Release -nologo"
v=$(grep -oP '"version_number": "\K[^"]+' manifest.json)
rm -f "InfiniteChest-$v.zip"
nix-shell -p zip --run "zip -j InfiniteChest-$v.zip manifest.json README.md icon.png bin/Release/netstandard2.1/InfiniteChest.dll"
