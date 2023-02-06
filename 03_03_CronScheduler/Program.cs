using System;

namespace _03_03_CronScheduler
{
    public class Program
    {
        static void Main(string[] args)
        {
            Trigger trigger = new Trigger();
            trigger.TriggerTheMission();
            Console.Read();
        }
    }
}


