using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using PublishDaemon.Exceptions;
using SharedDaemonLib;

namespace PublishDaemon
{
    public class RuntimePublishService : IPublishService
    {
        public IBuildDetail GetLastBuild(string ProjectCollection, string ProjectName, DateTime MinFinishTime)
        {
            Trace.WriteLine("Obtention du dernier build.");
            var tfs = new TfsTeamProjectCollection(new Uri(ProjectCollection));
            tfs.EnsureAuthenticated();

            var buildServer = tfs.GetService<IBuildServer>();

            IBuildDetailSpec buildSpec = buildServer.CreateBuildDetailSpec(ProjectName);
            buildSpec.InformationTypes = null;
            buildSpec.MinFinishTime = MinFinishTime;
            IBuildDetail[] bds = buildServer.QueryBuilds(buildSpec).Builds;
            return bds.AsEnumerable().OrderByDescending(b => b.FinishTime).FirstOrDefault();
        }

        public bool CanPublish(IBuildDetail build, string lastBuildVersion)
        {            
            return (build != null && build.Status == BuildStatus.Succeeded &&
                    build.TestStatus == BuildPhaseStatus.Succeeded &&
                    build.CompilationStatus == BuildPhaseStatus.Succeeded &&
                    build.BuildNumber != lastBuildVersion);
        }

        public void AccessPublishDirectory(string publishDirectory, string userName, string password)
        {
            Trace.WriteLine("Acces au repertoire de publication.");
            if (string.IsNullOrEmpty(publishDirectory))
                throw new PublishDirectoryNotFoundException("Le repertoire de publication n'a pas été specifié");

            if (!publishDirectory.StartsWith(@"\\")) {
                if(!Directory.Exists(publishDirectory))
                    throw new PublishDirectoryNotFoundException("Le repertoire de publication est introuvable.");
                if( HasFolderWritePermission(publishDirectory) )
                    throw new PublishDirectoryWriteErrorException("Vous n'avez pas la permission d'ecrire dans le repertoire de publication. " + publishDirectory);
                return;                
            }

            Trace.WriteLine("repertoire de type UNC.");
            if (!Directory.Exists(publishDirectory)) {
                Trace.WriteLine("Connexion à l'UNC");
                NetworkDrive networkDrive = new NetworkDrive();
                networkDrive.Force = true;
                networkDrive.ShareName = publishDirectory;
                networkDrive.MapDrive(userName, password);

                Trace.WriteLine("Connexion UNC terminée");
                if (HasFolderWritePermission(publishDirectory))
                    throw new PublishDirectoryWriteErrorException(
                        "Vous n'avez pas la permission d'ecrire dans le repertoire de publicaiton. " + publishDirectory);
            }
            Trace.WriteLine("Fin de la fonction.");
        }

        public Version IncrementVersion(Version source)
        {            
           return new Version(source.Major, source.Minor, source.Build, source.Revision + 1);         
        }

        public void Publish(string msbuildPath, string solutionPath, string publishDirectory, Version publishVersion)
        {
            if (!File.Exists(msbuildPath))
                throw new MsBuildNotFoundException("Impossible de trouver l'executable de compilation : " + msbuildPath);
            if (Path.GetFileName(msbuildPath).ToLower() != "msbuild.exe")
                throw new BadMsbuildException("L'executable de compilation doit être MSBUILD.exe");
            if (!File.Exists(solutionPath))
                throw new SolutionNotFoundException("La solution a publier est introuvable : " + solutionPath);
            if (Path.GetExtension(solutionPath).ToLower() != ".sln")
                throw new BadSolutionException("Le fichier à publier n'est pas une solution visual studio");
            
            Trace.WriteLine("Execution de la publication.");
            var process = Process.Start(msbuildPath, string.Format("/target:publish /p:Configuration=Release;PublishDir={2}\\ /property:ApplicationVersion={0};PublishVersion={0} \"{1}\"", publishVersion, solutionPath, publishDirectory));
            if (process != null) {
                process.WaitForExit();                
                Trace.WriteLine("Publication terminée");                
                /*CopyAll(new DirectoryInfo(tempPath), new DirectoryInfo(publishDirectory));
                Directory.Delete(tempPath, true);*/
            }
            Trace.WriteLine("fin de la fonction.");
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {            
            if (Directory.Exists(target.FullName) == false)
                Directory.CreateDirectory(target.FullName);
         
            foreach (FileInfo fi in source.GetFiles())
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private static bool HasFolderWritePermission(string destDir)
        {
            if (string.IsNullOrEmpty(destDir) || !Directory.Exists(destDir)) return false;
            try {
                DirectorySecurity security = Directory.GetAccessControl(destDir);
                var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                foreach (AuthorizationRule rule in security.GetAccessRules(true, true, typeof (SecurityIdentifier))) {
                    if (rule.IdentityReference == users) {
                        var rights = ((FileSystemAccessRule) rule);
                        if (rights.AccessControlType == AccessControlType.Allow) {
                            if (rights.FileSystemRights == (rights.FileSystemRights | FileSystemRights.Modify))
                                return true;
                        }
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }
    }

    [Serializable]
    public class PublishDirectoryNotFoundException : DaemonException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PublishDirectoryNotFoundException()
        {
        }

        public PublishDirectoryNotFoundException(string message) : base(message)
        {
        }

        public PublishDirectoryNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PublishDirectoryNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class PublishDirectoryWriteErrorException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PublishDirectoryWriteErrorException()
        {
        }

        public PublishDirectoryWriteErrorException(string message) : base(message)
        {
        }

        public PublishDirectoryWriteErrorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PublishDirectoryWriteErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}