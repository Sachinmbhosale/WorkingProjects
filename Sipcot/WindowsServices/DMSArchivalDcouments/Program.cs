using System.ServiceProcess;

namespace DMSArchivalDcouments
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
				new DocumentsArchival() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
