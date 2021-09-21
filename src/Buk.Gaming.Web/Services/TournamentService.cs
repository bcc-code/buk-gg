using Buk.Gaming.Classes;
using Buk.Gaming.Extensions;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Services;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class TournamentService : BaseService, ITournamentService
    {
        private readonly ITournamentRepository _tournaments;
        private readonly IPlayerRepository _players;
        private readonly IOrganizationService _organizations;
        private readonly ITeamService _teams;
        private readonly Toornament.ToornamentClient _toornament;

        public TournamentService(ISessionProvider session, IMemoryCache memoryCache, Toornament.ToornamentClient toornament, ITournamentRepository tournaments, ITeamService teams, IPlayerRepository players, IOrganizationService organizations) : base(memoryCache, session)
        {
            _toornament = toornament;
            _tournaments = tournaments;
            _teams = teams;
            _players = players;
            _organizations = organizations;
        }

        public async Task<(List<Participant> participants, List<Player> players, List<Team> teams)> GetParticipantsAsync(string tournamentId)
        {
            var user = await Session.GetCurrentUser();

            var tournament = await GetTournamentAsync(tournamentId);

            if (user.Id != tournament.ResponsibleId)
            {
                throw new UnauthorizedAccessException("No access, stay away!");
            }

            return new(tournament.Participants, tournament.SignupType.Value == SignupType.Player.Value ? await _players.GetPlayersAsync(tournament.Participants.Select(i => i.Id)) : null, tournament.SignupType.Equals(SignupType.Player) ? await _teams.GetTeamsInTournamentAsync(tournamentId) : null);
        }

        public async Task<Tournament> GetTournamentAsync(string tournamentId)
        {
            return (await GetTournamentsAsync()).FirstOrDefault(t => t.Id == tournamentId) ?? throw new Exception("Tournament not found");
        }

        public async Task<List<Team>> GetTeamsAsync(string tournamentId)
        {
            var teams = await _teams.GetTeamsAsync();

            var tournament = await GetTournamentAsync(tournamentId);

            return tournament.Participants.Select(p => teams.GetValueOrDefault(p.Id)).Where(p => p != null).ToList();
        }

        public Task<List<Tournament>> GetTournamentsAsync()
        {
            return Cache.WithSemaphoreAsync("TOURNAMENTS", async () =>
            {
                return await _tournaments.GetTournamentsAsync();
            }, TimeSpan.FromMinutes(30));
        }
        
        public async Task RegisterAsync(string tournamentId, List<string> information = null)
        {
            var user = await Session.GetCurrentUser();

            var tournament = await GetTournamentAsync(tournamentId);

            if (!tournament.SignupType.Equals(SignupType.Player))
            {
                throw new Exception("Cannot sign up to a tournament without type Player");
            }

            if (tournament.Participants.Any(p => p.Id == user.Id))
            {
                throw new Exception("Already signed up");
            }

            var p = await _toornament.Organizer.AddParticipantAsync(tournamentId, new()
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = user.Id,
                Name = user.DisplayName,
            });

            await _tournaments.AddParticipantAsync(tournamentId, new()
            {
                Id = user.Id,
                Information = information,
                ToornamentId = p.Id,
                Type = ParticipantType.Player,
            });
        }

        public async Task RegisterTeamAsync(string tournamentId, string teamId, List<string> information = null)
        {
            var user = await Session.GetCurrentUser();

            var tournament = await GetTournamentAsync(tournamentId);

            var team = await _teams.GetTeamAsync(teamId);

            if (!tournament.SignupType.Equals(SignupType.Team))
            {
                throw new Exception("Cannot sign up to a tournament without type Team");
            }

            if (tournament.Participants.Any(p => p.Id == teamId))
            {
                throw new Exception("Already signed up");
            }

            if (team.Members.FirstOrDefault(m => m.PlayerId == user.Id)?.Role.Equals(Role.Captain) != true)
            {
                var org = await _organizations.GetOrganizationAsync(team.OrganizationId);
                if (org.Members.FirstOrDefault(m => m.PlayerId == user.Id)?.Role.Strength < 3)
                {
                    throw new Exception("User can't do this");
                }
            }

            var p = await _toornament.Organizer.AddParticipantAsync(tournamentId, new()
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = team.Id,
                Name = team.Name,
            });

            await _tournaments.AddParticipantAsync(tournamentId, new()
            {
                Id = user.Id,
                Information = information,
                ToornamentId = p.Id,
                Type = ParticipantType.Player,
            });
        }
    }
}
