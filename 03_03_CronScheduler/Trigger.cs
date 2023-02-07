using Quartz;
using Quartz.Impl;

namespace _03_03_CronScheduler
{
    public class Trigger
    {
        private IScheduler Begin()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler().Result;
            if (!sched.IsStarted)
                sched.Start();
            return sched;
        }

        public void TriggerTheMission()
        {
            IScheduler sched = Begin();
            IJobDetail Mission = JobBuilder.Create<Job>().WithIdentity("Mission", null).Build();
            ITrigger MissionTrigger = TriggerBuilder.Create().WithIdentity("Mission").WithSchedule(CronScheduleBuilder.CronSchedule("0 31 09 * * ?")).Build(); // Every Monday 09:32 AM completes this task.
            sched.ScheduleJob(Mission, MissionTrigger);
            sched.Start();
        }
    }
}
