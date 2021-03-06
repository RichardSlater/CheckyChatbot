﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Checky.Common.ComponentModel {
    public interface IChatbotCommand {
        int Priority { get; }

        string HelpText { get; }

        string Example { get; }

        string Verb { get; }

        string Description { get; }

        bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage);

        Task Process(string command, string user, Func<string, Task> responseHandler,
                     IEnumerable<IChatbotCommand> otherCommands);
    }
}