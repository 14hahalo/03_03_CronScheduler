using System;

namespace _03_03_CronScheduler
{
    public class Program
    {
        public static string inputEmail;
        public static string inputPassword;

        static void Main(string[] args)
        {
            Console.WriteLine("Lutfen e-mail adresinizi giriniz. ");
            inputEmail = Console.ReadLine();
            Console.WriteLine("\nLutfen sifrenizi giriniz. ");
            inputPassword = Console.ReadLine();

            Console.WriteLine("\nLutfen bekleyiniz.\n ");

            Trigger trigger = new Trigger();
            trigger.TriggerTheMission();
            Console.Read();
        }
    }
}


