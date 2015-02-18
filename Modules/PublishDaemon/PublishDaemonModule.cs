using System;
using System.Threading;
using Microsoft.TeamFoundation.Build.Client;
using SharedDaemonLib;

namespace PublishDaemon
{
    public class PublishDaemonModule : DaemonBase<PublishConfig>
    {
        private readonly IPublishService _publishService;

        public PublishDaemonModule()
        {
            _publishService = new RuntimePublishService();
        }

        public PublishDaemonModule(IPublishService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            _publishService = service;
        }

        public override void Start(CancellationToken cancellationToken)
        {
            do {
                DaemonStarted = true;
                IBuildDetail lastBuild = _publishService.GetLastBuild(PublishConfiguration.Collection,PublishConfiguration.ProjectName,PublishConfiguration.LastBuildTime);
                if (_publishService.CanPublish(lastBuild, PublishConfiguration.LastBuildVersion)) {
                    try {
                        _publishService.AccessPublishDirectory(PublishConfiguration.PublishDirectory,PublishConfiguration.UserName,PublishConfiguration.Password);
                        PublishConfiguration.PublishVersion =_publishService.IncrementVersion(PublishConfiguration.PublishVersion);
                        _publishService.Publish(PublishConfiguration.MsBuildPath, PublishConfiguration.SolutionPath,PublishConfiguration.PublishDirectory,PublishConfiguration.PublishVersion);
                        PublishConfiguration.LastBuildTime = lastBuild.FinishTime;
                        PublishConfiguration.LastBuildVersion = lastBuild.BuildNumber;
                    }
                    catch (Exception ex) {
                        if( !(ex is DaemonException) )
                            throw new DaemonException("Une Erreur inconnue c'est produite", ex);
                    }
                }
            } while (!cancellationToken.WaitHandle.WaitOne(PublishConfiguration.RefreshTime));
        }
    }
    

    [Serializable]
    public class PublishConfig : DaemonModuleConfiguration
    {
        
    }
}