using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Serilog;

namespace AxisUtility
{
    public class Program
    {
        private static readonly Regex _cameraParametersRegex =
            new Regex(@"(?<login>[\w\W]+):+(?<password>[\w\W]+)@+(?<address>[\w\W]+)\|+(?<host>[\w\W]+)", RegexOptions.Compiled);

        private static CameraParameters[] _parameters;
        private static string _rule;
        private static FileInfo _file = null;

        public static async Task<int> Main(string[] args)
        {
            var logFile = Path.Combine(Directory.GetCurrentDirectory(), "log-.log");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information()
                .CreateLogger();

            var command = CreateCommand();

            var result = await command.InvokeAsync(args).ConfigureAwait(false);

            if (result != 0)
            {
                Log.Warning("Checking the startup arguments failed. The program crashed with code: {Code}.", result);

                return result;
            }

            await AddRulesAsync().ConfigureAwait(false);

            return 0;
        }

        private static async Task AddRulesAsync()
        {
            var handler = new RuleHandler();

            foreach (var parameters in _parameters)
            {
                parameters.RuleName = _rule;

                var addingRuleResult = await handler.AddRuleAsync(parameters).ConfigureAwait(false);

                if (!addingRuleResult)
                {
                    Log.Error(addingRuleResult.Error);

                    continue;
                }

                Log.Information("The rule \"{RuleName}\" has added for camera \"{CameraIp}\".", _rule, parameters.Address);
            }
        }

        private static RootCommand CreateCommand()
        {
            var rootCommand = new RootCommand
            {
                new Option<string>(new[] { "--camera", "-c" }, "The camera parameters: <login:password@ip|host>."),
                new Option<string>(new[] { "--rule", "-r" }, "The rule name: \"fire\" or \"smoke\"."),
                new Option<FileInfo>(new[] { "--file", "-f" }, "The path to the file."),
            };

            rootCommand.Description = "Axis camera utility";

            rootCommand.Handler = CommandHandler.Create<string, string, FileInfo>(ParseCommandLine);

            return rootCommand;
        }

        private static int ParseCommandLine(string camera, string rule, FileInfo file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(camera) && file is null)
                    throw new Exception("You must specify the parameters of the cameras.");

                if (!string.IsNullOrWhiteSpace(rule))
                {
                    var ruleTmp = rule.ToLower();

                    if (ruleTmp != Constants.Fire && ruleTmp != Constants.Smoke)
                        throw new Exception("The argument \"rule\" can only be \"fire\" or \"smoke\".");

                    _rule = ruleTmp;
                }

                if (!string.IsNullOrWhiteSpace(camera))
                {
                    var match = _cameraParametersRegex.Match(camera);

                    if (!match.Success)
                        throw new Exception("Camera parameters are not valid.");

                    _parameters = new[]
                    {
                        new CameraParameters
                        {
                            Login    = match.Groups["login"].Value,
                            Password = match.Groups["password"].Value,
                            Address  = match.Groups["address"].Value,
                            Host     = match.Groups["host"].Value,
                        }
                    };

                    return 0;
                }

                if (file is null || !file.Exists)
                    throw new Exception($"The file \"{file}\" not found.");

                _parameters = File.ReadLines(file.FullName)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x =>
                    {
                        var parameters = x.Split(';', StringSplitOptions.RemoveEmptyEntries).ToArray();

                        return new CameraParameters
                        {
                            Address  = parameters[0],
                            Login    = parameters[1],
                            Password = parameters[2],
                            Host     = parameters[3]
                        };
                    })
                    .ToArray();

                return 0;
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.Message);

                return 1;
            }

            return 0;
        }
    }
}
