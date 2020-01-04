using System;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int batchId=DBHelper.GetLastBatchId();
            batchId++;
            Thread generateBatch = new Thread(()=>DataGenerator.GenerateBatch(batchId));

            Thread generateTemperature = new Thread(()=>DataGenerator.GenerateTemperature());
            Thread generatePressure = new Thread(()=>DataGenerator.GeneratePressure());

            generateBatch.Start();
            generateTemperature.Start();
            generatePressure.Start();
            generateBatch.Join();
            generateTemperature.Join();
            generatePressure.Join();
        }
    }
}
