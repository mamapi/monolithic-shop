﻿using System;
using Nancy;
using Nancy.Hosting.Self;
using Topshelf;

namespace ActorsTests
{
    public sealed class ProfileStatsEndpoint : NancyModule
    {
        public ProfileStatsEndpoint()
        {
            Get("/", args => Response.AsJson(true));
            Get("/status/", args => Response.AsJson(true));
        }
    }

    public class Microservice
    {
        private NancyHost _nancyHost;

        public void Start()
        {
            var cfg = new HostConfiguration();
            cfg.RewriteLocalhost = true;
            cfg.UrlReservations.CreateAutomatically = true;

            _nancyHost = new NancyHost(new Uri("http://localhost:5000"), new DefaultNancyBootstrapper(), cfg);
            _nancyHost.Start();

        }

        public void Stop()
        {
            _nancyHost.Stop();
            Console.WriteLine("Stopped. Good bye!");
        }
    }

    class Program
    {
        // http://blog.amosti.net/self-hosted-http-service-in-c-with-nancy-and-topshelf/

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Microservice>(s =>
                {
                    s.ConstructUsing(name => new Microservice());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Nancy-SelfHost example");
                x.SetDisplayName("Nancy-SelfHost Service");
                x.SetServiceName("Nancy-SelfHost");
            });
        }
    }
}
