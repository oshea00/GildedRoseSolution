using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using App;
using static System.Math;

namespace Tests
{
    public class Tests
    {
        List<Item> StartingItems;

        [SetUp]
        public void Setup()
        { 
            StartingItems = new List<Item>
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
            };

        }

        [Test]
        public void LegacyItemsAgedCorrectly()
        {
            var app = new Program(StartingItems);
            app.UpdateQuality();
            Assert.AreEqual(new [] { 19, 1, 6, 80, 21 }, app.AgedItems.Select(i=>i.Quality).Take(5)); 
        }

        [Test]
        public void InventoryItemCanOnlyAge()
        {
            var inv = new InventoryItem
            {
                Name = "Conjured Mana Cake",
                SellIn = 3,
                Quality = 6
            };
            inv.UpdateQuality();
            Assert.AreEqual(2, inv.SellIn);
            Assert.AreEqual(6, inv.Quality);
        }

        [Test]
        [TestCase(0, ExpectedResult = 2, Description = "Conjured items are twice the decay of an item past sell date")]
        [TestCase(3, ExpectedResult = 4, Description = "Conjured items are twice normal decay rate")]
        public int ConjuredInventoryItemsDecayTwiceAsQuickly(int sell)
        {
            var inv = new InventoryItemConjured
            {
                Name = "Conjured Mana Cake",
                SellIn = sell,
                Quality = 6
            };
            inv.UpdateQuality();
            Assert.AreEqual(Max(0,sell-1), inv.SellIn);
            return inv.Quality;
        }

        [Test]
        [TestCase(0, ExpectedResult = 4, Description = "twice the decay of an item past sell date")]
        [TestCase(3, ExpectedResult = 5, Description = "normal decay rate")]
        public int DecayableItemsDecayTwiceAsFastWhenPastSellDate(int sell)
        {
            var inv = new InventoryItemDecaying
            {
                Name = "Decayable Cake",
                SellIn = sell,
                Quality = 6
            };
            inv.UpdateQuality();
            Assert.AreEqual(Max(0, sell - 1), inv.SellIn);
            return inv.Quality;
        }

        [Test]
        public void LegendaryItemsDontAgeAndDontDecay()
        {
            var inv = new InventoryItemLegendary{ 
                Name = "Sulfuras, Hand of Ragnaros", 
                SellIn = 0, 
                Quality = 80 };
            inv.UpdateQuality();
            Assert.AreEqual(0, inv.SellIn);
            Assert.AreEqual(80, inv.Quality);

        }

        [Test]
        public void ImprovableItemsCantExceedMAXQUALITY()
        {
            var inv = new InventoryItemImproving
            {
                Name = "Aged Brie",
                SellIn = 2,
                Quality = InventoryItemDecaying.MAX_QUALITY
            };
            inv.UpdateQuality();
            Assert.AreEqual(1, inv.SellIn);
            Assert.AreEqual(InventoryItemDecaying.MAX_QUALITY,inv.Quality);
        }

        [Test] 
        public void SomeItemsCanImproveWithAge()
        {
            var inv = new InventoryItemImproving { 
                Name = "Aged Brie", 
                SellIn = 2, 
                Quality = 0 };
            inv.UpdateQuality();
            Assert.AreEqual(1, inv.Quality);
        }

        [Test]
        [TestCase(0, 10,true, ExpectedResult = 0, Description = "zero quality after sell date")]
        [TestCase(0, 10, false, ExpectedResult = 10, Description = "keep quality after sell date")]
        [TestCase(5, 10, true, ExpectedResult = 13, Description ="quality rate 3 sellin <= 5")]
        [TestCase(10, 10, true, ExpectedResult = 12, Description = "quality rate 2 sellin <= 10")]
        [TestCase(11, 10, true, ExpectedResult = 11, Description = "quality rate 1 sellin > 10")]
        public int SomeItemsImproveBasedOnAge(int sell, int quality, bool zeroOnMax)
        {
            var inv = new InventoryItemAgeBasedImproving
            (
                "Backstage passes to a TAFKAL80ETC concert",
                sell,
                quality,
                (sellin) =>
                {
                    if (sellin <= 5)
                        return 3;
                    if (sellin <= 10)
                        return 2;
                    return 1;

                },
                zeroOnMax
            );
            inv.UpdateQuality();
            Assert.AreEqual(Max(0,sell - 1), inv.SellIn);
            return inv.Quality;
        }
    }
}