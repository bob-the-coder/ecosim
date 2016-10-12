using System;
using System.Runtime.CompilerServices;
using NLog;

namespace Logging
{
    public class LogHelper
    {
        private static Logger _DefaultLogger;
        //TODO: Maybe Create a dictionary with all the loggers that are different from _DefaultLogger private static ConcurrentDictionary<String, String> are;

        /// <summary>
        ///     Property that exposes the interface of NLog Logger. (Property is readonly)
        /// </summary>
        private static Logger Logger
        {
            get
            {
                if (_DefaultLogger == null)
                {
                    _DefaultLogger = LogManager.GetCurrentClassLogger();
                }
                return _DefaultLogger;
            }
        }

        /// <summary>
        ///     This method will create a new logger with a specific name, because the default name is the actual name for the
        ///     current class.
        /// </summary>
        /// <typeparam name="T">the calling class from which the logging is done and name will be extracted</typeparam>
        /// <returns>the new logger with the name of T</returns>
        private static Logger CreateSpecificLogger<T>() where T : class
        {
            string loggerName = typeof (T).FullName;
            Logger result = null;
            if (!string.IsNullOrWhiteSpace(loggerName))
            {
                result = LogManager.GetLogger(loggerName);
            }
            return result;
        }

        /// <summary>
        ///     Allow the logging of an exeception as an error. The log entry will contain the stack trace of the execption and
        ///     an additional message in case the user provides it.
        /// </summary>
        /// <typeparam name="T">the calling class from which the logging is done and name will be extracted</typeparam>
        /// <param name="e">the execption to be logged</param>
        /// <param name="additionalMessage">an additional message that will be logged if provided</param>
        public static void LogException<T>(Exception e, string additionalMessage = null,
            [CallerMemberName] string memberName = "") where T : class
        {
            if (e != null)
            {
                Logger specificLogger = CreateSpecificLogger<T>();
                if (specificLogger != null)
                {
                    specificLogger.Error(CreateExceptionMessage(e, memberName, additionalMessage), e);
                }
            }
        }

        /// <summary>
        ///     Allow the logging of an exceptional case without having an actual .Net object exception.
        /// </summary>
        /// <typeparam name="T">the calling class from which the logging is done and name will be extracted</typeparam>
        /// <param name="exceptionMessage">the execption message to be logged</param>
        public static void LogException<T>(string exceptionMessage, [CallerMemberName] string memberName = "")
            where T : class
        {
            if (!string.IsNullOrWhiteSpace(exceptionMessage))
            {
                Logger specificLogger = CreateSpecificLogger<T>();
                if (specificLogger != null)
                {
                    specificLogger.Error("{0}: {1}", memberName, exceptionMessage);
                }
            }
        }

        /// <summary>
        ///     Allow the logging of an exeception as an error. The log entry will contain the stack trace of the execption and
        ///     an additional message in case the user provides it. The name of the logger in the logfiles will be the name of the
        ///     current class: "LaunchPad.Helpers.LogHelper"
        /// </summary>
        /// <param name="e">the execption to be logged</param>
        /// <param name="additionalMessage">an additional message that will be logged if provided</param>
        public static void LogException(Exception e, string additionalMessage = null,
            [CallerMemberName] string memberName = "")
        {
            if (e != null)
            {
                Logger.Error(CreateExceptionMessage(e, memberName, additionalMessage), e);
            }
        }

        /// <summary>
        ///     Allow the logging of an exceptional case without having an actual .Net object exception. The name of the logger in
        ///     the logfiles will be the name of the current class: "LaunchPad.Helpers.LogHelper"
        /// </summary>
        /// <param name="exceptionMessage">the execption message to be logged</param>
        public static void LogException(string exceptionMessage, [CallerMemberName] string memberName = "")
        {
            if (!string.IsNullOrWhiteSpace(exceptionMessage))
            {
                Logger.Error("{0}: {1}", memberName, exceptionMessage);
            }
        }


        /// <summary>
        ///     Creates a message to log from the exception using a particular template
        /// </summary>
        /// <param name="e">the exception to log</param>
        /// <param name="additionalMessage">the aditional message to add to the log</param>
        /// <returns>the formated string template based on the exception</returns>
        private static string CreateExceptionMessage(Exception e, string memberName, string additionalMessage = null)
        {
            string message = string.Empty;
            if (e != null)
            {
                if (!string.IsNullOrWhiteSpace(additionalMessage))
                {
                    message += string.Format("Message: {0} \n Stack Trace: \n {1}", additionalMessage, e);
                }
                else
                {
                    message += string.Format("Stack Trace: \n {0}", e);
                }
            }
            if (!string.IsNullOrWhiteSpace(memberName))
            {
                message = string.Format("{0}: {1}", memberName, message);
            }
            return message;
        }

        /// <summary>
        ///     Allow the logging of an informational message. The log entry will contain the message. The name of the logger in
        ///     the logfiles will be the name of the current class: "LaunchPad.Helpers.LogHelper"
        /// </summary>
        /// <param name="infoMessage">a message that will be logged if provided</param>
        public static void LogInfo(string infoMessage, [CallerMemberName] string memberName = "")
        {
            if (!string.IsNullOrWhiteSpace(infoMessage))
            {
                Logger.Info("{0}: {1}", memberName, infoMessage);
            }
        }

        /// <summary>
        ///     Allow the logging of an informational message. The log entry will contain the message. The name of the logger in
        ///     the logfiles will be the name of T
        /// </summary>
        /// <typeparam name="T">The class for which a new logger will be created</typeparam>
        /// <param name="infoMessage">a message that will be logged if provided</param>
        public static void LogInfo<T>(string infoMessage, [CallerMemberName] string memberName = "") where T : class
        {
            if (!string.IsNullOrWhiteSpace(infoMessage))
            {
                Logger specificLogger = CreateSpecificLogger<T>();
                if (specificLogger != null)
                {
                    specificLogger.Info("{0}: {1}", memberName, infoMessage);
                }
            }
        }
    }
}