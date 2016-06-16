using System.Collections.ObjectModel;

namespace Checky.Slack.AlertProxy.Areas.HelpPage.ModelDescriptions {
    public class EnumTypeModelDescription : ModelDescription {
        public EnumTypeModelDescription() {
            Values = new Collection<EnumValueDescription>();
        }

        public Collection<EnumValueDescription> Values { get; private set; }
    }
}