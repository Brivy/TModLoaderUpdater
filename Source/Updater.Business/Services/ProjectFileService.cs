using System.Reflection;
using TModLoaderMaintainer.Application.Updater.Business.Contracts.Services;
using TModLoaderMaintainer.Models.ProjectFiles.Constants;

namespace TModLoaderMaintainer.Application.Updater.Business.Services
{
    public class ProjectFileService : IProjectFileService
    {
        public FileInfo OpenProjectFile(string filePath)
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.Load(typeof(ProjectFileNames).Assembly.GetName()).Location);
            var projectFilePath = !string.IsNullOrWhiteSpace(assemblyLocation) ? Path.Combine(assemblyLocation, filePath) : filePath;
            return new FileInfo(projectFilePath);
        }
    }
}
