using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PersonProfile_DF.Utilities
{	
	public static class Logger
    {
        private enum LogDestination { Console, TextFile, Database };

        public static string CaptureErrorLog(string logDestination, string directoryName, string connectionString, Exception exception, Guid errorIdentifier, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogDestination destination = ConvertToEnum(logDestination);

            LogError logError = GetErrorDetail(exception, errorIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (destination == LogDestination.Console)
            {
                ToConsole(logError);
            }
            else if (destination == LogDestination.TextFile)
            {
                string fullFilename = directoryName + Path.DirectorySeparatorChar + "ErrorLog.txt";

                ToTextFile(fullFilename, logError);
            }
            else if (destination == LogDestination.Database)
            {
                string tableName = "LogError";
                Dictionary<string, object> columnsWithValues = new Dictionary<string, object>();
                columnsWithValues.Add("CorrelationId", logError.CorrelationId);
                columnsWithValues.Add("ErrorIdentifier", logError.ErrorIdentifier);
                columnsWithValues.Add("SourceFilename", logError.SourceFilename);
                columnsWithValues.Add("MethodName", logError.MethodName);
                columnsWithValues.Add("LineNumber", logError.LineNumber);
                columnsWithValues.Add("ErrorDetails", logError.ErrorDetails);
                columnsWithValues.Add("DateTimeStamp", logError.DateTimeStamp);

                ToDatabaseTable(connectionString, tableName, columnsWithValues);
            }

            return logError.ErrorIdentifier.ToString();
        }

        public static void CaptureApiTrace(string logDestination, string directoryName, string connectionString, LogTraceApiTraffic traceApi, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogDestination destination = ConvertToEnum(logDestination);

            if (destination == LogDestination.Console)
            {
                ToConsole(traceApi);
            }
            else if (destination == LogDestination.TextFile)
            {
                string fullFilename = directoryName + Path.DirectorySeparatorChar + "TraceApi.txt";

                ToTextFile(fullFilename, traceApi);
            }
            else if(destination == LogDestination.Database)
            {
                string tableName = "LogTraceApiTraffic";

                Dictionary<string, object> columnsWithValues = new Dictionary<string, object>();
                columnsWithValues.Add("CorrelationId", traceApi.CorrelationId);
                columnsWithValues.Add("UserInformation", traceApi.UserInformation);
                columnsWithValues.Add("ControllerName", traceApi.ControllerName);
                columnsWithValues.Add("ActionName", traceApi.ActionName);
                columnsWithValues.Add("MethodName", traceApi.MethodName);
                columnsWithValues.Add("DateTimeStampForRequest", traceApi.DateTimeStampForRequest);
                columnsWithValues.Add("DateTimeStampForResponse", traceApi.DateTimeStampForResponse);
                columnsWithValues.Add("RequestUrl", traceApi.RequestUrl);
                columnsWithValues.Add("QueryString", traceApi.QueryString);
                columnsWithValues.Add("RequestHeaders", traceApi.RequestHeaders);
                columnsWithValues.Add("RequestBody", traceApi.RequestBody);
                columnsWithValues.Add("IsSuccessStatusCode", traceApi.IsSuccessStatusCode);

                columnsWithValues.Add("ResponseHeaders", traceApi.ResponseHeaders);
                columnsWithValues.Add("ResponseBody", traceApi.ResponseBody);
                columnsWithValues.Add("ErrorIfAny", traceApi.ErrorIfAny);

                ToDatabaseTable(connectionString, tableName, columnsWithValues);
            }
        }

        public static void CaptureDbCalls(string logDestination, string directoryName, string connectionString, string dbQueryLog, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogDestination destination = ConvertToEnum(logDestination);
            LogTraceDbQuery log = new LogTraceDbQuery { DateTimeStamp = DateTime.Now, QueryDetails = dbQueryLog };

            if (destination == LogDestination.Console)
            {
                ToConsole(log);
            }
            else if (destination == LogDestination.TextFile)
            {
                string fullFilename = directoryName + Path.DirectorySeparatorChar + "ErrorLog.txt";

                ToTextFile(fullFilename, log);
            }
            else if (destination == LogDestination.Database)
            {
                string tableName = "LogTraceDbQuery";
                Dictionary<string, object> columnsWithValues = new Dictionary<string, object>();
                columnsWithValues.Add("QueryDetails", log.QueryDetails);
                columnsWithValues.Add("DateTimeStamp", log.DateTimeStamp);

                ToDatabaseTable(connectionString, tableName, columnsWithValues);
            }
        }
        private static LogDestination ConvertToEnum(string logDestination)
        {
            object obj;

            if(Enum.TryParse(typeof(LogDestination), logDestination, true, out obj))
            {
                return (LogDestination)obj;
            }
            else
            {
                throw new Exception("Invalid LogDestination. Valid values are: TextFile, Database.");
            }
        }

        private static void ToConsole(object objectToLog)
        {
            string textDetails = JsonConvert.SerializeObject(objectToLog, Formatting.Indented);
            Console.WriteLine(textDetails);
        }

        private static void ToTextFile(string fullFileName, object objectToLog)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
            }

            using (StreamWriter writer = new StreamWriter(fullFileName, true))
            {
                string textDetails = JsonConvert.SerializeObject(objectToLog, Formatting.Indented);
                writer.WriteLine(textDetails);
            }
        }

        private static void ToDatabaseTable(string connectionString, string tableName, Dictionary<string, object> columnsWithValues)
        {
            string sqlQuery = $"INSERT INTO {tableName}({string.Join(",", columnsWithValues.Select(x => x.Key))}) VALUES({string.Join(",", columnsWithValues.Select(x => "@" + x.Key))});";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    foreach (KeyValuePair<string, object> pair in columnsWithValues)
                    {
                        if(pair.Value == null || (pair.Value is string && string.IsNullOrEmpty((string)pair.Value)))
                        {
                            command.Parameters.AddWithValue(pair.Key, DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                    }

                    int nbrOfRowsInserted = command.ExecuteNonQuery();
                }
            }
        }

        private static string PrintExceptionDetails(Exception exp)
        {
            StringBuilder exceptionMessage = new StringBuilder();

            if (exp != null)
            {
                //exceptionMessage.AppendLine("Exception: " + exp.Message);

                //if (exp.InnerException != null)
                //{
                //    exceptionMessage.AppendLine("Inner Exception: " + exp.InnerException.Message);
                //}

                //exceptionMessage.AppendLine("Stack Trace: " + exp.StackTrace);
                exceptionMessage.AppendLine("Exception Details: " + exp.ToString());
            }

            return exceptionMessage.ToString();
        }

        private static LogError GetErrorDetail(Exception exception, Guid errorIdentifier, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            LogError logError = new LogError();
            
            logError.ErrorIdentifier = errorIdentifier;
            logError.SourceFilename = sourceFilePath;
            logError.MethodName = memberName;
            logError.LineNumber = sourceLineNumber;
            logError.ErrorDetails = PrintExceptionDetails(exception);
            logError.DateTimeStamp = DateTime.Now;

            return logError;
        }

        public class LogError
        {
            public string CorrelationId { get; set; }
            public Guid ErrorIdentifier { get; set; }
            public string SourceFilename { get; set; }
            public string MethodName { get; set; }
            public int LineNumber { get; set; }
            public string ErrorDetails { get; set; }
            public DateTime DateTimeStamp { get; set; }
        }

        public class LogTraceApiTraffic
        {
            public string CorrelationId { get; set; }
            public string UserInformation { get; set; }

            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public string MethodName { get; set; }            

            public DateTime DateTimeStampForRequest { get; set; }
            public string RequestUrl { get; set; }
            public string QueryString { get; set; }
            public string RequestHeaders { get; set; }
            public string RequestBody { get; set; }

            public DateTime DateTimeStampForResponse { get; set; }
            public bool IsSuccessStatusCode { get; set; }
            public string ResponseHeaders { get; set; }
            public string ResponseBody { get; set; }

            public string ErrorIfAny { get; set; }
        }

        class LogTraceDbQuery
        {
            public string QueryDetails { get; set; }
            public DateTime DateTimeStamp { get; set; }
        }
    }

}

