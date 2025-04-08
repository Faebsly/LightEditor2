// IProjectService.cs
using LightEditor2.Core.Models;

namespace LightEditor2.Core.Services
{
    public interface IProjectService
    {
        /// <summary>
        /// Alle Projekte inklusive Untergruppen abrufen
        /// </summary>
        /// <returns></returns>
        Task<List<Project>> GetProjectsAsync();

        /// <summary>
        /// Ein bestimmtes Projekt anhand der ID abrufen
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<Project?> GetProjectByIdAsync(int projectId);

        /// <summary>
        /// Ein neues Projekt hinzufügen
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<bool> AddProjectAsync(Project project);

        /// <summary>
        /// Ein bestehendes Projekt bearbeiten
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<bool> UpdateProjectAsync(Project project);

        /// <summary>
        /// Ein Projekt anhand der ID löschen
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<bool> DeleteProjectAsync(int projectId);

    }
}
