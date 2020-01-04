using Common;
using Microsoft.AspNetCore.SignalR;
using Server.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public class BatchPublisher : Hub
    {
        static List<DataInfo> pressures = new List<DataInfo>();
        static List<DataInfo> temperatures = new List<DataInfo>();
        static Dictionary<int, BatchInfo> batchIdComputations = new Dictionary<int, BatchInfo>();
        static DateTime batchStart, batchEnd;
        static int currentBatchId;
        static Object _lock = new Object();

        public ChannelReader<BatchInfo> Previous(int count, int delay, CancellationToken cancellationToken)
        {
            Console.WriteLine("Previous");
            var channel = Channel.CreateUnbounded<BatchInfo>();

            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<BatchInfo> writer,
            int count, int delay,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                List<BatchInfo> batchInfos = DBHelper.GetLastXBatches(count);
                Console.WriteLine(batchInfos.Count);
                foreach (BatchInfo batchInfo in batchInfos)
                {

                    await writer.WriteAsync(batchInfo, cancellationToken);

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                localException = ex;
            }

            writer.Complete(localException);
        }
        public void PublishBatch(BatchInfo batch)
        {

            if (!batchIdComputations.ContainsKey(batch.BatchId))
            {


                if (batchIdComputations.ContainsKey(currentBatchId))
                {
                    List<DataInfo> temperaturesToRemove = temperatures.Where(x => x.TimeStamp >= batchStart && x.TimeStamp <= batchEnd).ToList();
                    List<DataInfo> pressuresToRemove = pressures.Where(x => x.TimeStamp >= batchStart && x.TimeStamp <= batchEnd).ToList();

                    foreach (DataInfo dataInfo in pressuresToRemove)
                    {
                        batchIdComputations[currentBatchId].Pressure.AddValue(dataInfo.Value);
                    }
                    foreach (DataInfo dataInfo in temperaturesToRemove)
                    {
                        batchIdComputations[currentBatchId].Temperature.AddValue(dataInfo.Value);
                    }
                    lock (_lock)
                    {
                        foreach (DataInfo dataInfo in pressuresToRemove)
                        {
                            pressures.Remove(dataInfo);
                        }
                        foreach (DataInfo dataInfo in temperaturesToRemove)
                        {
                            temperatures.Remove(dataInfo);
                        }

                    }
                    DBHelper.AddBatch(batchIdComputations[currentBatchId]);
                    PublishBatchInfo(currentBatchId, batchIdComputations[currentBatchId]);
                }
                batchStart = batch.TimeStamp;
                batchIdComputations.Add(batch.BatchId, batch);
                currentBatchId = batch.BatchId;
            }
            batchEnd = batch.TimeStamp;
        }
        public Task PublishBatchInfo(int batchId, BatchInfo batchInfo)
        {
            Console.WriteLine("Publish Batch Info");
            return Clients.All.SendAsync("OnBatchInfoPublished", batchInfo);
        }
        public void PublishPressure(DataInfo pressure)
        {
            lock (_lock)
            {
                pressures.Add(pressure);
            }

        }
        public void PublishTemperature(DataInfo temperature)
        {
            lock (_lock)
            {
                temperatures.Add(temperature);
            }
        }
    }
}
