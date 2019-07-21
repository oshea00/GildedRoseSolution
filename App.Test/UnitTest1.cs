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
                new InventoryItem {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20,
                    QualityStrategy = new DegradingQualityStrategy()},
                new InventoryItem {Name = "Aged Brie", SellIn = 2, Quality = 0,
                    QualityStrategy = new ImprovingQualityStrategy()},
                new InventoryItem {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7,
                    QualityStrategy = new DegradingQualityStrategy()},
                new InventoryItem {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
                new InventoryItem
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 15,
                    Quality = 20,
                    QualityStrategy = new BackstagePassQualityStrategy()
                },
                new InventoryItem {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6,
                    QualityStrategy = new ConjuredQualityStrategy()},
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
                Quality = 6,
            };
            inv.Update();
            Assert.AreEqual(2, inv.SellIn);
            Assert.AreEqual(6, inv.Quality);
        }

        [Test]
        [TestCase(0, ExpectedResult = 2, Description = "Conjured items are twice the decay of an item past sell date")]
        [TestCase(3, ExpectedResult = 4, Description = "Conjured items are twice normal decay rate")]
        public int ConjuredInventoryItemsDecayTwiceAsQuickly(int sell)
        {
            var inv = new InventoryItem
            {
                Name = "Conjured Mana Cake",
                SellIn = sell,
                Quality = 6,
                QualityStrategy = new ConjuredQualityStrategy()
            };
            inv.Update();
            Assert.AreEqual(Max(0,sell-1), inv.SellIn);
            return inv.Quality;
        }

        [Test]
        [TestCase(0, ExpectedResult = 4, Description = "twice the decay of an item past sell date")]
        [TestCase(3, ExpectedResult = 5, Description = "normal decay rate")]
        public int DecayableItemsDecayTwiceAsFastWhenPastSellDate(int sell)
        {
            var inv = new InventoryItem
            {
                Name = "Decayable Cake",
                SellIn = sell,
                Quality = 6,
                QualityStrategy = new DegradingQualityStrategy()
            };
            inv.Update();
            Assert.AreEqual(Max(0, sell - 1), inv.SellIn);
            return inv.Quality;
        }

        [Test]
        public void LegendaryItemsDontAgeAndDontDecay()
        {
            var inv = new InventoryItem{ 
                Name = "Sulfuras, Hand of Ragnaros", 
                SellIn = 0, 
                Quality = 80,
                QualityStrategy = new LegendaryQualityStrategy()
                 
            };
            inv.Update();
            Assert.AreEqual(0, inv.SellIn);
            Assert.AreEqual(80, inv.Quality);

        }

        [Test]
        public void ImprovableItemsCantExceedMAXQUALITY()
        {
            var inv = new InventoryItem
            {
                Name = "Aged Brie",
                SellIn = 2,
                Quality = InventoryItem.MAX_QUALITY,
                QualityStrategy = new ImprovingQualityStrategy()
            };
            inv.Update();
            Assert.AreEqual(1, inv.SellIn);
            Assert.AreEqual(InventoryItem.MAX_QUALITY,inv.Quality);
        }

        [Test] 
        public void SomeItemsCanImproveWithAge()
        {
            var inv = new InventoryItem {
                Name = "Aged Brie",
                SellIn = 2,
                Quality = 0,
                QualityStrategy = new ImprovingQualityStrategy()
            };
            inv.Update();
            Assert.AreEqual(1, inv.Quality);
        }

        [Test]
        [TestCase(0, 10, ExpectedResult = 0, Description = "zero quality after sell date")]
        [TestCase(5, 10, ExpectedResult = 13, Description ="quality rate 3 sellin <= 5")]
        [TestCase(10, 10,ExpectedResult = 12, Description = "quality rate 2 sellin <= 10")]
        [TestCase(11, 10,ExpectedResult = 11, Description = "quality rate 1 sellin > 10")]
        public int SomeItemsImproveBasedOnAge(int sell, int quality)
        {
            var inv = new InventoryItem
            {
                Name = "Backstage passes to a TAFKAL80ETC concert",
                SellIn = sell,
                Quality = quality,
                QualityStrategy = new BackstagePassQualityStrategy()
            };
            inv.Update();
            Assert.AreEqual(Max(0,sell - 1), inv.SellIn);
            return inv.Quality;
        }

        [Test]
        public void QualityReducesNormally()
        {
            IQualityStrategy legendaryStrategy = new LegendaryQualityStrategy();
            var inv = new InventoryItem { SellIn = 0, Quality = 5 };
            Assert.AreEqual(5, legendaryStrategy.UpdateQuality(inv));
        }

        [Test]
        [TestCase(0, 80, ExpectedResult = 80)]
        [TestCase(0, 5, ExpectedResult = 6)]
        public int QualityCanImprove(int sellIn, int quality)
        {
            IQualityStrategy improveQualityStrategy = new ImprovingQualityStrategy();
            var inv = new InventoryItem { SellIn = sellIn, Quality = quality };
            return improveQualityStrategy.UpdateQuality(inv);
        }

        [Test]
        [TestCase(0, 6, ExpectedResult = 4)]
        [TestCase(0, 1, ExpectedResult = 0)]
        [TestCase(1, 6, ExpectedResult = 5)]
        [TestCase(1, 0, ExpectedResult = 0)]
        public int QualityDegrades(int sellIn, int quality)
        {
            IQualityStrategy degradingQualityStrategy = new DegradingQualityStrategy();
            var inv = new InventoryItem { SellIn = sellIn, Quality = quality };
            return degradingQualityStrategy.UpdateQuality(inv);
        }

        [Test]
        [TestCase(0, 6, ExpectedResult = 2)]
        [TestCase(0, 1, ExpectedResult = 0)]
        [TestCase(1, 6, ExpectedResult = 4)]
        [TestCase(1, 0, ExpectedResult = 0)]
        public int ConjuredQualityDegradesFaster(int sellIn, int quality)
        {
            IQualityStrategy conjuredQualityStrategy = new ConjuredQualityStrategy();
            var inv = new InventoryItem { SellIn = sellIn, Quality = quality };
            return conjuredQualityStrategy.UpdateQuality(inv);
        }

        [Test]
        [TestCase(0, 10, ExpectedResult = 0)]
        [TestCase(5, 10, ExpectedResult = 13)]
        [TestCase(10, 10, ExpectedResult = 12)]
        [TestCase(11, 10, ExpectedResult = 11)]
        public int BackstagePassesCanImproveFaster(int sellIn, int quality)
        {
            IQualityStrategy backstageQualityStrategy = new BackstagePassQualityStrategy();
            var inv = new InventoryItem { SellIn = sellIn, Quality = quality };
            return backstageQualityStrategy.UpdateQuality(inv);
        }

    }
}