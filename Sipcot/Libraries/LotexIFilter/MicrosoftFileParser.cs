using System.IO;
namespace Lotex.IFilter.Parser
{
    public static class MicrosoftFileParser
    {

        public static string Extract(string FileName)
        {
            string content;
            TextReader reader = new FilterReader(FileName);
            using (reader)
            {
                 content = reader.ReadToEnd();
            }
            return content;
        }
    }
}
