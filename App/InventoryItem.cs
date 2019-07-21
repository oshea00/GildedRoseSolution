using System;
namespace App
{
    public interface IInventoryItem
    {
        void Update();
    }

    public interface IQualityStrategy
    {
        int UpdateQuality(int currentAge, int currentQuality, int maxQuality);
    }

    public class LegendaryQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(int currentAge, int currentQuality, int maxQuality)
        {
            return currentQuality;
        }
    }

    public class ImprovingQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(int currentAge, int currentQuality, int maxQuality)
        {
            if (currentQuality > maxQuality)
                return currentQuality;
            return ((currentQuality + 1) > maxQuality ? currentQuality : currentQuality+1);
        }
    }

    public class DegradingQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(int currentAge, int currentQuality, int maxQuality)
        {
            if(currentAge == 0)
            {
                return ((currentQuality - 2) < 0 ? 0 : currentQuality - 2);
            }
            else
            {
                return ((currentQuality - 1) < 0 ? 0 : currentQuality - 1);
            }
        }
    }

    public class ConjuredQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(int currentAge, int currentQuality, int maxQuality)
        {
            if (currentAge == 0)
            {
                return ((currentQuality - 4) < 0 ? 0 : currentQuality - 4);
            }
            else
            {
                return ((currentQuality - 2) < 0 ? 0 : currentQuality - 2);
            }
        }
    }

    public class BackstagePassQualityStrategy : IQualityStrategy
    {
        public int UpdateQuality(int currentAge, int currentQuality, int maxQuality)
        {
            if (currentAge == 0)
                return 0;

            if (currentAge <= 5)
                return ((currentQuality + 3) > maxQuality ? maxQuality : currentQuality + 3);

            if (currentAge <= 10)
                return ((currentQuality + 2) > maxQuality ? maxQuality : currentQuality + 2);

            return ((currentQuality + 1) > maxQuality ? maxQuality : currentQuality + 1);
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

        protected void AgeItem()
        {
            SellIn = ((SellIn - 1) < 0 ? 0 : SellIn - 1);
        }

        public virtual void Update()
        {
            Quality = QualityStrategy.UpdateQuality(SellIn, Quality, MAX_QUALITY);
            AgeItem();
        }
    }

}
