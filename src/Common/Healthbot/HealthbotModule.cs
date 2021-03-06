﻿using System.Net.Http;
using Checky.Common.ComponentModel;
using Ninject.Modules;

namespace Checky.Common.Healthbot {
    public class HealthbotModule : NinjectModule {
        public override void Load() {
            Bind<IChatbotCommand>().To<HealthBotCommand>().InTransientScope();
            Bind<IChatbotCommand>().To<UrlBotCommand>().InTransientScope();
            Bind<IHealthcheckClient>().To<HealthcheckClient>().WithConstructorArgument((HttpClient) null);
            Bind<IHealthcheckFormatter>().To<HealthcheckFormatter>().InSingletonScope();
        }
    }
}