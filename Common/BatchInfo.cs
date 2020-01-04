using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class BatchInfo
    {
        public BatchInfo(int batchId, DateTime timestamp)
        {
            BatchId = batchId;
            BatchName = "Batch " + batchId;
            TimeStamp = timestamp;
            Temperature = new Computation();
            Pressure = new Computation();
        }
        public BatchInfo()
        {
            Temperature = new Computation();
            Pressure = new Computation();
        }
        public int BatchId { get; set; }
        public String BatchName { get; set; }
        public DateTime TimeStamp { get; set; }
        public Computation Temperature { get; set; }
        public Computation Pressure { get; set; }
        public override string ToString()
        {
            return BatchName + " " + TimeStamp.ToString("s");
        }
    }
}
