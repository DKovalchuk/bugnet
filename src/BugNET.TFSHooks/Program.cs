using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BugNET.TFSHooks
{
    class Program
    {
        private static string TFSLink
        {
            get
            {
                return ConfigurationManager.AppSettings["TfsLink"];
            }
        }

        private static string ProjectName
        {
            get
            {
                return ConfigurationManager.AppSettings["TfsProjectName"];
            }
        }

        private static string BugNetServicesUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["BugNetServicesUrl"];
            }
        }

        public static bool BugNetWindowsAuthentication
        {
            get
            {
                bool result = false;
                bool.TryParse(ConfigurationManager.AppSettings["BugNetWindowsAuthentication"], out result);

                return result;
            }
        }

        static void Main(string[] args)
        {
            var url = new Uri(TFSLink);

            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(url);

            tfs.Connect(Microsoft.TeamFoundation.Framework.Common.ConnectOptions.None);
            var tfsService = tfs.GetService<VersionControlServer>();
            var bugNetService = new WebServices.BugNetServices
                {
                    CookieContainer = new System.Net.CookieContainer(),
                    Url = BugNetServicesUrl
                };
            
            var history = tfsService
                .QueryHistory(
                    path: ProjectName,
                    version: VersionSpec.Latest,
                    deletionId: 0,
                    recursion: RecursionType.Full,
                    user: String.Empty,
                    versionFrom: null,
                    versionTo: VersionSpec.Latest,
                    maxCount: 100,
                    includeChanges: false,
                    slotMode: true);

            if(history != null && history is IEnumerable<Changeset>)
            {
                var changesetList = history as IEnumerable<Changeset>;
                foreach(var changeset in changesetList)
                {
                    
                }
            }
        }
    }
}
