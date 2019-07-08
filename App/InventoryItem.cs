using System;
namespace App
{
    public interface IInventoryItem
    {
        void UpdateQuality();
    }

    public class InventoryItem:  Item, IInventoryItem
    {
        protected void AgeItem()
        {
            SellIn -= 1;
            if (SellIn < 0)
                SellIn = 0;
        }

        public virtual void UpdateQuality()
        {
            AgeItem();
        }
    }

    public class InventoryItemDecaying : InventoryItem, IRateable
    {
        public const int MIN_QUALITY = 0;
        public const int MAX_QUALITY = 50;
        public virtual int Rate 
        {
            get {
                return (SellIn > 0) ? 1 : 2;
            }
        }

        public virtual void ApplyRate()
        {
            Quality -= Rate;
            if (Quality < MIN_QUALITY)
                Quality = MIN_QUALITY;
            if (Quality > MAX_QUALITY)
                Quality = MAX_QUALITY;
        }

        public override void UpdateQuality()
        {
            ApplyRate();
            AgeItem();
        }
    }

    public class InventoryItemImproving : InventoryItemDecaying
    {
        public override int Rate => -1;
    }

    public class InventoryItemConjured : InventoryItemDecaying
    {
        public override int Rate => base.Rate * 2;
    }

    public class InventoryItemLegendary : InventoryItem
    {
    }

    public class InventoryItemAgeBasedImproving : InventoryItemImproving
    {
        Func<int, int> RateFormula;
        bool ZeroQualityMax;

        public InventoryItemAgeBasedImproving(string name, int sellin, int quality, Func<int,int> rateformula, bool zeromax)
        {
            Name = name;
            SellIn = sellin;
            Quality = quality;
            RateFormula = rateformula;
            ZeroQualityMax = zeromax;
        }

        public override int Rate
        {
            get
            {
                return RateFormula(SellIn) * base.Rate;
            }
        }

        public override void UpdateQuality()
        {
            if (SellIn == 0)
            {
                if (ZeroQualityMax)
                    Quality = 0;
            }
            else
            {
                base.UpdateQuality();
            }
        }
    }
}
