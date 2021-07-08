using System;
using System.Linq;
using System.Threading.Tasks;

using Axis.Share;
using Axis.Share.Extensions;
using Axis.Share.Soap;

using AxisUtility.Factories;

using Serilog;

namespace AxisUtility
{
    public sealed class RuleHandler
    {
        public async Task<Result> AddRuleAsync(CameraParameters parameters)
        {
            Log.Debug("Starting execute method {Method}", nameof(AddRuleAsync));

            if (parameters is null)
                return Result.Fail($"{nameof(parameters)} is null");

            var client = new ActionSafeClient(parameters.Address, parameters.Login, parameters.Password);

            return await client.OpenAsync().Build()
                .Add(RemoveExistRuleAsync(client, parameters))
                .Add(AddNewRuleAsync(client, parameters))
                //.Add(client.CloseAsync())
                .ExecuteAsync()
                .ConfigureAwait(false);
        }

        private static Task<Result> AddNewRuleAsync(ActionSafeClient client, CameraParameters parameters) => parameters.RuleName switch
        {
            Constants.Fire  => AddFireRuleAsync(client, parameters),
            Constants.Smoke => AddSmokeRuleAsync(client, parameters),
            _               => throw new Exception("Unknown rule name")
        };

        private static async Task<Result> AddSmokeRuleAsync(ActionSafeClient client, CameraParameters parameters)
        {
            Log.Debug("Starting execute method {Method}", nameof(AddSmokeRuleAsync));

            var addingConfigurationResult = await client.AddConfigurationAsync(ConfigurationFactory.CreateSmokeConfiguration(parameters.Host));

            if (!addingConfigurationResult)
            {
                return Result.Fail($"Failed to add configuration for rule \"{parameters.RuleName}\" "
                  + $"for camera \"{parameters.Address}\": {addingConfigurationResult.Error}.");
            }

            string configurationId = addingConfigurationResult.Value;

            var addingRuleResult = await client.AddRuleAsync(RuleFactory.CreateSmokeRule(configurationId, TimeSpan.FromMinutes(5))).ConfigureAwait(false);

            if (!addingRuleResult)
            {
                return Result.Fail($"Failed to add rule {parameters.RuleName} "
                  + $"for camera {parameters.Address}: {addingConfigurationResult.Error}.");
            }

            return Result.Successful;
        }

        private static async Task<Result> AddFireRuleAsync(ActionSafeClient client, CameraParameters parameters)
        {
            Log.Debug("Starting execute method {Method}", nameof(AddFireRuleAsync));

            var addingConfigurationResult = await client.AddConfigurationAsync(ConfigurationFactory.CreateFireConfiguration(parameters.Host));

            if (!addingConfigurationResult)
            {
                return Result.Fail($"Failed to add configuration for rule \"{parameters.RuleName}\" "
                  + $"for camera \"{parameters.Address}\": {addingConfigurationResult.Error}.");
            }

            string configurationId = addingConfigurationResult.Value;

            var addingRuleResult = await client.AddRuleAsync(RuleFactory.CreateFireRule(configurationId, TimeSpan.FromSeconds(5))).ConfigureAwait(false);

            if (!addingRuleResult)
            {
                return Result.Fail($"Failed to add rule \"{parameters.RuleName}\" "
                  + $"for camera \"{parameters.Address}\": {addingConfigurationResult.Error}.");
            }

            return Result.Successful;
        }

        private static async Task<Result> RemoveExistRuleAsync(ActionSafeClient client, CameraParameters parameters)
        {
            Log.Debug("Starting execute method {Method}", nameof(RemoveExistRuleAsync));

            var gettingRulesResult = await client.GetRulesAsync().ConfigureAwait(false);

            if (!gettingRulesResult)
                return Result.Fail($"Failed to get rules from camera \"{parameters.Address}\": {gettingRulesResult.Error}.");

            var foundRule = gettingRulesResult.Value.FirstOrDefault(x => x.Name.ToLower() == parameters.RuleName);

            if (foundRule is null)
                return Result.Successful;

            parameters.RuleId          = foundRule.RuleID;
            parameters.ConfigurationId = foundRule.PrimaryAction;

            return await RemoveRuleAsync(client, parameters).Build()
                .Add(RemoveConfigurationAsync(client, parameters))
                .ExecuteAsync()
                .ConfigureAwait(false);
        }

        private static async Task<Result> RemoveRuleAsync(ActionSafeClient client, CameraParameters parameters)
        {
            Log.Debug("Starting execute method {Method}", nameof(RemoveRuleAsync));

            var result = await client.RemoveRuleAsync(parameters.RuleId).ConfigureAwait(false);

            if (!result)
                return Result.Fail($"Failed to remove rule \"{parameters.RuleName}\" for camera \"{parameters.Address}\": {result.Error}.");

            Log.Information("The rule \"{RuleName}\" for camera \"{Address}\" has removed.", parameters.RuleName, parameters.Address);

            return result;
        }

        private static async Task<Result> RemoveConfigurationAsync(ActionSafeClient client, CameraParameters parameters)
        {
            Log.Debug("Starting execute method {Method}", nameof(RemoveConfigurationAsync));

            // var r = await client.GetConfigurationsAsync();
            //
            // Log.Information("{C}", r.Value.ToJson(Formatting.Indented));

            var result = await client.RemoveConfigurationAsync(parameters.ConfigurationId).ConfigureAwait(false);

            if (!result)
                return Result.Fail($"Failed to remove configuration for rule \"{parameters.RuleName}\" for camera \"{parameters.Address}\": {result.Error}.");

            Log.Information("The configuration for rule \"{RuleName}\" for camera \"{Address}\" has removed.", parameters.RuleName, parameters.Address);

            return result;
        }
    }
}
