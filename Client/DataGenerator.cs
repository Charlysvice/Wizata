using Common;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Client
{
    public class DataGenerator
    {
        private static Semaphore mutex = new Semaphore(0, 1);
        //In Seconds
        private const int BATCH_MIN_TIME = 60, BATCH_MAX_TIME = 60*5;
        private const int BATCHNOTIFICATION_MIN_TIME = 30, BATCHNOTIFICATION_MAX_TIME = 40;
        //In Milliseconds
        private const int DATA_ENTRANCE_MIN_TIME = 500, DATA_ENTRANCE_MAX_TIME = 5000;
        public static void GenerateBatch(int batchId)
        {
            Console.WriteLine("Generate Batch");
            HubConnection hubBatch = new HubConnectionBuilder().WithUrl($"https://wizataassessment.azurewebsites.net/batchPublisher").WithAutomaticReconnect().Build();
            Console.WriteLine("Generate Batch Wait");
            hubBatch.StartAsync().Wait();
            Console.WriteLine("Generate Batch Wait Done");
            Random randomBatchTime = new Random();
            Random randomBatchNotification = new Random();
            int nextBatchNotificationIn = randomBatchNotification.Next(BATCHNOTIFICATION_MIN_TIME, BATCHNOTIFICATION_MAX_TIME);
            int nextBatchIn = randomBatchTime.Next(BATCH_MIN_TIME, BATCH_MAX_TIME);
            
            DateTime startTime = DateTime.Now;
            DateTime notificationStartTime = DateTime.Now;
            BatchInfo batchInfo = new BatchInfo(batchId, startTime);
            hubBatch.InvokeAsync("PublishBatch", batchInfo).Wait();
            Console.WriteLine(batchInfo);
            Console.WriteLine("{0} Next Batch In : " + nextBatchIn + " seconds", startTime);
            mutex.Release();
            mutex.Release();
            while (true)
            {
                DateTime now = DateTime.Now;

                TimeSpan spanBatchNotificationTime = now - notificationStartTime;
                if (spanBatchNotificationTime.TotalSeconds >= nextBatchNotificationIn)
                {
                    notificationStartTime = now;
                    batchInfo.TimeStamp = now;
                    hubBatch.InvokeAsync("PublishBatch", batchInfo).Wait();
                    //Console.WriteLine(batchInfo);
                    nextBatchNotificationIn = randomBatchNotification.Next(BATCHNOTIFICATION_MIN_TIME, BATCHNOTIFICATION_MAX_TIME);
                }

                TimeSpan spanBatchTime = now - startTime;
                if (spanBatchTime.TotalSeconds >= nextBatchIn)
                {
                    startTime = now;
                    batchId++;
                    batchInfo = new BatchInfo(batchId, startTime);
                    hubBatch.InvokeAsync("PublishBatch", batchInfo).Wait();
                    //Console.WriteLine(batchInfo);
                    nextBatchIn = randomBatchTime.Next(BATCH_MIN_TIME, BATCH_MAX_TIME);
                    Console.WriteLine("{0} Next Batch In : " + nextBatchIn + " seconds",now);
                }
            }
        }
        public static void GenerateTemperature()
        {

            HubConnection hubBatch = new HubConnectionBuilder().WithUrl($"https://wizataassessment.azurewebsites.net/batchPublisher").WithAutomaticReconnect().Build();
            Console.WriteLine("Generate Temperature Wait");
            
            hubBatch.StartAsync().Wait();
            Console.WriteLine("Generate Temperature Wait Done");
            mutex.WaitOne();
            Random randomTemperature = new Random();
            Random randomWait = new Random();

            while (true)
            {
                DataInfo temperatureInfo = new DataInfo();
                temperatureInfo.TimeStamp = DateTime.Now;
                temperatureInfo.Value = randomTemperature.Next(0, 400);
                //Console.WriteLine(temperatureInfo);
                hubBatch.InvokeAsync("PublishTemperature", temperatureInfo).Wait();
                int wait = randomWait.Next(DATA_ENTRANCE_MIN_TIME, DATA_ENTRANCE_MAX_TIME);
                //Console.WriteLine("Wait:" + wait);
                Thread.Sleep(wait);
            }
        }
        public static void GeneratePressure()
        {

            HubConnection hubBatch = new HubConnectionBuilder().WithUrl($"https://wizataassessment.azurewebsites.net/batchPublisher").WithAutomaticReconnect().Build();
            Console.WriteLine("Generate Pressure Wait");
            hubBatch.StartAsync().Wait();
            Console.WriteLine("Generate Pressure Wait Done");
            mutex.WaitOne();
            Random randomPressure = new Random();
            Random randomWait = new Random();
            while (true)
            {
                DataInfo pressureInfo = new DataInfo();
                pressureInfo.TimeStamp = DateTime.Now;
                pressureInfo.Value = randomPressure.Next(500, 2000);
                //Console.WriteLine(pressureInfo);
                hubBatch.InvokeAsync("PublishPressure", pressureInfo).Wait();
                int wait = randomWait.Next(DATA_ENTRANCE_MIN_TIME, DATA_ENTRANCE_MAX_TIME);
                //Console.WriteLine("Wait:" + wait);
                Thread.Sleep(wait);
            }
        }
    }
}
