﻿using log4net.Config;
using Ninject;

namespace CheckyChatbotSlack {
    internal class Program {
        private static void Main() {
            XmlConfigurator.Configure();
            IKernel kernel = new StandardKernel(new CheckyChatbotModule());
            kernel.Load("Checky.*.dll");
            kernel.Get<IRunner>().Run();
        }
    }
}