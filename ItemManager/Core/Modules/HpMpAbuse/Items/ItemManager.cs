﻿namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus;

    using Utils;

    internal class ItemManager
    {
        public static MenuManager Menu;

        public static MultiSleeper Sleeper = new MultiSleeper();

        private static Hero Hero;

        public readonly List<UsableItem> UsableItems = new List<UsableItem>();

        private readonly List<uint> droppedItems = new List<uint>();

        private readonly Dictionary<ItemSlot, Item> itemSlots = new Dictionary<ItemSlot, Item>();

        public ItemManager(MenuManager menuManager)
        {
            Menu = menuManager;
            Hero = ObjectManager.LocalHero;

            PowerTreads = new PowerTreads("item_power_treads");
            TranquilBoots = new TranquilBoots("item_tranquil_boots");

            UsableItems.Add(new Mekansm("item_mekansm"));
            UsableItems.Add(new ArcaneBoots("item_arcane_boots"));
            UsableItems.Add(new GuardianGreaves("item_guardian_greaves"));
            UsableItems.Add(SoulRing = new SoulRing("item_soul_ring"));
            UsableItems.Add(Bottle = new Bottle("item_bottle"));
            UsableItems.Add(StashBottle = new StashBottle("item_bottle"));
            UsableItems.Add(new MagicStick("item_magic_stick"));
            UsableItems.Add(new UrnOfShadows("item_urn_of_shadows"));
            UsableItems.Add(new Shrine("filler_ability"));

            Game.OnIngameUpdate += OnUpdate;
        }

        public Bottle Bottle { get; set; }

        public PowerTreads PowerTreads { get; }

        public SoulRing SoulRing { get; }

        public StashBottle StashBottle { get; }

        public ItemSlot StashBottleSlot { get; private set; }

        public bool StashBottleTaken { get; private set; }

        public TranquilBoots TranquilBoots { get; }

        public void DropItem(Item item, bool queue = true, bool forceDrop = false)
        {
            SaveItemSlot(item);

            if (Menu.RecoveryMenu.ItemsToBackpack && Hero.ActiveShop == ShopType.None && !forceDrop)
            {
                if (Sleeper.Sleeping("disabled" + item.Handle))
                {
                    return;
                }

                item.MoveItem(ItemSlot.BackPack_1);
                var itemSlot = itemSlots.First(x => x.Value.Equals(item)).Key;
                item.MoveItem(itemSlot);
                itemSlots.Remove(itemSlot);
                Sleeper.Sleep(6000, "disabled" + item.Handle);
                return;
            }

            droppedItems.Add(item.Handle);
            Hero.DropItem(item, Hero.Position, queue);
        }

        public void DropItems(ItemUtils.Stats dropStats, Item ignoredItem = null)
        {
            if (dropStats == ItemUtils.Stats.None)
            {
                return;
            }

            Hero.Inventory.Items
                .Where(x => !x.Equals(ignoredItem) && x.IsDroppable && ItemUtils.GetStats(x).HasFlag(dropStats))
                .ForEach(x => DropItem(x));
        }

        public int DroppedItemsCount()
        {
            return droppedItems.Count;
        }

        public void OnClose()
        {
            Game.OnIngameUpdate -= OnUpdate;
            UsableItems.Clear();
        }

        public void PickUpItems(IEnumerable<AbilityId> itemIds)
        {
            foreach (var item in ObjectManager.GetEntitiesParallel<PhysicalItem>()
                .Where(x => itemIds.Contains(x.Item.Id)))
            {
                Hero.PickUpItem(item);
            }
        }

        public void PickUpItems(bool all = false)
        {
            if (StashBottleTaken)
            {
                Bottle.Item?.MoveItem(StashBottleSlot);
                Bottle.Item = null;
                StashBottleTaken = false;
            }

            if (!droppedItems.Any() && !all)
            {
                return;
            }

            var items = ObjectManager.GetEntitiesParallel<PhysicalItem>()
                .Where(x => x.Distance2D(Hero) < 350 && (all || droppedItems.Contains(x.Item.Handle)))
                .ToList();

            var count = items.Count;

            if (count <= 0)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                Hero.PickUpItem(items[i], i != 0);
            }

            foreach (var itemSlot in itemSlots)
            {
                itemSlot.Value.MoveItem(itemSlot.Key);
            }

            itemSlots.Clear();
            droppedItems.Clear();
        }

        public void PickUpItemsOnMove(ExecuteOrderEventArgs args)
        {
            if (!droppedItems.Any())
            {
                return;
            }

            if (Sleeper.Sleeping("Used"))
            {
                args.Process = false;
                return;
            }

            args.Process = false;
            PickUpItems();
            Hero.Move(args.TargetPosition, true);
            Sleeper.Sleep(2000, "Main");
        }

        public void TakeBottleFromStash()
        {
            if (StashBottle.Item == null)
            {
                return;
            }

            SaveBottleStashSlot();

            for (var i = 0; i < 6; i++)
            {
                var currentSlot = (ItemSlot)i;
                var currentItem = Hero.Inventory.GetItem(currentSlot);

                if (currentItem == null || !UsableItems.Select(x => x.Item).Contains(currentItem))
                {
                    StashBottle.Item.MoveItem(currentSlot);
                    break;
                }
            }

            StashBottleTaken = true;
            Bottle.Item = StashBottle.Item;
            Bottle.SetSleep(300 + Game.Ping);
            Sleeper.Sleep(300 + Game.Ping, this);
        }

        private void OnUpdate(EventArgs args)
        {
            //if (!Sleeper.Sleeping(TranquilBoots))
            //{
            //    if (Menu.TranquilBoots.CombineActive)
            //    {
            //        TranquilBoots.FindItem();
            //    }

            //    Sleeper.Sleep(500, TranquilBoots);
            //}

            if (Sleeper.Sleeping(this))
            {
                return;
            }

            Sleeper.Sleep(3000, this);

            foreach (var item in UsableItems)
            {
                item.FindItem();
            }

            PowerTreads.FindItem();
            TranquilBoots.FindItem();
        }

        private void SaveBottleStashSlot()
        {
            for (var i = ItemSlot.StashSlot_1; i <= ItemSlot.StashSlot_6; i++)
            {
                var currentItem = Hero.Inventory.GetItem(i);
                if (currentItem == null)
                {
                    continue;
                }

                if (Hero.Inventory.GetItem(i).Equals(StashBottle.Item))
                {
                    StashBottleSlot = i;
                    break;
                }
            }
        }

        private void SaveItemSlot(Item item)
        {
            for (var i = ItemSlot.InventorySlot_1; i <= ItemSlot.InventorySlot_6; i++)
            {
                if (itemSlots.ContainsKey(i))
                {
                    continue;
                }

                var inventoryItem = Hero.Inventory.GetItem(i);
                if (inventoryItem == null || !inventoryItem.Equals(item))
                {
                    continue;
                }

                itemSlots.Add(i, item);
                break;
            }
        }
    }
}