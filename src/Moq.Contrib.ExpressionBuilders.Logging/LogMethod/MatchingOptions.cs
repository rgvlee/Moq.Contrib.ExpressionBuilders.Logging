using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging.LogMethod
{
    public class MatchingOptions
    {
        public Predicate<LogLevel> LogLevelPredicate { get; set; }

        public Predicate<EventId> EventIdPredicate { get; set; }

        public List<Predicate<KeyValuePair<string, object>>> LoggedValuesPredicates { get; set; } = new List<Predicate<KeyValuePair<string, object>>>();

        public Predicate<Exception> ExceptionPredicate { get; set; }
    }
}