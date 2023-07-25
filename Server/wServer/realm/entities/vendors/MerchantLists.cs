using System;
using System.Collections.Generic;
using common;
using common.resources;
using log4net;
using wServer.realm.terrain;

namespace wServer.realm.entities.vendors
{
    public class ShopItem : ISellableItem
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShopItem));
        public ushort ItemId { get; private set; }
        public int Price { get; }
        public int Count { get; }

        public ShopItem(string name, int price, int count = -1) {
            Price = price;
            Count = count;
            if (Program.Resources.GameData.IdToObjectType.TryGetValue(name, out var type))
                ItemId = type;
            else
            {
                // hp pot fallback
                ItemId = 0x0a22;
                Log.Warn($"Could not add {name} to merchant lists, item not found.");
            }
        }
    }

    internal static class MerchantLists
    {
        private static readonly List<ISellableItem> Weapon = new()
        {
            new ShopItem("Dagger of Foul Malevolence", 1000),
            new ShopItem("Bow of Covert Havens", 1000),
            new ShopItem("Staff of the Cosmic Whole", 1000),
            new ShopItem("Wand of Recompense", 1000),
            new ShopItem("Sword of Acclaim", 1000),
            new ShopItem("Masamune", 1000),
            new ShopItem("Staff of the Vital Unity", 7500),
            new ShopItem("Sadamune", 7500),
            new ShopItem("Sword of Splendor", 7500),
            new ShopItem("Wand of Evocation", 7500),
            new ShopItem("Bow of Mystical Energy", 7500),
            new ShopItem("Dagger of Sinister Deeds", 7500)
        };

        private static readonly List<ISellableItem> Ability = new()
        {
            new ShopItem("Cloak of Ghostly Concealment", 2000),
            new ShopItem("Quiver of Elvish Mastery", 2000),
            new ShopItem("Elemental Detonation Spell", 2000),
            new ShopItem("Tome of Holy Guidance", 2000),
            new ShopItem("Helm of the Great General", 2000),
            new ShopItem("Colossus Shield", 2000),
            new ShopItem("Seal of the Blessed Champion", 2000),
            new ShopItem("Baneserpent Poison", 2000),
            new ShopItem("Bloodsucker Skull", 2000),
            new ShopItem("Giantcatcher Trap", 2000),
            new ShopItem("Planefetter Orb", 2000),
            new ShopItem("Prism of Apparitions", 2000),
            new ShopItem("Scepter of Storms", 2000),
            new ShopItem("Doom Circle", 2000)
        };

        private static readonly List<ISellableItem> Armor = new()
        {
            new ShopItem("Robe of the Grand Sorcerer", 1000),
            new ShopItem("Hydra Skin Armor", 1000),
            new ShopItem("Acropolis Armor", 1000),
            new ShopItem("Wyrmhide Armor", 7500),
            new ShopItem("Robe of the Star Mother", 7500),
            new ShopItem("Dominion Armor", 7500)
        };

        private static readonly List<ISellableItem> Rings = new()
        {
             new ShopItem("Ring of Unbound Attack", 3000),
             new ShopItem("Ring of Unbound Defense", 3000),
             new ShopItem("Ring of Unbound Speed", 3000),
             new ShopItem("Ring of Unbound Dexterity", 3000),
             new ShopItem("Ring of Unbound Vitality", 3000),
             new ShopItem("Ring of Unbound Wisdom", 3000),
             new ShopItem("Ring of Unbound Health", 3000),
             new ShopItem("Ring of Unbound Magic", 3000)
        };

        private static readonly List<ISellableItem> Aldragine = new()
        {
            new ShopItem("Scepter of the Other", 150),
            new ShopItem("Burden of the Warpawn", 160),
            new ShopItem("The Odyssey", 120),
            new ShopItem("The Executioner", 120),
            new ShopItem("Rip of Soul", 130),
            new ShopItem("Sincryer's Demise", 140)
        };

        private static readonly List<ISellableItem> Drannol = new()
        {
            new ShopItem("Aegis of the Devourer", 130),
            new ShopItem("Drannol's Fury", 120),
            new ShopItem("Grasp of Elysium", 140),
            new ShopItem("Fortification Shield", 160),
            new ShopItem("Never Before Seen", 120)
        };

        private static readonly List<ISellableItem> Special = new()
        {
            new ShopItem("Backpack", 100000),
            new ShopItem("Amulet of Resurrection", 750000)
        };

        private static readonly List<ISellableItem> LegendaryConsumables = new()
        {
            new ShopItem("Speedier Sprout", 100000),
            new ShopItem("Pandora's Box", 500000),
            new ShopItem("Purification Crystal", 250000)
        };

        private static readonly List<ISellableItem> Coins = new()
        {
            new ShopItem("100 Gold", 100),
            new ShopItem("1000 Gold", 1000),
            new ShopItem("10000 Gold", 10000),
            new ShopItem("100000 Gold", 100000)
        };

        public static readonly Dictionary<TileRegion, Tuple<List<ISellableItem>, CurrencyType, int>> Shops = new()
        {
            { TileRegion.Store_1, new Tuple<List<ISellableItem>, CurrencyType, int>(Weapon, CurrencyType.Fame, 0) },
            { TileRegion.Store_2, new Tuple<List<ISellableItem>, CurrencyType, int>(Ability, CurrencyType.Fame, 0) },
            { TileRegion.Store_3, new Tuple<List<ISellableItem>, CurrencyType, int>(Armor, CurrencyType.Fame, 0) },
            { TileRegion.Store_4, new Tuple<List<ISellableItem>, CurrencyType, int>(Rings, CurrencyType.Fame, 0) },
            { TileRegion.Store_5, new Tuple<List<ISellableItem>, CurrencyType, int>(Coins, CurrencyType.Gold, 0) },
            { TileRegion.Store_7, new Tuple<List<ISellableItem>, CurrencyType, int>(Special, CurrencyType.Fame, 0) },
            { TileRegion.Store_8, new Tuple<List<ISellableItem>, CurrencyType, int>(LegendaryConsumables, CurrencyType.Fame, 0) },
            { TileRegion.Store_15, new Tuple<List<ISellableItem>, CurrencyType, int>(Aldragine, CurrencyType.Onrane, 20) },
            { TileRegion.Store_16, new Tuple<List<ISellableItem>, CurrencyType, int>(Drannol, CurrencyType.Onrane, 20) },
        };
    }
}