using System;

namespace Util
{
    [Serializable]
    public struct MinMax<T>
    {
        public MinMax(T min, T max)
        {
            this.min = min;
            this.max = max;
        }
 
        public T min;
        public T max;
    }
}