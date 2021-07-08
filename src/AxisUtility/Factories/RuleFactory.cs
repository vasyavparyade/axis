using System;
using System.Collections.Generic;
using System.Xml;

using Axis.Share.Soap.ActionService;

namespace AxisUtility.Factories
{
    public static class RuleFactory
    {
        /// <summary>
        ///     Document header for XML documents.
        /// </summary>
        private const string DocumentHeader = @"<?xml version=""1.0"" encoding=""UTF-8""?>";

        /// <summary>
        ///     XML for a topic filter.
        /// </summary>
        private const string TopicXml =
            @"<wsnt:TopicExpression " +
            @"xmlns:tns1=""http://www.onvif.org/ver10/topics"" " +
            @"xmlns:tnsaxis=""http://www.axis.com/2009/event/topics"" " +
            @"xmlns:wsnt=""http://docs.oasis-open.org/wsn/b-2"" " +
            @"Dialect=""http://www.onvif.org/ver10/tev/topicExpression/ConcreteSet"">" +
            @"{0}" +
            @"</wsnt:TopicExpression>";

        /// <summary>
        ///     XML for a message filter.
        /// </summary>
        private const string MessageXml =
            @"<wsnt:MessageContent " +
            @"xmlns:wsnt=""http://docs.oasis-open.org/wsn/b-2"" " +
            @"Dialect=""http://www.onvif.org/ver10/tev/messageContentFilter/ItemFilter"">" +
            @"{0}" +
            @"</wsnt:MessageContent>";

        public static NewActionRule CreateFireRule(string configurationId, TimeSpan? timeout)
        {
            timeout ??= TimeSpan.FromSeconds(5);

            return new NewActionRule
            {
                Name    = "Fire",
                Enabled = true,
                Conditions = new[]
                {
                    FormatTopicExpression("tns1:Device/tnsaxis:IO/Port//.",
                        "boolean(//SimpleItem[@Name=\"port\" and @Value=\"0\"]) and boolean(//SimpleItem[@Name=\"state\" and @Value=\"1\"])")
                },
                PrimaryAction     = configurationId,
                ActivationTimeout = $"PT{timeout.Value.Seconds}S"
            };
        }

        public static NewActionRule CreateSmokeRule(string configurationId, TimeSpan? timeout)
        {
            timeout ??= TimeSpan.FromMinutes(5);

            return new NewActionRule
            {
                Name    = "Smoke",
                Enabled = true,
                StartEvent = FormatTopicExpression("tnsaxis:CameraApplicationPlatform/SmokeGuard",
                    "boolean(//SimpleItem[@Name=\"event\" and @Value=\"Smoke Alarm\"]) and boolean(//SimpleItem[@Name=\"app\" and @Value=\"SmokeGuard\"])"),
                PrimaryAction     = configurationId,
                ActivationTimeout = $"PT{timeout.Value.Seconds}S"
            };
        }

        /// <summary>
        ///     Formats a topic expression from the provided topic and message filters.
        /// </summary>
        private static FilterType FormatTopicExpression(string topicFilter, string messageFilter = null)
        {
            var xmlElements = new List<XmlElement> { ToXml(TopicXml, topicFilter) };

            if (!string.IsNullOrWhiteSpace(messageFilter))
            {
                xmlElements.Add(ToXml(MessageXml, messageFilter));
            }

            return new FilterType { Any = xmlElements.ToArray() };
        }

        /// <summary>
        ///     Converts a string to an <see cref="XmlElement"/>.
        /// </summary>
        private static XmlElement ToXml(string xml, string data)
        {
            string document = DocumentHeader + string.Format(xml, data);

            var doc = new XmlDocument();
            doc.LoadXml(document);

            return doc.DocumentElement;
        }
    }
}
