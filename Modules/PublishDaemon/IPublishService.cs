using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Build.Client;

namespace PublishDaemon
{
    public interface IPublishService
    {
        IBuildDetail GetLastBuild(string ProjectCollection, string ProjectName, DateTime MinFinishTime);
        bool CanPublish(IBuildDetail build, string lastBuildVersion);
        void AccessPublishDirectory(string publishDirectory, string userName, string password);
        Version IncrementVersion(Version source);
        void Publish(string msbuildPath, string solutionPath, string publishDirectory, Version publishVersion);
    }
}
