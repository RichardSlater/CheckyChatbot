﻿using Checky.Common.ComponentModel;
using Ninject.Modules;

namespace Checky.Common.Chatbot {
    public class ChatbotModule : NinjectModule {
        public override void Load() {
            Bind<IChatbotCommand>().To<FallbackCommand>().InSingletonScope();
            Bind<IChatbotCommand>().To<UsageCommand>().InSingletonScope();
            Bind<IHelpers>().To<Helpers>().InThreadScope();
        }
    }
}