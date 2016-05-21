﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComponentModel;

namespace Chatbot {
    public class FallbackCommand : IChatbotCommand {
        public int Priority => int.MaxValue;
        public string HelpText => string.Empty;
        public string Example => string.Empty;
        public string Verb => string.Empty;

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            return true;
        }

        public Task Process(string command, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var possibleVerbs = otherCommands
                .Select(x => x.Verb)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Aggregate((current, next) => $"`{current}`, `{next}`");
            return
                responseHandler(
                    $"Sorry, I didn't understand `{command}`, try: `@checky: <verb> [<arguments>]` where _<verb>_ is one of these: {possibleVerbs}");
        }
    }
}