// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Options
{
    /// <summary>
    /// Extension methods for <see cref="IOptionsMonitor{TOptions}"/>.
    /// </summary>
    public static class OptionsMonitorExtensions
    {
        /// <summary>
        /// Registers a listener to be called whenever <typeparamref name="TOptions"/> changes.
        /// </summary>
        /// <typeparam name="TOptions">The type of options instance being monitored.</typeparam>
        /// <param name="monitor">The IOptionsMonitor.</param>
        /// <param name="listener">The action to be invoked when <typeparamref name="TOptions"/> has changed.</param>
        /// <returns>An object that should be disposed to stop listening for changes.</returns>
        public static IDisposable? OnChange<[DynamicallyAccessedMembers(Options.DynamicallyAccessedMembers)] TOptions>(
            this IOptionsMonitor<TOptions> monitor,
            Action<TOptions> listener)
                => monitor.OnChange((o, _) => listener(o));
    }
}
