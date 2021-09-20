using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Buk.Gaming.Sanity.Models;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using Sanity.Linq.Extensions;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buk.Gaming.Sanity.Extensions;

namespace Buk.Gaming.Sanity {
    public class SanityTeamRepository : SanityRepository, ITeamRepository 
    {
        public SanityTeamRepository(SanityDataContext sanity, IMemoryCache cache): base(sanity, cache)
        {

        }

        public async Task<List<Team>> GetTeamsForOrganizationAsync(string organizationId)
        {
            return (await Sanity.DocumentSet<SanityTeam>().Where(i => i.Organization.Ref == organizationId).ToListAsync()).Select(i => i.ToTeam()).ToList();
        }

        public async Task<List<Team>> GetTeamsForOrganizationsAsync(IEnumerable<string> organizationIds)
        {
            return (await Sanity.DocumentSet<SanityTeam>().Where(i => organizationIds.Contains(i.Organization.Ref)).ToListAsync()).Select(i => i.ToTeam()).ToList();
        }

        public async Task<List<Team>> GetTeamsForGameAsync(string gameId)
        {
            return (await Sanity.DocumentSet<SanityTeam>().Where(i => i.Game.Ref == gameId).ToListAsync()).Select(i => i.ToTeam()).ToList();
        }

        public async Task SaveOrCreateTeamAsync(Team team)
        {
            if (string.IsNullOrEmpty(team.Id))
            {
                team.Id = Guid.NewGuid().ToString();

                await Sanity.DocumentSet<SanityTeam>().Create(team.ToSanity()).CommitAsync();
            } else
            {
                await Sanity.DocumentSet<SanityTeam>().Update(team.ToSanity()).CommitAsync();
            }
        }
    }
}