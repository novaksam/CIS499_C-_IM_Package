// --------------------------------------------------------------------------------------------------------------------
// <copyright file="eventLogging.cs" company="Sam Novak">
//   CIS499 - 2013 - IM client
// </copyright>
// <summary>
//   The event logging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The event logging.
    /// </summary>
    public static class EventLogging
    {
        /// <summary>
        /// The name of the process logging events
        /// In this case "C# Instant message server"
        /// </summary>
        private const string EventSource = "C# Instant message client";

        /// <summary>
        /// Target event log for event logging
        /// System, Application, etc
        /// </summary>
        private const string EventLogName = "Application";

        /// <summary>
        /// The write event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        internal static void WriteEvent(string message, EventLogEntryType type)
        {
            EventLog.WriteEntry(EventSource, message, type, 0);
        }

        /// <summary>
        /// For logging exceptions during run time
        /// </summary>
        /// <param name="ex">
        /// The exception to be logged
        /// </param>
        internal static void WriteError(Exception ex)
        {
            EventLog.WriteEntry(EventSource, ex.Message, EventLogEntryType.Error, 1144);
        }

        /// <summary>
        /// Creates the event log source for the service.
        /// Only ran during startup of the service.
        /// </summary>
        internal static void CreateEventSource()
        {
            if (!EventLog.SourceExists(EventSource))
            {
                EventLog.CreateEventSource(EventSource, EventLogName);
            }
        }
    }
}