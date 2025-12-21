using CC.Domain.Entities;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;

namespace CC.Infrastructure.Persistences.Repositories
{
    public class FeatureRepository : GenericRepository<Feature, int>, IFeatureRepository
    {
        public FeatureRepository(AppDbContext context) : base(context)
        {
        }
    }
}
