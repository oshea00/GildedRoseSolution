using System;
namespace App
{
    public interface IInventoryItem
    {
        void UpdateQuality();
    }

    public class InventoryItem: IInventoryItem
    {
        protected Item _item;

        public int SellIn => _item.SellIn;
        public int Quality => _item.Quality;
        public string Name => _item.Name;

        public InventoryItem(Item item)
        {
            _item = item;
        }

        protected void AgeItem()
        {
            _item.SellIn -= 1;
            if (_item.SellIn < 0)
                _item.SellIn = 0;
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
        public InventoryItemDecaying(Item item) : base(item)
        {
        }

        public virtual int Rate 
        {
            get {
                return (SellIn > 0) ? 1 : 2;
            }
        }

        public virtual void ApplyRate()
        {
            _item.Quality -= Rate;
            if (_item.Quality < MIN_QUALITY)
                _item.Quality = MIN_QUALITY;
            if (_item.Quality > MAX_QUALITY)
                _item.Quality = MAX_QUALITY;
        }

        public override void UpdateQuality()
        {
            ApplyRate();
            AgeItem();
        }
    }

    public class InventoryItemImproving : InventoryItemDecaying
    {
        public InventoryItemImproving(Item item) : base(item) { }
        public override int Rate => -1;
    }

    public class InventoryItemConjured : InventoryItemDecaying
    {
        public InventoryItemConjured(Item item) : base(item) {}
        public override int Rate => base.Rate * 2;
    }

    public class InventoryItemLegendary : InventoryItem
    {
        public InventoryItemLegendary(Item item) : base(item) { }
    }

    public class InventoryItemAgeBasedImproving : InventoryItemImproving
    {
        readonly Func<int, int> _rateFormula;
        readonly bool _zeroQualityMax;

        public InventoryItemAgeBasedImproving(Item item, Func<int,int> rateFormula, bool zeroQualityMax) : base(item) {
            _zeroQualityMax = zeroQualityMax;
            _rateFormula = rateFormula;
        }

        public override int Rate
        {
            get
            {
                return _rateFormula(SellIn) * base.Rate;
            }
        }

        public override void UpdateQuality()
        {
            if (SellIn == 0)
            {
                if (_zeroQualityMax)
                    _item.Quality = 0;
            }
            else
            {
                base.UpdateQuality();
            }
        }
    }
}
