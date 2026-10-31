# Infinite Chest

Adds an **Infinite Chest** to the Hammer (Furniture tab). It uses the vanilla wood chest's model and recipe (10 Wood).

- Put any non-gear item in an empty Infinite Chest: every slot fills with full stacks of it.
- Take items out and the chest refills immediately.
- Put more of that item in and it is absorbed (deleted): handy for dumping surplus.
- Only that item can be put in afterwards. Gear (weapons, tools, armor, shields, torches, utility, trinkets) is never accepted; ammo is.
- Breaking the chest drops no items; you get the wood back.

## Configuration

In `BepInEx/config/BrugherheimFolks.InfiniteChest.cfg`, section `[Filter]`:

- `Mode`: `All` (default), `Whitelist` (only the listed items) or `Blacklist` (everything but the listed items).
- `Items`: comma separated item prefab names, as used by the `spawn` console command, e.g. `Wood,Stone,Resin`.

Gear is never accepted, whatever the filter says.

## Multiplayer

Install on every client **and** the server. Jötunn enforces that everyone has the same version.
The filter settings are server-enforced: the server's values are synced to every client and only admins can change them.

## Contributing

Source and build instructions: see [CONTRIBUTING.md](https://github.com/TonioGela/Infinite-Chest/blob/main/CONTRIBUTING.md).
