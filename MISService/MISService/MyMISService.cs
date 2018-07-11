﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using MISService.Method;
using System.Threading;

namespace MISService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };  

    public partial class MyMISService : ServiceBase
    {
        private int eventId = 1;
        private const string LOGNAME = "MyMISNewLog";
        private const string EVENTNAME = "MyMISSource";
        public bool Running {get; set;} 

        System.Timers.Timer timer = new System.Timers.Timer();

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus); 

        public MyMISService()
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(EVENTNAME))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    EVENTNAME, LOGNAME);
            }
            eventLog1.Source = EVENTNAME;
            eventLog1.Log = LOGNAME;
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);  

            eventLog1.WriteEntry("MIS OnStart");
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);

            Running = true;
            Thread newThread = new Thread(DoWork);
            newThread.Start(this);

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void DoWork(object parent)
        {
            MyMISService m = (MyMISService)parent;
            LogMethods.Log.Info("-------------- *** Starting MISService *** ------------");
            int polling = 1;
            if (SalesForceMethods.AuthenticateSfdcEnterpriseUser())
            {
                while (m.Running)
                {
                    LogMethods.Log.Debug("* Polling:" + polling++);
                    ProjectMethods pm = new ProjectMethods();
                    pm.GetAllProjects();
                    Thread.Sleep(10000);
                }
            }
            LogMethods.Log.Info("-------------- *** Endings MISService *** ------------");
        }

        protected override void OnStop()
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);  

            eventLog1.WriteEntry("MIS OnStop");
            Running = false;

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("MIS OnContinue");
        }
    }
}
