﻿using System;
using System.Net.Mail;
using System.Data.Entity;
using System.Linq;
using GithubDashboard.Data;
using GithubDashboard.EmailHelpers;
using GithubDashboard.Helpers;
using GithubDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IPaymentProvider _paymentProvider;

        public PaymentController(IEmailService emailService, IPaymentProvider paymentProvider)
        {
            _emailService = emailService;
            _paymentProvider = paymentProvider;
        }

        [HttpGet]
        [Route("/pay/{orderId}")]
        public IActionResult Pay(Guid orderId)
        {
            return View(orderId);
        }

        [HttpPost]
        [Route("/pay/create/")]
        public IActionResult CreatePayment(Payment payment)
        {
            var email = User.GetEmail();

            var successfullPayment = false;

            using (var context = new MainDatabaseContext())
            {
                var order = context.Orders
                    .Where(x => x.Id == payment.OrderId)
                    .Include(p => p.ProductOrders.Select(po => po.Product))
                    .Include(p => p.Buyer)
                    .FirstOrDefault();

                if (order == null) return View("Error");

                successfullPayment = _paymentProvider.SendPaymentData(payment);

                order.AddPayment(payment, successfullPayment);
                order.AddBuyer(GetOrCreateNewBuyer(context, email));

                context.SaveChanges();
            }

            _emailService.SendEmail(email, successfullPayment ? EmailType.PaymentAccepted : EmailType.PaymentRefused);

            return successfullPayment ? View("Success") : View("Failure");
        }

        private Buyer GetOrCreateNewBuyer(MainDatabaseContext context, string email)
        {
            return context.Buyers.FirstOrDefault(x => x.Email == email)
                                ?? new Buyer(User.Identity.Name, User.GetEmail());
        }
    }
}
