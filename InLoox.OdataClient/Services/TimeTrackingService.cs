using Default;

namespace InLoox.ODataClient.Services
{

    public class TimeTrackingService
    {
        private readonly Container _ctx;

        public TimeTrackingService(Container ctx)
        {
            _ctx = ctx;
        }
    }

}