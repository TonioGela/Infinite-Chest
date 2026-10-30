using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;

namespace InfiniteChest
{
    [BepInPlugin("BrugherheimFolks.InfiniteChest", "Infinite Chest", "1.0.1")]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class Plugin : BaseUnityPlugin
    {
        public const string ChestName = "Infinite Chest";
        const string ItemKey = "InfiniteChest_item";
        static bool refilling;

        void Awake()
        {
            PrefabManager.OnVanillaPrefabsAvailable += AddPiece;
            new Harmony("BrugherheimFolks.InfiniteChest").PatchAll();
        }

        static void AddPiece()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= AddPiece;
            // Cloning keeps the vanilla chest's model and its requirements (10 Wood)
            var piece = new CustomPiece("piece_chest_infinite", "piece_chest_wood", "Hammer");
            piece.Piece.m_name = ChestName;
            piece.Piece.m_description = "Put one item in: it is filled with that item forever.";
            piece.PiecePrefab.GetComponent<Container>().m_name = ChestName;
            PieceManager.Instance.AddPiece(piece);
        }

        // Runs on the ZDO owner (whoever has the chest open), so the refill syncs like any normal chest edit.
        [HarmonyPatch(typeof(Container), "OnContainerChanged")]
        static class Refill
        {
            static void Postfix(Container __instance, ZNetView ___m_nview, bool ___m_loading)
            {
                if (refilling || ___m_loading || __instance.m_name != ChestName || !__instance.IsOwner()) return;
                var inv = __instance.GetInventory();
                var zdo = ___m_nview.GetZDO();
                var itemName = zdo.GetString(ItemKey);
                if (itemName == "")
                {
                    var first = inv.GetAllItems().Find(i => i.m_dropPrefab);
                    if (first == null) return;
                    itemName = first.m_dropPrefab.name;
                    zdo.Set(ItemKey, itemName);
                }
                var prefab = ObjectDB.instance.GetItemPrefab(itemName);
                if (!prefab) return;
                var max = prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_maxStackSize;
                refilling = true;
                try
                {
                    // Each AddItem tops up partial stacks, then takes an empty slot; Inventory.Changed -> Save per call.
                    while (inv.CanAddItem(prefab, 1)) inv.AddItem(prefab, max);
                }
                finally { refilling = false; }
            }
        }

        // Every way of putting items in (drag, shift-click, stack all, our refill) ends in one of these overloads.
        [HarmonyPatch]
        static class OnlyChosenItem
        {
            static IEnumerable<MethodBase> TargetMethods() => AccessTools.GetDeclaredMethods(typeof(Inventory))
                .Where(m => m.Name == "AddItem" && m.GetParameters()[0].ParameterType == typeof(ItemDrop.ItemData));

            static bool Prefix(Inventory __instance, ItemDrop.ItemData __0, ref bool __result) => __result = Allowed(__instance, __0);
        }

        // Dropping onto an occupied slot swaps items, removing them from their source *before* adding them to the
        // destination, so a rejected AddItem there deletes the item. Reject the whole swap up front instead.
        [HarmonyPatch(typeof(InventoryGrid), nameof(InventoryGrid.DropItem))]
        static class NoLossySwap
        {
            static bool Prefix(InventoryGrid __instance, Inventory fromInventory, ItemDrop.ItemData item, Vector2i pos, ref bool __result)
            {
                var target = __instance.GetInventory();
                var itemAt = target.GetItemAt(pos.x, pos.y);
                return __result = Allowed(target, item) && (itemAt == null || itemAt == item || Allowed(fromInventory, itemAt));
            }
        }

        static readonly AccessTools.FieldRef<Inventory, string> InventoryName = AccessTools.FieldRefAccess<Inventory, string>("m_name");

        static bool Allowed(Inventory inv, ItemDrop.ItemData item)
        {
            if (InventoryName(inv) != ChestName) return true;
            var items = inv.GetAllItems();
            var gear = item.IsEquipable() && item.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Ammo;
            return !gear && (items.Count == 0 || items[0].m_shared.m_name == item.m_shared.m_name);
        }

        // Breaking the chest would otherwise spill a full chest of items on the ground.
        [HarmonyPatch(typeof(Container), "OnDestroyed")]
        static class NoDrop
        {
            static bool Prefix(Container __instance) => __instance.m_name != ChestName;
        }
    }
}
