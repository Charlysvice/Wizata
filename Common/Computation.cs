using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Computation
    {
        private double mean = 0;
        private double sum = 0;
        private int count = 0;
        public void AddValue(int value)
        {
            //Knuth algorithm
            double previousMean = mean;
            count++;
            mean = mean + (value - mean) / count;
            sum = sum + (value - mean) * (value - previousMean);
        }
        public double Mean
        {
            get
            {
                return mean;
            }
            set
            {
                mean = value;
            }
        }
        public double Sum
        {
            get
            {
                return sum;
            }
            set
            {
                sum = value;
            }
        }
        public double Deviation
        {
            get
            {
                if (count > 0)
                    return Math.Sqrt(sum / count);
                else
                    return 0;
            }
        }

        public int Count { get => count; set => count = value; }
    }
}
