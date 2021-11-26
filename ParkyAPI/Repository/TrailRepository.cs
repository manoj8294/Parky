using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Model;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateTrail(Trail trail)
        {
            _db.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            return _db.Trails.Include(tr => tr.NationalPark).FirstOrDefault(np => np.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.Include(tr => tr.NationalPark).OrderBy( np => np.Id).ToList();
        }

        public bool TrailExists(string name)
        {
            bool value = _db.Trails.Any(np => np.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExists(int trailId)
        {
            bool value = _db.Trails.Any(np => np.Id == trailId);
            return value;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        public bool UpdateTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }

        public ICollection<Trail> GetTrailsInNationPark(int nationalParkId)
        {
            return _db.Trails.Include( tr => tr.NationalPark).Where(tr => tr.NationalParkId == nationalParkId).ToList();
        }
    }
}
