using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    public class Program
    {
        IList<Item> Items;

        static void Main(string[] args)
        {
            System.Console.WriteLine("OMGHAI!");

            var app = new Program()
            {
                Items = new List<Item>
                {
                    new InventoryItem {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20,
                        QualityStrategy = DegradingQualityStrategy.Instance},
                    new InventoryItem {Name = "Aged Brie", SellIn = 2, Quality = 0,
                        QualityStrategy = ImprovingQualityStrategy.Instance},
                    new InventoryItem {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7,
                        QualityStrategy = DegradingQualityStrategy.Instance},
                    new InventoryItem {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
                    new InventoryItem
                    {
                        Name = "Backstage passes to a TAFKAL80ETC concert",
                        SellIn = 15,
                        Quality = 20,
                        QualityStrategy = BackstagePassQualityStrategy.Instance
                    },
                    new InventoryItem {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6,
                        QualityStrategy = ConjuredQualityStrategy.Instance},
                    }
            };        

            app.UpdateQuality();

        }

        public Program() { }
        public Program(IList<Item> items)
        {
            Items = items;
        }

        public IList<Item> AgedItems => Items.Select(i => i).ToList();

        public void UpdateQuality()
        {
            foreach (var item in Items)
            {
                var inv = item as InventoryItem;
                inv.Update();
            }

        }
    }

    public class Item
    {
        public string Name { get; set; }

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }
}
