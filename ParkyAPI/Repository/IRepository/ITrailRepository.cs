using ParkyAPI.Model;
using System.Collections.Generic;

namespace ParkyAPI.Repository.IRepository
{
    public interface ITrailRepository
    {
        ICollection<Trail> GetTrails();

        ICollection<Trail> GetTrailsInNationPark(int nationalParkId);

        Trail GetTrail(int trailId);

        bool TrailExists(string name);

        bool TrailExists(int trailId);

        bool CreateTrail(Trail trail);

        bool UpdateTrail(Trail trail);

        bool DeleteTrail(Trail trail);

        bool Save();
    }
}
