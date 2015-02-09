using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.TeamFoundation.Build.Client;

namespace Test_PublishDaemon
{
    public class FakeBuildDetail : IBuildDetail
    {
        public void Connect(int pollingInterval, int timeout, ISynchronizeInvoke synchronizingObject)
        {
            throw new NotImplementedException();
        }

        public void Connect(int pollingInterval, ISynchronizeInvoke synchronizingObject)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public IBuildDeletionResult Delete()
        {
            throw new NotImplementedException();
        }

        public IBuildDeletionResult Delete(DeleteOptions options)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void FinalizeStatus()
        {
            throw new NotImplementedException();
        }

        public void FinalizeStatus(BuildStatus status)
        {
            throw new NotImplementedException();
        }

        public void RefreshMinimalDetails()
        {
            throw new NotImplementedException();
        }

        public void RefreshAllDetails()
        {
            throw new NotImplementedException();
        }

        public void Refresh(string[] informationTypes, QueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Wait()
        {
            throw new NotImplementedException();
        }

        public bool Wait(TimeSpan pollingInterval, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public bool Wait(TimeSpan pollingInterval, TimeSpan timeout, ISynchronizeInvoke synchronizingObject)
        {
            throw new NotImplementedException();
        }

        public Guid RequestIntermediateLogs()
        {
            throw new NotImplementedException();
        }

        public string BuildNumber { get; set; }
        public BuildPhaseStatus CompilationStatus { get; set; }
        public string DropLocation { get; set; }
        public string DropLocationRoot { get; private set; }
        public string LabelName { get; set; }
        public bool KeepForever { get; set; }
        public string LogLocation { get; set; }
        public string Quality { get; set; }
        public BuildStatus Status { get; set; }
        public BuildPhaseStatus TestStatus { get; set; }
        public IBuildController BuildController { get; private set; }
        public Uri BuildControllerUri { get; private set; }
        public IBuildDefinition BuildDefinition { get; private set; }
        public Uri BuildDefinitionUri { get; private set; }
        public bool BuildFinished { get; private set; }
        public IBuildServer BuildServer { get; private set; }
        public IBuildInformation Information { get; private set; }
        public string LastChangedBy { get; private set; }
        public string LastChangedByDisplayName { get; private set; }
        public DateTime LastChangedOn { get; private set; }
        public string ProcessParameters { get; private set; }
        public BuildReason Reason { get; private set; }
        public ReadOnlyCollection<int> RequestIds { get; private set; }
        public ReadOnlyCollection<IQueuedBuild> Requests { get; private set; }
        public bool IsDeleted { get; private set; }
        public string SourceGetVersion { get; set; }
        public DateTime StartTime { get; private set; }
        public DateTime FinishTime { get; private set; }
        public Uri Uri { get; private set; }
        public string TeamProject { get; private set; }
        public string RequestedBy { get; private set; }
        public string RequestedFor { get; private set; }
        public string ShelvesetName { get; private set; }
        public event StatusChangedEventHandler StatusChanging;
        public event StatusChangedEventHandler StatusChanged;
        public event PollingCompletedEventHandler PollingCompleted;
    }
}