using System;

namespace Common
{
    public class DataInfo
    {
        public DateTime TimeStamp { get; set; }
        public int Value { get; set; }
        public override string ToString()
        {
            return Value + " " + TimeStamp.ToString("s");
        }
    }
}
