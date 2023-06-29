using System.Data;

namespace WorkflowNotifications
{
    public class DBResult
    {
        public DBResult()
        {
            // Database results
            dsResult = new DataSet();
        }

        // Database results
        public DataSet dsResult { get; set; }
        public int ErrorState { get; set; }
        public int ErrorSeverity { get; set; }
        public string Message { get; set; }
    }
}
