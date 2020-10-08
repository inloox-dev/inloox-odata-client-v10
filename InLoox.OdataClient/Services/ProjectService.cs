using Default;
using InLoox.ODataClient.Data.BusinessObjects;
using System.Linq;
using System.Threading.Tasks;

namespace InLoox.ODataClient.Extensions.Service
{
    public class ProjectService
    {
        private readonly Container _ctx;

        public ProjectService(Container ctx)
        {
            _ctx = ctx;
        }

        public Task<ProjectView> GetAnyOpenProject()
        {
            var query = _ctx.projectview
                .Where(k => k.IsArchived == false && k.IsRequest == false && k.IsRecycled == false);

            return query.FirstOrDefaultSq();
        }
    }
}
