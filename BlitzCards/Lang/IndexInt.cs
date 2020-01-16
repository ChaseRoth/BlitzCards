namespace BlitzCards.Lang
{
    /// <summary>
    ///     A wrapper for int that provides indexing functionalities. - Inspired from Nullable<T>
    ///     
    ///     This can be broken easily broken but for the state of this program words fine.
    /// 
    /// </summary>
    public struct IndexInt
    {
        private int maxValue;
        private int minValue;
        private int value;

        /// <summary>
        ///     Holds the Maximum value that is allowed
        /// </summary>
        public int MaxValue { get => maxValue; set => maxValue = value; }

        /// <summary>
        ///     Holds the Minimum value that is allowed
        /// </summary>
        public int MinValue { get => minValue; set => minValue = value; }

        /// <summary>
        ///     Holds the current position or index value
        /// </summary>
        public int Value
        {
            get => value;
            set
            {
                // If attempting increment to the max we will overflow to min
                if (value > MaxValue)
                {
                    this.value = minValue;
                }
                // If attempting decrement past the min we overflow to the max
                else if (value < minValue)
                {
                    this.value = MaxValue;
                }
                // The current value is acceptable (between min & max)
                else
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        ///     Overload for setting the min & max values to a specified value. The "value" field will be automatically assigned 0.
        /// </summary>
        public IndexInt(int _minValue, int _maxValue)
        {
            minValue = _minValue;
            maxValue = _maxValue;
            value = 0;
        }

        /// <summary>
        ///     Overload for setting the min, max, and value fields to the specified value.
        /// </summary>
        public IndexInt(int _minValue, int _maxValue, int _value)
        {
            minValue = _minValue;
            maxValue = _maxValue;
            value = _value;
        }

        /// <summary>
        ///     Will increment the current value by one. If the current value before increment is equal to the Max or Min values allowed;
        ///     the value will overflow.
        /// </summary>
        public static IndexInt operator ++(IndexInt _instance)
        {
            _instance.Value++;
            return _instance;
        }

        /// <summary>
        ///     Will decrement the current value by one. If the current value before decrement is equal to the Max or Min values allowed;
        ///     the value will overflow.
        /// </summary>=
        public static IndexInt operator --(IndexInt _instance)
        {
            _instance.Value--;
            return _instance;
        }
    }
}