using System;
using System.Linq;
using System.Threading.Tasks;
using Default;
using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Extensions;

namespace InLoox.ODataClient.Services
{
    public class ProjectService : IProjectService
    {
        private readonly Container _ctx;

        public ProjectService(Container ctx)
        {
            _ctx = ctx;
        }

        public Task<ProjectView> GetFirstOpenProjectByName()
        {
            var query = _ctx.projectview
                .OrderBy(k => k.Name)
                .Where(k => k.IsArchived == false && k.IsRequest == false && k.IsRecycled == false);

            return query.FirstOrDefaultSq();
        }

        public Task<ProjectView> GetProject(System.Linq.Expressions.Expression<Func<ProjectView, bool>> predicate)
        {
            var query = _ctx.projectview
                .Where(predicate);

            return query.FirstOrDefaultSq();
        }
    }
}
