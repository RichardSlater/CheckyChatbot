using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Checky.Common.ComponentModel;

namespace Checky.Common.Chatbot {
    public class UsageCommand : IChatbotCommand {
        private readonly IHelpers _helpers;
        private readonly DateTime _startupTime = DateTime.UtcNow;
        private int _usageCount;

        public UsageCommand(IHelpers helpers) {
            _helpers = helpers;
        }

        public int Priority => 100;
        public string HelpText => "@checky usage";
        public string Example => "@checky usage";
        public string Verb => "usage";
        public string Description => "Displays usage statistics for the bot";

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            _usageCount++;
            if (!wasMentioned && !isDirectMessage) {
                return false;
            }
            var matcher = new Regex("\\b([usage]+)$", RegexOptions.Compiled);
            return _helpers.CanAcceptWithRegex(receivedText, matcher, "usage");
        }

        public Task Process(string command, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var delta = DateTime.UtcNow - _startupTime;
            return
                responseHandler(
                    $"I have been operational for {delta.TotalDays:N0} days, {delta.Hours} hours and {delta.Minutes} minutes. In that time I have seen to {_usageCount} message");
        }
    }
}