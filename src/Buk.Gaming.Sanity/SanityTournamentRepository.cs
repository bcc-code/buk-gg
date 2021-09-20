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
using System.Threading;

namespace Buk.Gaming.Sanity
{
    public class SanityTournamentRepository : SanityRepository, ITournamentRepository
    {
        public SanityTournamentRepository(SanityDataContext sanity, IMemoryCache cache) : base(sanity, cache)
        {

        }

        public async Task<List<Tournament>> GetTournamentsAsync()
        {
            return (await Sanity.DocumentSet<SanityTournament>().Where(i => !i.Id.StartsWith("draft")).ToListAsync()).Select(i => i.ToTournament(Sanity.HtmlBuilder)).ToList();
        }

        public async Task AddParticipantAsync(string tournamentId, Participant participant)
        {
            await UpdateDocumentAsync<SanityTournament>(tournamentId, (t) =>
            {
                if (t.SignupType == "player")
                {
                    if (participant.Type != ParticipantType.Player)
                    {
                        throw new Exception("Wrong participant type");
                    }
                    t.SoloPlayers.Add(participant.ToSanity());
                }
                else
                {
                    if (participant.Type != ParticipantType.Team)
                    {
                        throw new Exception("Wrong participant type");
                    }
                    t.Teams.Add(participant.ToSanity());
                }
            });
        }

        public async Task RemoveParticipantAsync(string tournamentId, string participantId)
        {
            await UpdateDocumentAsync<SanityTournament>(tournamentId, (t) =>
            {
                if (t.SignupType == "player")
                {
                    var participant = t.SoloPlayers.FirstOrDefault(i => i.Player.Ref == participantId);

                    if (participant == null)
                    {
                        throw new Exception("Tried to remove non-existing participant");
                    }

                    t.SoloPlayers.Remove(participant);
                } else
                {
                    var participant = t.Teams.FirstOrDefault(i => i.Team.Ref == participantId);

                    if (participant == null)
                    {
                        throw new Exception("Tried to remove non-existing participant");
                    }

                    t.Teams.Remove(participant);
                }
            });
        }
    }
}
