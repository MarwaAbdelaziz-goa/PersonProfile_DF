using System;
using System.Runtime.ExceptionServices;

namespace PersonProfile_DF.Business
{	
    internal class GlobalExceptionHandler
    {
        // These variables are used to track if CurrentDomain_FirstChanceException and CurrentDomain_UnhandledException is NOT called recursively IF there is any error within these functions.
        // Keep in mind that even if we wrap the code (in CurrentDomain_FirstChanceException or CurrentDomain_UnhandledException) with try/catch/finally, then CurrentDomain_FirstChanceException IS still called by the Framework.
        private volatile bool _insideFirstChanceExceptionHandler;
        private volatile bool _insideUnhandledExceptionHandler;

        internal GlobalExceptionHandler()
        { }

        internal void RegisterGlobalExceptionHandler()
        {
            // Use ANY of the following lines to capture all the exceptions raised (including raised by the Microsoft Framework). Keep in mind that "FirstChanceException" will create lot of noise
            
            //AppDomain.CurrentDomain.FirstChanceException += new EventHandler<FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);

            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            // This will include more noise because these error will be reported even if a explicit error is being thrown by user-code.

            if (_insideFirstChanceExceptionHandler)
            {
                // Prevent recursion if an exception is thrown inside this method
                return;
            }

            _insideFirstChanceExceptionHandler = true;
            try
            {
                // As per the working of FirstChanceExceptions, if there is any error within the following TRY block, then the catch block is NOT going to catch that exception BEFORE it comes again to CurrentDomain_FirstChanceException.
                // So, _insideFirstChanceExceptionHandler is the safe guard to avoid infinite recursion ending with StackOverflow exception.

                var exception = e.Exception;

                //var methodBase = exception.TargetSite;
                //var className = methodBase.DeclaringType.FullName;
                //var methodName = methodBase.Name;
                string errDetails = exception.Message + System.Environment.NewLine + exception.StackTrace;

                Utilities.Logger.CaptureErrorLog(App.Configuration.ErrorLogDestination, App.Configuration.TextLogDirectory, App.Configuration.ConnectionString, exception, Guid.NewGuid());
            }
            catch
            {
                // You have to catch all exceptions inside this method
            }
            finally
            {
                _insideFirstChanceExceptionHandler = false;
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // This will introduce less noise as only Unhandled errors will be reported.

            if (_insideUnhandledExceptionHandler)
            {
                // Prevent recursion if an exception is thrown inside this method
                return;
            }

            _insideUnhandledExceptionHandler = true;
            try
            {
                // As per the working of FirstChanceExceptions, if there is any error within the following TRY block, then the catch block is NOT going to catch that exception BEFORE it comes again to CurrentDomain_FirstChanceException.
                // So, _insideFirstChanceExceptionHandler is the safe guard to avoid infinite recursion ending with StackOverflow exception.

                var exception = e.ExceptionObject as Exception;

                //var methodBase = exception.TargetSite;
                //var className = methodBase.DeclaringType.FullName;
                //var methodName = methodBase.Name;
                string errDetails = exception.Message + System.Environment.NewLine + exception.StackTrace;

                Utilities.Logger.CaptureErrorLog(App.Configuration.ErrorLogDestination, App.Configuration.TextLogDirectory, App.Configuration.ConnectionString, exception, Guid.NewGuid());
            }
            catch
            {
                // You have to catch all exceptions inside this method
            }
            finally
            {
                _insideUnhandledExceptionHandler = false;
            }
        }
    }       

}

