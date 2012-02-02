using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Nauplius.SharePoint.ADLDS.UserProfiles
{
	partial class ImportService : ServiceBase
	{
		static void Main()
		{
			if (!Environment.UserInteractive)
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					new ImportService() 
				};

				ServiceBase.Run(ServicesToRun);
			}
			else
			{
				ImportService svc = new ImportService();
				Console.WriteLine("Running Nauplius.SharePoint.ADLDS.UserProfiles in test mode.");
				Console.WriteLine("Press any key to quit...");

				svc.OnStart(null);

				Console.ReadKey();

				svc.OnStop();
			}
		}

		public ImportService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Program p = new Program();
			p.Timer();
		}
	}
}
