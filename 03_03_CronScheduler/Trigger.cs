using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ITrigger MissionTrigger = TriggerBuilder.Create().WithIdentity("Mission").WithSchedule(CronScheduleBuilder.CronSchedule("0 41 08 ? * MON")).Build();
            sched.ScheduleJob(Mission, MissionTrigger);
            sched.Start();
        }
    }
}
