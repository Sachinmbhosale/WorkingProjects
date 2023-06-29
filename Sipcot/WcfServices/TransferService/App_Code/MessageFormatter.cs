using System.ComponentModel;
using System.Web.UI;

namespace TransferService
{
    public enum MessageType { Success, Error, Notice }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:MessageFormatter runat=server></{0}:MessageFormatter>")]
    public class MessageFormatter : Control
    {
        public static string GetFormattedSuccessMessage(string message)
        {
            return GetFormattedMessage(message, MessageType.Success);
        }

        public static string GetFormattedErrorMessage(string message)
        {
            return GetFormattedMessage(message, MessageType.Error);
        }

        public static string GetFormattedNoticeMessage(string message)
        {
            return GetFormattedMessage(message, MessageType.Notice);
        }

        public static string GetFormattedMessage(string message, MessageType messageType = MessageType.Notice)
        {
            switch (messageType)
            {
                case MessageType.Success: return "<div class=\"alert alert-success\"> <button type=\"button\" class=\"close\" data-dismiss=\"alert\"> ×</button>Success:<strong></strong>" + " " + message + "</div>";
                case MessageType.Error: return "<div class=\"alert alert-danger\"> <button type=\"button\" class=\"close\" data-dismiss=\"alert\"> ×</button><strong>Error:</strong>" + " " + message + "</div>";
                default: return "<div class=\"alert alert-info\"> <button type=\"button\" class=\"close\" data-dismiss=\"alert\"> ×</button><strong>Notice:</strong>" + " " + message + "</div>";
            }
        }
    }
}