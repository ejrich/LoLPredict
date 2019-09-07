using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace LoLPredict.Web.DAL
{
    public interface IGameRepository
    {
        Task<IEnumerable<Patch>> LoadPatches();
        Task<IEnumerable<Champion>> LoadChampionsByPatch(string patch);
        Task<Model> LoadLatestModel(string patch);
    }

    public class GameRepository : IGameRepository
    {
        private readonly GameContext _context;

        public GameRepository(GameContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patch>> LoadPatches()
        {
            var patches = await _context.Patches.ToListAsync();

            return patches;
        }

        public async Task<IEnumerable<Champion>> LoadChampionsByPatch(string patch)
        {
            var champions = await _context.Champions
                .Where(_ => _.Patch == patch)
                .OrderBy(_ => _.Name)
                .ToListAsync();

            return champions;
        }

        public async Task<Model> LoadLatestModel(string patch)
        {
            var model = await _context.Models
                .OrderByDescending(_ => _.Id)
                .FirstOrDefaultAsync(_ => _.Patch == patch);

            return model;
        }
    }
}
