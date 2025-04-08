using IDSAuditToolLib.Validator;
using IdsLib.IdsSchema.IdsNodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IDSValidator.Test.Utility
{
    public sealed class IDSEditorLogger : ILogger
    {
        public List<IDSMessageAudit> MessaggesStack { get; set; } = new List<IDSMessageAudit>();

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel >= LogLevel.Warning && state is IEnumerable<KeyValuePair<string, object>> properties)
            {
                var formattedError = formatter(state, exception);

                var error = new IDSMessageAudit();

                foreach (KeyValuePair<string, object> pair in properties)
                {
                    if (pair.Key == "errorCode")
                        error.Code = (int?)pair.Value;
                    else if (pair.Value is NodeIdentification nodeIdentification)
                    {
                        error.StartLineNumber = nodeIdentification.StartLineNumber;
                        error.StartLinePosition = nodeIdentification.StartLinePosition;
                        error.NodePath = nodeIdentification.PositionalIdentifier;
                        error.NodeType = nodeIdentification.NodeType;
                    }
                }

                var removeErrorMessage = Regex.Replace(formattedError, @"^Error \d+\: ", "");
                var removeXMLLineData = Regex.Replace(removeErrorMessage, @" element at line \d+\, position \d+", "");

                error.Message = removeXMLLineData;

                if (!error.Code.HasValue)
                    error.Code = 0;

                MessaggesStack.Add(error);
            }
        }
    }
}
