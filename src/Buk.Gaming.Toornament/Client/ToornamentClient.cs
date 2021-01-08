using Buk.Gaming.Toornament.Dtos;
using Buk.Gaming.Toornament.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Buk.Gaming.Toornament
{
    public class ToornamentClient
    {
        protected ToornamentOptions Options { get; }
        protected ToornamentApiRequestHandler Api { get; }

        protected static ConcurrentDictionary<string, HttpClient> HttpClients { get; set; } = new ConcurrentDictionary<string, HttpClient>();

        protected HttpClient HttpClient { get; set; }

        public ToornamentClient(ToornamentOptions options, string scopes = ToornamentScopes.Organizer.All)
        {
            Options = options;
            HttpClient = HttpClients.GetOrAdd(options.ClientId + "_" + scopes, new ToornamentHttpClient(options, scopes));
            Organizer = new ToornamentOrganizerClient(HttpClient);
        }

        public ToornamentOrganizerClient Organizer { get;  }

        public class ToornamentOrganizerClient
        {
            ToornamentApiRequestHandler Api { get; }
            public ToornamentOrganizerClient(HttpClient httpClient)
            {
                Api = new ToornamentApiRequestHandler(httpClient, "https://api.toornament.com/organizer/v2");
            }

            #region Tournaments

            public Task<List<Tournament>> GetTournamentsAsync(TournamentQueryOptions query = null, int take = 50, int skip = 0)
            {
                return Api.GetAsync<List<Tournament>>("tournaments", "tournaments", query, take, skip);
            }

            public Task<Tournament> CreateTournamentAsync(Tournament tournament)
            {
                return Api.PostAsync<Tournament>("tournaments");
            }

            public Task<Tournament> GetTournamentAsync(string id)
            {
                return Api.GetSingleAsync<Tournament>($"tournaments/{Uri.EscapeDataString(id)}");
            }

            public Task<List<Participant>> GetParticipantsAsync(string id) 
            {
                return Api.GetAsync<List<Participant>>($"tournaments/{Uri.EscapeDataString(id)}/participants", "participants", null, 50, 0);
            }

            public async Task<Participant[]> SyncParticipantsAsync(string id, Participant[] participants)
            {
                for (int i = 0; i < participants.Length; i++) {
                    if (string.IsNullOrEmpty(participants[i].Id))
                    {
                        participants[i] = await Api.PostAsync<Participant>($"tournaments/{Uri.EscapeDataString(id)}/participants", participants[i]);
                    } else {
                        await Api.PatchAsync<Participant, Participant>($"tournaments/{Uri.EscapeDataString(id)}/participants/{participants[i].Id}", participants[i]);
                    }
                }
                return participants;
            }

            public async Task<Participant> AddParticipantAsync(string id, Participant participant)
            {
                
                if (string.IsNullOrEmpty(participant.Id))
                {
                    participant = await Api.PostAsync<Participant>($"tournaments/{Uri.EscapeDataString(id)}/participants", participant);
                } else {
                    await Api.PatchAsync<Participant, Participant>($"tournaments/{Uri.EscapeDataString(id)}/participants/{participant.Id}", participant);
                }
                return participant;
            }

            public Task<List<Stage>> GetStagesAsync(string tournamentId)
            {
                return Api.GetAsync<List<Stage>>($"tournaments/{Uri.EscapeDataString(tournamentId)}/stages", "");
            }

            public Task<Tournament> PatchTournamentAsync<TPatch>(string id, TPatch patch)
            {
                return Api.PatchAsync<Tournament, TPatch>($"tournaments/{Uri.EscapeDataString(id)}");
            }

            public Task DeleteTournamentAsync<TPatch>(string id)
            {
                return Api.DeleteAsync<object>($"tournaments/{Uri.EscapeDataString(id)}");
            }

            #endregion


        }
        
    }

    
}
