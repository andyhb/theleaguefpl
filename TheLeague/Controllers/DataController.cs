using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class DataController : Controller {

        private readonly IMongoFplDataRequestProvider _mongoFplDataRequestProvider;

        public DataController(IMongoFplDataRequestProvider mongoFplDataRequestProvider) {
            _mongoFplDataRequestProvider = mongoFplDataRequestProvider;
        }

        [HttpGet("[action]")]
        public FplDataRequest GetLatest() {
            return _mongoFplDataRequestProvider.GetLatest().Result;
        }

        [HttpGet("[action]")]
        public FplDataRequest Update() {
            FplDataProvider.GetAllData();
            return GetLatest();
        }
    }
}
