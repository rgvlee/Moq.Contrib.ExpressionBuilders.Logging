using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq.Contrib.ExpressionBuilders.Logging.Helpers
{
    /// <summary>
    ///     A helper to perform checks on arguments.
    /// </summary>
    public static class EnsureArgument
    {
        /// <summary>
        ///     Ensures that a string argument is not null or empty.
        /// </summary>
        /// <param name="argument">The string argument.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <returns>The string argument.</returns>
        /// <exception cref="ArgumentNullException">If the string argument is null.</exception>
        /// <exception cref="ArgumentException">If the string argument is empty.</exception>
        public static string IsNotNullOrEmpty(string argument, string argumentName)
        {
            if (!string.IsNullOrEmpty(argument))
            {
                return argument;
            }

            IsNotNull(argumentName, nameof(argumentName));
            IsNotNullOrEmpty(argumentName, nameof(argumentName));

            var ex = argument == null ? new ArgumentNullException(argumentName) : new ArgumentException(argumentName);
            throw ex;
        }

        /// <summary>
        ///     Ensures that an argument is not null.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <returns>The argument.</returns>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        public static T IsNotNull<T>(T argument, string argumentName) where T : class
        {
            if (argument != null)
            {
                return argument;
            }

            IsNotNull(argumentName, nameof(argumentName));
            IsNotNullOrEmpty(argumentName, nameof(argumentName));

            var ex = new ArgumentNullException(argumentName);
            throw ex;
        }

        /// <summary>
        ///     Ensures that a sequence is not empty.
        /// </summary>
        /// <typeparam name="T">The enumerable item type.</typeparam>
        /// <param name="argument">The enumerable argument.</param>
        /// <param name="argumentName">The enumerable argument name.</param>
        /// <returns>The enumerable argument.</returns>
        public static IEnumerable<T> IsNotEmpty<T>(IEnumerable<T> argument, string argumentName)
        {
            IsNotNull(argument, argumentName);

            if (argument.Any())
            {
                return argument;
            }

            IsNotNull(argumentName, nameof(argumentName));
            IsNotNullOrEmpty(argumentName, nameof(argumentName));

            var ex = new ArgumentException(argumentName);
            throw ex;
        }
    }
}