using Meltix_domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class VideoRepository : GenericRepository<Video>
    {
        public VideoRepository(DbContext context) : base(context)
        {
        }
    }
}
