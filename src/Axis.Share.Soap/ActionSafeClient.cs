using System;
using System.Threading.Tasks;

using Axis.Share.Soap.ActionService;

namespace Axis.Share.Soap
{
    public class ActionSafeClient
    {
        private readonly ActionClient _client;

        public ActionSafeClient(string address, string userName, string password)
        {
            _client = ClientFactory.CreateActionServiceClient(address, userName, password);
        }

        public virtual Task<Result> OpenAsync() => Result.From(async () =>
        {
            await _client.OpenAsync().ConfigureAwait(false);
        });

        public virtual Task<Result> CloseAsync() => Result.From(async () =>
        {
            await _client.CloseAsync().ConfigureAwait(false);
        });

        public virtual Task<Result<ActionConfiguration[]>> GetConfigurationsAsync() => Result<ActionConfiguration[]>.From(async () =>
        {
            var response = await _client.GetActionConfigurationsAsync().ConfigureAwait(false);

            return response.ActionConfigurations;
        });

        public virtual Task<Result<ActionRule[]>> GetRulesAsync() => Result<ActionRule[]>.From(async () =>
        {
            var response = await _client.GetActionRulesAsync().ConfigureAwait(false);

            return response.ActionRules;
        });

        public virtual Task<Result<string>> AddConfigurationAsync(NewActionConfiguration configuration) => Result<string>.From(async () =>
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            var response = await _client.AddActionConfigurationAsync(configuration).ConfigureAwait(false);

            return response.ConfigurationID;
        });

        public virtual Task<Result<string>> AddRuleAsync(NewActionRule rule) => Result<string>.From(async () =>
        {
            if (rule is null)
                throw new ArgumentNullException(nameof(rule));

            var response = await _client.AddActionRuleAsync(rule).ConfigureAwait(false);

            return response.RuleID;
        });

        public virtual Task<Result> RemoveConfigurationAsync(string configurationId) => Result.From(async () =>
        {
            if (string.IsNullOrWhiteSpace(configurationId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(configurationId));

            await _client.RemoveActionConfigurationAsync(configurationId).ConfigureAwait(false);
        });

        public virtual Task<Result> RemoveRuleAsync(string ruleId) => Result.From(async () =>
        {
            if (string.IsNullOrWhiteSpace(ruleId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(ruleId));

            await _client.RemoveActionRuleAsync(ruleId).ConfigureAwait(false);
        });
    }
}
