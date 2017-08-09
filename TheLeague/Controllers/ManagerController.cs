using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TheLeague.Providers.Interfaces;
using TheLeague.SharedModels;

namespace TheLeague.Controllers {
    [Route("api/[controller]")]
    public class ManagerController : Controller {

        private readonly IMongoManagerProvider _mongoManagerProvider;

        public ManagerController(IMongoManagerProvider mongoManagerProvider) {
            _mongoManagerProvider = mongoManagerProvider;
        }

        [HttpGet("[action]")]
        public IEnumerable<Manager> GetAllManagers() {
            return _mongoManagerProvider.GetAll().Result;
        }

        [HttpGet("[action]")]
        public Manager GetManager(int id) {
            return _mongoManagerProvider.Get(id).Result;
        }
    }
}
