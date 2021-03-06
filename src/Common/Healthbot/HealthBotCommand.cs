﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Checky.Common.ComponentModel;
using Checky.Common.Datastore.Environment;
using Ninject.Extensions.Logging;

namespace Checky.Common.Healthbot {
    public class HealthBotCommand : IChatbotCommand {
        private readonly IHealthcheckClient _client;
        private readonly IEnvironmentRepository _environments;
        private readonly IHealthcheckFormatter _formatter;
        private readonly IHelpers _helpers;
        private readonly ILogger _logger;

        private readonly Regex _matcher =
            new Regex("\\b([stau]+)\\s([0-9A-Za-z]+)\\s([0-9A-Za-z]+)(\\s([0-9A-Za-z]+))?$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public HealthBotCommand(IEnvironmentRepository environments, IHealthcheckClient client,
                                IHealthcheckFormatter formatter, IHelpers helpers, ILogger logger) {
            if (environments == null) throw new ArgumentNullException(nameof(environments));
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (helpers == null) throw new ArgumentNullException(nameof(helpers));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            _environments = environments;
            _client = client;
            _formatter = formatter;
            _helpers = helpers;
            _logger = logger;
        }

        public string Verb => "status";

        public string Description => "Queries services for their version number and health";

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            if (!wasMentioned && !isDirectMessage) {
                return false;
            }
            return _helpers.CanAcceptWithRegex(receivedText, _matcher, "status");
        }

        public int Priority => 50;

        public Task Process(string receivedText, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var match = _matcher.Match(receivedText);

            _logger.Debug("Recieved: '{0}' from {1} (Matched: {2})", receivedText, user, match.Success);

            if (!match.Success) {
                return responseHandler($"Sorry, I didn't understand `{receivedText}` try `{Example}`.");
            }

            var environmentText = match.Groups[2].Value;
            var serviceText = match.Groups[3].Value;
            string regionText = null;
            if (match.Groups.Count > 5) {
                regionText = match.Groups[5].Value;
            }
            var matchingEnvironments = _environments.Find(environmentText).ToArray();

            if (!matchingEnvironments.Any()) {
                return responseHandler($"Unable to find {environmentText}");
            }

            if (matchingEnvironments.Length > 1) {
                return
                    responseHandler(
                        $"`{environmentText}` matched too many environments, be more speicific.  I matched {string.Join(", ", matchingEnvironments)}");
            }

            var environment = _environments.Get(matchingEnvironments.Single());

            if (environment == null)
                return responseHandler($"Unable to find {environmentText}");

            var services = environment.Services.ToList();

            Func<Service, bool> servicePredicate =
                x =>
                    x.Name.StartsWith(serviceText, StringComparison.InvariantCultureIgnoreCase);

            if (!services.Any(servicePredicate))
                return
                    responseHandler(
                        $"Environment `{environment.Id}` exists but {serviceText} wasn't found, try one of these: `{string.Join("`, `", services.Select(x => x.Name.ToLower()))}`");

            var matchedServices = services.Where(servicePredicate).Select(x => x.Name).ToList();
            if (matchedServices.Count > 1) {
                return
                    responseHandler(
                        $"Matched {matchedServices.Count} services: `{string.Join("`, `", matchedServices)}` be more specific!");
            }

            var service = environment.Services.Single(servicePredicate);

            if (!string.IsNullOrWhiteSpace(regionText)) {
                var region = service.Regions?.SingleOrDefault(x => x.Name == regionText);

                if (region == null) {
                    return
                        responseHandler(
                            $"Environment `{environment.Id}` exists and contains `{serviceText}` however the region `{regionText}` wasn't found, try one of these: {string.Join(", ", service.Regions.Select(x => x.Name))}");
                }

                var regionState = _client.GetHealth(region.BaseUri, service.ServerCertificateSubject);
                return responseHandler(_formatter.Render(environment.Id, $"{service.Name} ({region.Name})", regionState));
            }

            var environmentState = _client.GetHealth(service.BaseUri, service.ServerCertificateSubject);
            return responseHandler(_formatter.Render(environment.Id, service.Name, environmentState));
        }

        public string HelpText => "*@checky*: status _environment_ _service_";

        public string Example => "@checky: status 52dev delivery";
    }
}