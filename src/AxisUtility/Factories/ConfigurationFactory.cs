using System;

using Axis.Share.Soap.ActionService;

namespace AxisUtility.Factories
{
    public static class ConfigurationFactory
    {
        public static NewActionConfiguration CreateFireConfiguration(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(host));

            return new NewActionConfiguration
            {
                Name          = "Send notification about fire through TCP",
                TemplateToken = "com.axis.action.fixed.notification.tcp",
                Parameters = new ActionParameters
                {
                    Parameter = new[]
                    {
                        new ActionParameter
                        {
                            Name  = "port",
                            Value = "8002"
                        },
                        new ActionParameter
                        {
                            Name  = "qos",
                            Value = "0"
                        },
                        new ActionParameter
                        {
                            Name = "message",
                            Value =
                                "<?xml version=\"1.0\"?> <KEENEO_MESSAGE xmlns:xsi=\"http://www.w3.org/2001/XMLSchemainstance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"TYPE=\"ALARM_START \"SENDER_IP=\"<camera_ip>\"> <SCENARIO_NAME>fire</SCENARIO_NAME>  </KEENEO_MESSAGE>"
                        },
                        new ActionParameter
                        {
                            Name  = "host",
                            Value = host
                        }
                    }
                }
            };
        }

        public static NewActionConfiguration CreateSmokeConfiguration(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(host));

            return new NewActionConfiguration
            {
                Name          = "Send notification about smoke through TCP",
                TemplateToken = "com.axis.action.fixed.notification.tcp",
                Parameters = new ActionParameters
                {
                    Parameter = new[]
                    {
                        new ActionParameter
                        {
                            Name  = "port",
                            Value = "8002"
                        },
                        new ActionParameter
                        {
                            Name  = "qos",
                            Value = "0"
                        },
                        new ActionParameter
                        {
                            Name = "message",
                            Value =
                                "<?xml version=\"1.0\"?> <KEENEO_MESSAGE xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" TYPE=\"ALARM_START\" SENDER_IP=\"<camera_ip>\"> <SCENARIO_NAME>smoke</SCENARIO_NAME>  </KEENEO_MESSAGE>"
                        },
                        new ActionParameter
                        {
                            Name  = "host",
                            Value = host
                        }
                    }
                }
            };
        }
    }
}
