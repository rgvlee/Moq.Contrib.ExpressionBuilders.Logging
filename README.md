# Moq.Contrib.ExpressionBuilders.Logging

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/c43470e42c2e41188c1d683e13ed5d3a)](https://www.codacy.com/manual/rgvlee/Moq.Contrib.ExpressionBuilders.Logging?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=rgvlee/Moq.Contrib.ExpressionBuilders.Logging&amp;utm_campaign=Badge_Grade) [![Codacy Badge](https://app.codacy.com/project/badge/Coverage/c43470e42c2e41188c1d683e13ed5d3a)](https://www.codacy.com/manual/rgvlee/Moq.Contrib.ExpressionBuilders.Logging?utm_source=github.com&utm_medium=referral&utm_content=rgvlee/Moq.Contrib.ExpressionBuilders.Logging&utm_campaign=Badge_Coverage)

## Overview

Moq.Contrib.ExpressionBuilders.Logging started out in direct response to my personal pain verifying `ILogger.Log`/`ILogger<T>.Log` invocations. From matching multiple predicates to getting the log message to matching on logged values, the expressions can get long, complex and hard to follow pretty quickly.

As the name suggests, this library provides an expression builder that builds clean, readable statements that you can use to setup or verify `ILogger.Log`/`ILogger<T>.Log` invocations.

## Resources

-   [Source repository](https://github.com/rgvlee/Moq.Contrib.ExpressionBuilders.Logging/)
-   [Moq.Contrib.ExpressionBuilders.Logging - NuGet](https://www.nuget.org/packages/Moq.Contrib.ExpressionBuilders.Logging/)

## Usage

### Verify

Start with `Log.With` and build the expression that matches your expectation. The following will verify that you logged error 666 "I am a meat popsicle":

```c#
logger.Verify(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"), Times.Once);
```

Options that are not specified are defaulted to It.IsAny\<\>; you could simply verify that you've logged n number of errors:

```c#
logger.Verify(Log.With.LogLevel(LogLevel.Error), Times.Exactly(numberOfInvocations));
```

Predicates are supported so you can get fancy (error or critical log where the event id is between 1 and 1000 inclusive and the message contains "popsicle"):

```c#
logger.Verify(Log.With.LogLevel(x => x == LogLevel.Error || x == LogLevel.Critical)
        .And.EventId(x => x.Id >= 1 && x.Id <= 1000)
        .And.LogMessage(x => x.Contains("popsicle")),
    Times.Once);
```

You can invoke Verify on the Mock\<ILogger\>/Mock\<ILogger\<T\>\> or the mocked object itself:

```c#
var loggerMock = new Mock<ILogger<IFoo>>();
var logger = loggerMock.Object;

logger.LogError(666, "I am a meat popsicle");

loggerMock.Verify(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"), Times.Once);

logger.Verify(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"), Times.Once);
```

String equality (e.g., log message, logged values) is resolved using x == y. For everything else, equality is resolved using x.Equals(y). If you are providing a reference type to the LoggedValues or Exception options you will need to provide an implementation of x.Equals(y) for the equality condition to pass (see [`IEquatable<T>`](<https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1?view=netcore-3.1>) and/or [Fody.Equals](https://github.com/Fody/Equals)).

#### Exception and exception message matching

The following verifies that I had one log invocation where the exception message was equal to `exceptionMessage`:

```c#
var exceptionMessage = _fixture.Create<string>();
var exception = new Exception(exceptionMessage);
var logger = Mock.Of<ILogger<IFoo>>();

logger.LogError(666, exception, "I am a meat popsicle");

logger.Verify(Log.With.ExceptionMessage(exceptionMessage), Times.Once);
```

If you want to match on another condition (e.g., message contains x, message starts with y, message contains x and starts with y), use the predicate overload:

```c#
var exceptionMessage = "Wow that escalated quickly";
var firstWord = exceptionMessage.Split().First();
var thirdWord = exceptionMessage.Split().ElementAt(2);
var exception = new Exception(exceptionMessage);
var logger = Mock.Of<ILogger<IFoo>>();

logger.LogError(666, exception, "I am a meat popsicle");

logger.Verify(Log.With.ExceptionMessage(x => x.StartsWith(firstWord) && x.Contains(thirdWord)), Times.Once);
```

If you want to match on another property (e.g., InnerException.Message), use the Exception predicate overload:

```c#
var innerExceptionMessage = _fixture.Create<string>();
var innerException = new Exception(innerExceptionMessage);
var exceptionMessage = _fixture.Create<string>();
var exception = new Exception(exceptionMessage, innerException);
var logger = Mock.Of<ILogger<IFoo>>();

logger.LogError(666, exception, "I am a meat popsicle");

logger.Verify(Log.With.Exception(x => x.Message.Equals(exceptionMessage) && x.InnerException.Message.Equals(innerExceptionMessage)), Times.Once);
```

## Setup and Verifiable

As the expression builder builds an expression, you can use it to build the mock setup expression as well.

Verifying a verifiable setup with a custom callback:

```c#
var logMessage = "I am a meat popsicle";
var invocationCount = 0;

var loggerMock = new Mock<ILogger<IFoo>>();
loggerMock.Setup(Log.With.LogLevel(x => x == LogLevel.Error).And.LogMessage(logMessage)).Callback(() => invocationCount++).Verifiable();
var logger = loggerMock.Object;

for (var i = 0; i < numberOfInvocations; i++)
{
    logger.LogError(logMessage);
}

Mock.Verify(loggerMock);
Assert.That(invocationCount, Is.EqualTo(numberOfInvocations));
```

VerifyAll:

```c#
var loggerMock = new Mock<ILogger<IFoo>>();
loggerMock.Setup(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"));
var logger = loggerMock.Object;

for (var i = 0; i < numberOfInvocations; i++)
{
    logger.LogError(666, "I am a meat popsicle");
}

Mock.VerifyAll(loggerMock);
```

## Chaining behaviour

Options can only be specified once. The following will ignore the LogLevel.Error option:

```c#
logger.Verify(Log.With.LogLevel(LogLevel.Error).And.LogLevel(LogLevel.Critical), Times.Once);
```

Use the predicate overload instead:

```c#
logger.Verify(Log.With.LogLevel(x => x == LogLevel.Error || x == LogLevel.Critical), Times.Exactly(2));
```

This applies to all options except for LoggedValue:

```c#
var key1 = "key1";
var key2 = "key2";
var value1 = "value1";
var value2 = "value2";
var logMessage = $"The second property {{{key2}}} is first, followed by the first property {{{key1}}}";

var logger = Mock.Of<T>();

for (var i = 0; i < numberOfInvocations; i++)
{
    logger.LogError(logMessage, value2, value1);
}

logger.Verify(Log.With.LoggedValue(key1, value1).And.LoggedValue(key2, value2), Times.Exactly(numberOfInvocations));
```
