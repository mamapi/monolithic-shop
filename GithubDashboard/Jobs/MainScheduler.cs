﻿using System;
using Quartz;
using Quartz.Impl;
using SimpleInjector;

namespace GithubDashboard.Jobs
{
    public static class MainScheduler
    {
        public static void Start(Container container)
        {
            try
            {
                var scheduler = container.GetInstance<IScheduler>();

                scheduler.Start();
                var job = JobBuilder.Create<WarehouseJob>().Build();

                var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(60)
                        .RepeatForever())
                    .Build();

                scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }
}