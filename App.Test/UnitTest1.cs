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
                new InventoryItem {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80,
                    QualityStrategy = new LegendaryQualityStrategy()},
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
            Assert.AreEqual(5, legendaryStrategy.UpdateQuality(0, 5, 50));
        }

        [Test]
        public void QualityCanImprove()
        {
            IQualityStrategy improveQualityStrategy = new ImprovingQualityStrategy();
            Assert.AreEqual(80, improveQualityStrategy.UpdateQuality(0, 80, 50));
            Assert.AreEqual(6, improveQualityStrategy.UpdateQuality(0, 5, 50));
        }

        [Test]
        public void QualityDegrades()
        {
            IQualityStrategy degradingQualityStrategy = new DegradingQualityStrategy();
            Assert.AreEqual(4, degradingQualityStrategy.UpdateQuality(0, 6, 50));
            Assert.AreEqual(0, degradingQualityStrategy.UpdateQuality(0, 1, 50));
            Assert.AreEqual(5, degradingQualityStrategy.UpdateQuality(1, 6, 50));
            Assert.AreEqual(0, degradingQualityStrategy.UpdateQuality(1, 0, 50));
        }

        [Test]
        public void ConjuredQualityDegradesFaster()
        {
            IQualityStrategy conjuredQualityStrategy = new ConjuredQualityStrategy();
            Assert.AreEqual(2, conjuredQualityStrategy.UpdateQuality(0, 6, 50));
            Assert.AreEqual(0, conjuredQualityStrategy.UpdateQuality(0, 1, 50));
            Assert.AreEqual(4, conjuredQualityStrategy.UpdateQuality(1, 6, 50));
            Assert.AreEqual(0, conjuredQualityStrategy.UpdateQuality(1, 0, 50));
        }

        [Test]
        public void BackstagePassesCanImproveFaster()
        {
            IQualityStrategy backstageQualityStrategy = new BackstagePassQualityStrategy();
            Assert.AreEqual(0, backstageQualityStrategy.UpdateQuality(0, 10, 50));
            Assert.AreEqual(13, backstageQualityStrategy.UpdateQuality(5, 10, 50));
            Assert.AreEqual(12, backstageQualityStrategy.UpdateQuality(10, 10, 50));
            Assert.AreEqual(11, backstageQualityStrategy.UpdateQuality(11, 10, 50));
        }

    }
}