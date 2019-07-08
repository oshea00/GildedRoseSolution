using System;
namespace App
{
    public interface IRateable
    {
        int Rate { get; }
        void ApplyRate();
    }
}
