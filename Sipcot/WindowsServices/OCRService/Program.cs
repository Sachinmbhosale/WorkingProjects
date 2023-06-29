using System.ServiceProcess;

namespace OCRService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new DMSInfoserachOCR() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
