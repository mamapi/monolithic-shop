﻿using System.Data.Entity;
using System.Linq;
using GithubDashboard.Data;
using GithubDashboard.EmailHelpers;
using GithubDashboard.Services;
using Quartz;

namespace GithubDashboard.Jobs
{
    public class CheckTransferJob : IJob
    {
        private readonly IEmailService _emailService;
        private readonly ITransferCheckService _transferCheckService;

        public CheckTransferJob(IEmailService emailService, ITransferCheckService transferCheckService)
        {
            _emailService = emailService;
            _transferCheckService = transferCheckService;
        }

        public void Execute(IJobExecutionContext jobContext)
        {
            using (var context = new MainDatabaseContext())
            {
                var orders = context.Orders
                    .Where(x => x.Status == OrderStatus.WaitingForPayment)
                    .Include(o => o.Buyer)
                    .ToList();

                foreach (var order in orders)
                {
                    var hasReceivedMoney = _transferCheckService.Check(order.Id);
                    if(hasReceivedMoney)
                    {
                        order.Status = OrderStatus.Delivering;
                        _emailService.SendEmail(order.Buyer.Email, EmailType.TransferReceived);
                    }
                }

                context.SaveChanges();
            }
        }
    }
}