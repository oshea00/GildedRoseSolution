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
                    new InventoryItemDecaying {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                    new InventoryItemImproving {Name = "Aged Brie", SellIn = 2, Quality = 0},
                    new InventoryItemDecaying {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                    new InventoryItemLegendary {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                    new InventoryItemAgeBasedImproving
                          (
                              "Backstage passes to a TAFKAL80ETC concert",
                              15,
                              20,
                              sellin => {
                                  if (sellin <= 5)
                                      return 3;
                                  if (sellin <= 10)
                                      return 2;
                                  return 1;
                              },
                              true
                          ),
                    new InventoryItemConjured {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6},
                }
            };        

            app.UpdateQuality();

            System.Console.Read();

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
                inv.UpdateQuality();
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
