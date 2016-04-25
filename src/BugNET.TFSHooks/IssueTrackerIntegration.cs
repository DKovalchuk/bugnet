using log4net;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BugNET.TFSHooks
{
    public class IssueTrackerIntegration
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string IssueIdRegEx
        {
            get
            {
                return ConfigurationManager.AppSettings["IssueIdRegEx"];
            }
        }

        /// <summary>
        /// Updates the issue tracker from the change set.
        /// </summary>
        /// <param name="repository"> </param>
        /// <param name="changeset"> </param>
        /// <param name="service"> </param>
        public static void UpdateBugNetForChangeset(string repository, Changeset changeset, WebServices.BugNetServices service)
        {
            var issuesAffectedList = new List<int>();
            var regEx = new Regex(IssueIdRegEx, RegexOptions.IgnoreCase);

            var commitMessage = changeset.Comment.Trim();
            var matchResults = regEx.Match(commitMessage);

            if (!matchResults.Success) // none in the commit message
            {
                Log.Info("TFSHook: Found no Issue Ids in change set");
                return;
            }

            // capture the issues ids in the commit message
            // validate if the issue id is 
            // change the commit message for each issue id (may be more to the commit)
            while (matchResults.Success)
            {
                var value = matchResults.Groups[1].Value.Trim();
                var issueIdParts = value.Split(new[] { '-' });

                if (issueIdParts.Length.Equals(2))
                {
                    var idString = issueIdParts[1];

                    int issueId;
                    if (int.TryParse(idString, out issueId))
                    {
                        if (service.ValidIssue(issueId)) // check the issue to make sure it exists
                        {
                            commitMessage = Regex.Replace(commitMessage, AppSettings.IssueIdRegEx, "<a href=\"IssueDetail.aspx?id=$2#top\"><b>$1</b></a>");
                            issuesAffectedList.Add(issueId);
                        }
                    }
                }

                matchResults = matchResults.NextMatch();
            }

            if (issuesAffectedList.Count <= 0) return;


        }
}
