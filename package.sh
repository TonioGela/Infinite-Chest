#!/usr/bin/env bash
# Builds the mod and writes build/BrugherheimFolks-Infinite_Chest-<version>.zip (Thunderstore upload or r2modman "Import local mod").
set -euo pipefail
cd "$(dirname "$0")"
command -v dotnet >/dev/null || exec nix-shell -p dotnet-sdk_8 --run ./package.sh
export DOTNET_ROLL_FORWARD=Major # tcli targets net6
dotnet build -c Release -nologo
dotnet tool restore
dotnet tcli build
