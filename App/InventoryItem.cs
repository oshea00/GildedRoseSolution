using System;
namespace App
{
    public interface IInventoryItem
    {
        void Update();
    }

    public interface IQualityStrategy
    {
        int UpdateQuality(InventoryItem inv);
    }

    public class LegendaryQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(InventoryItem inv)
        {
            return inv.Quality;
        }
    }

    public class ImprovingQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(InventoryItem inv)
        {
            if (inv.Quality > InventoryItem.MAX_QUALITY)
                return inv.Quality;
            return ((inv.Quality + 1) > InventoryItem.MAX_QUALITY ? inv.Quality : inv.Quality+1);
        }
    }

    public class DegradingQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(InventoryItem inv)
        {
            if(inv.SellIn == 0)
            {
                return ((inv.Quality - 2) < 0 ? 0 : inv.Quality - 2);
            }
            else
            {
                return ((inv.Quality - 1) < 0 ? 0 : inv.Quality - 1);
            }
        }
    }

    public class ConjuredQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(InventoryItem inv)
        {
            if (inv.SellIn == 0)
            {
                return ((inv.Quality - 4) < 0 ? 0 : inv.Quality - 4);
            }
            else
            {
                return ((inv.Quality - 2) < 0 ? 0 : inv.Quality - 2);
            }
        }
    }

    public class BackstagePassQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(InventoryItem inv)
        {
            if (inv.SellIn == 0)
                return 0;

            if (inv.SellIn <= 5)
                return ((inv.Quality + 3) > InventoryItem.MAX_QUALITY ? InventoryItem.MAX_QUALITY : inv.Quality + 3);

            if (inv.SellIn <= 10)
                return ((inv.Quality + 2) > InventoryItem.MAX_QUALITY ? InventoryItem.MAX_QUALITY : inv.Quality + 2);

            return ((inv.Quality + 1) > InventoryItem.MAX_QUALITY ? InventoryItem.MAX_QUALITY : inv.Quality + 1);
        }
    }

    public class InventoryItem:  Item, IInventoryItem
    {
        public const int MAX_QUALITY = 50;
        public IQualityStrategy QualityStrategy { get; set; }

        public InventoryItem()
        {
            QualityStrategy = new LegendaryQualityStrategy();
        }

        void AgeItem()
        {
            SellIn = ((SellIn - 1) < 0 ? 0 : SellIn - 1);
        }

        public void Update()
        {
            Quality = QualityStrategy.UpdateQuality(this);
            AgeItem();
        }
    }

}
