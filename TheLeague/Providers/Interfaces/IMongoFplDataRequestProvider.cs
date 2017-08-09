using System.Threading.Tasks;
using TheLeague.SharedModels;

namespace TheLeague.Providers.Interfaces {
    public interface IMongoFplDataRequestProvider {
        void AddDataRequest(FplDataRequest dataRequest);
        Task<FplDataRequest> GetLatest();
    }
}