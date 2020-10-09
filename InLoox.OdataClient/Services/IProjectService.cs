using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InLoox.ODataClient.Data.BusinessObjects;

namespace InLoox.ODataClient.Services
{
    public interface IProjectService
    {
        Task<ProjectView> GetFirstOpenProjectByName();
        Task<ProjectView> GetProject(Expression<Func<ProjectView, bool>> predicate);
    }
}