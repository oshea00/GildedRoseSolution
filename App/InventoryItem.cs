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
        static readonly LegendaryQualityStrategy _instance = new LegendaryQualityStrategy();
        public static LegendaryQualityStrategy Instance => _instance;
        public int UpdateQuality(InventoryItem inv)
        {
            return inv.Quality;
        }
    }

    public class ImprovingQualityStrategy : IQualityStrategy
    {
        static readonly ImprovingQualityStrategy _instance = new ImprovingQualityStrategy();
        public static ImprovingQualityStrategy Instance => _instance;
        public int UpdateQuality(InventoryItem inv)
        {
            if (inv.Quality > InventoryItem.MAX_QUALITY)
                return inv.Quality;
            return ((inv.Quality + 1) > InventoryItem.MAX_QUALITY ? inv.Quality : inv.Quality+1);
        }
    }

    public class DegradingQualityStrategy : IQualityStrategy
    {
        static readonly DegradingQualityStrategy _instance = new DegradingQualityStrategy();
        public static DegradingQualityStrategy Instance => _instance;
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
        static readonly ConjuredQualityStrategy _instance = new ConjuredQualityStrategy();
        public static ConjuredQualityStrategy Instance => _instance;
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
        static readonly BackstagePassQualityStrategy _instance = new BackstagePassQualityStrategy();
        public static BackstagePassQualityStrategy Instance => _instance;
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
            QualityStrategy = LegendaryQualityStrategy.Instance;
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
