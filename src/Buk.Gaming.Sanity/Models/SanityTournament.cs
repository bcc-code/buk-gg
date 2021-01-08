using Buk.Gaming.Models;
using Sanity.Linq;
using Newtonsoft.Json;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityTournament : SanityDocument
    {
        public SanityTournament()
        {
            SanityType = "tournament";
        }

        public SanityLocaleString Title { get; set;}

        public SanitySlug Slug { get; set; }

        public SanityReference<SanityCamp> Camp { get; set; }

        public string Platform { get; set; }

        public List<SanityReference<SanityCategory>> Categories { get; set; }

        [Include]
        public SanityReference<Player> Responsible { get; set; }

        [Include]
        public SanityImage MainImage { get; set; }

        [Include]
        public SanityImage Logo { get; set; }

        public SanityLocaleBlock Introduction { get; set; }

        public SanityLocaleBlock Body { get; set; }

        public SanityLocaleString   RegistrationForm { get; set;}

        public bool RegistrationOpen { get; set; }

        public string ToornamentId { get; set; }

        public string TelegramLink { get; set; }

        public string LiveStream { get; set; }

        public bool LiveChat { get; set; }
        
        [Include]
        public SanityReference<SanityGame> Game { get; set; }

        public string SignupType { get; set; }

        [Include]
        public SanityReference<SanityTeam> Winner { get; set; }

        public List<SanityLocaleString> RequiredInfo { get; set; }

        [Include]
        public List<TournamentPlayer> SoloPlayers { get; set; }

        [Include]
        public List<TournamentTeam> Teams { get; set; }

        public TeamSize TeamSize { get; set; }

        public List<Contact> Contacts { get; set; }

        public TournamentInfo ToTournamentInfo(SanityHtmlBuilder htmlBuilder)
        {
            int teamCount = this.Teams != null ? this.Teams.Count : 0;
            var teams = new Team[teamCount];

            if (teams.Length > 0) {
                for (int i = 0; i < this.Teams.Count; i++)
                {
                    var captain = this.Teams[i].Team.Value?.Captain.Value;
                    teams[i] = new Team();
                    teams[i].Name = this.Teams[i].Team.Value?.Name;
                    teams[i].Id = this.Teams[i].Team.Ref;
                    teams[i].Organization = new Organization() {
                        Id = this.Teams[i].Team.Value?.Organization.Ref,
                        IsPublic = this.Teams[i].Team.Value?.Organization.Value?.IsPublic == true,
                    };
                    teams[i].Captain = new Player() {
                        Id = captain.Id,
                        Name = captain.Name,
                        Nickname = captain.Nickname,
                        DiscordId = captain.DiscordId,
                        DiscordUser = captain.DiscordUser,
                    };
                }
            }

            var categories = new string[this.Categories != null ? this.Categories.Count : 0];

            if (categories.Length > 0)
            {
                for (int i = 0; i < this.Categories.Count; i++)
                {
                    categories[i] = this.Categories[i].Ref;
                }
            }

            string[] playerIds = new string[this.SoloPlayers != null ? this.SoloPlayers.Count : 0];

            for (int i = 0; i < playerIds.Length; i++)
            {
                playerIds[i] = this.SoloPlayers[i].Player.Ref;
            }

            var requiredInfo = new string[this.RequiredInfo != null ? this.RequiredInfo.Count : 0];

            if (requiredInfo.Length > 0)
            {
                for (int i = 0; i < this.RequiredInfo.Count; i++)
                {
                    requiredInfo[i] = this.RequiredInfo[i].GetForCurrentCulture();
                }
            }

            return new TournamentInfo {
                Slug = this.Slug?.Current,
                MainImage = this.MainImage?.Asset?.Value?.Url,
                Logo = this.Logo?.Asset?.Value?.Url,
                Platform = this.Platform,
                Title = this.Title?.GetForCurrentCulture(),
                // Introduction = this.Introduction?.GetForCurrentCulture(htmlBuilder),
                Body = this.Body?.GetForCurrentCulture(htmlBuilder),
                ToornamentId = this.ToornamentId,
                RegistrationForm = this.RegistrationForm?.GetForCurrentCulture(),
                RegistrationOpen = this.RegistrationOpen,
                TelegramLink = this.TelegramLink,
                LiveStream = this.LiveStream,
                LiveChat = this.LiveChat,
                Game = this.Game?.Value?.ToGame(),
                Id = this.Id,
                SignupType = this.SignupType,
                RequiredInformation = requiredInfo,
                Teams = teams,
                TeamSize = this.TeamSize,
                CategoryIds = categories,
                Winner = this.Winner?.Value?.Name,
                Contacts = this.Contacts?.ToArray(),
                ResponsibleId = this.Responsible?.Ref,
                PlayerIds = playerIds,
            };
        }

        public TournamentAdminInfo ToTournamentAdminInfo(SanityHtmlBuilder htmlBuilder)
        {
            int teamCount = this.Teams != null ? this.Teams.Count : 0;
            var teams = new Participant<Team>[teamCount];

            if (teams.Length > 0) {
                for (int i = 0; i < this.Teams.Count; i++)
                {
                    var captain = this.Teams[i].Team.Value?.Captain.Value;
                    teams[i] = new Participant<Team>();
                    teams[i].Item = new Team();
                    teams[i].Item.Name = this.Teams[i].Team.Value?.Name;
                    teams[i].Item.Id = this.Teams[i].Team.Ref;
                    teams[i].Item.Organization = new Organization() {
                        Id = this.Teams[i].Team.Value?.Organization.Ref,
                        IsPublic = this.Teams[i].Team.Value?.Organization.Value?.IsPublic == true,
                        Name = this.Teams[i].Team.Value?.Organization.Value?.Name,
                    };
                    teams[i].Item.Captain = this.Teams[i].Team.Value?.Captain?.Value;
                    
                    teams[i].Item.Players = new Player[this.Teams[i].Team.Value.Players.Count];
                    for (int x = 0; x < this.Teams[i].Team.Value?.Players.Count; x++) 
                    {
                        teams[i].Item.Players[x] = this.Teams[i].Team.Value?.Players[x]?.Value;
                    }
                    teams[i].Information = this.Teams[i].Information.ToArray();
                    teams[i].ToornamentId = this.Teams[i].ToornamentId;
                }
            }

            int playerCount = this.SoloPlayers != null ? this.SoloPlayers.Count : 0;

            var players = new Participant<Player>[playerCount];

            if (players.Length > 0)
            {
                for (int i = 0; i < this.SoloPlayers.Count; i++)
                {
                    players[i] = new Participant<Player>();
                    players[i].Item = this.SoloPlayers[i].Player.Value;
                    players[i].Information = this.SoloPlayers[i].Information.ToArray();
                    players[i].ToornamentId = this.SoloPlayers[i].ToornamentId;
                }
            }

            var categories = new string[this.Categories != null ? this.Categories.Count : 0];

            if (categories.Length > 0)
            {
                for (int i = 0; i < this.Categories.Count; i++)
                {
                    categories[i] = this.Categories[i].Ref;
                }
            }

            var requiredInfo = new string[this.RequiredInfo != null ? this.RequiredInfo.Count : 0];

            if (requiredInfo.Length > 0)
            {
                for (int i = 0; i < this.RequiredInfo.Count; i++)
                {
                    requiredInfo[i] = this.RequiredInfo[i].GetForCurrentCulture();
                }
            }

            return new TournamentAdminInfo {
                Slug = this.Slug?.Current,
                MainImage = this.MainImage?.Asset?.Value?.Url,
                Logo = this.Logo?.Asset?.Value?.Url,
                Title = this.Title?.GetForCurrentCulture(),
                // Introduction = this.Introduction?.GetForCurrentCulture(htmlBuilder),
                Body = this.Body?.GetForCurrentCulture(htmlBuilder),
                ToornamentId = this.ToornamentId,
                RegistrationForm = this.RegistrationForm?.GetForCurrentCulture(),
                RegistrationOpen = this.RegistrationOpen,
                TelegramLink = this.TelegramLink,
                LiveStream = this.LiveStream,
                LiveChat = this.LiveChat,
                Game = this.Game?.Value?.ToGame(),
                Id = this.Id,
                SignupType = this.SignupType,
                RequiredInformation = requiredInfo,
                TeamSize = this.TeamSize,
                SoloPlayers = players,
                CategoryIds = categories,
                Winner = this.Winner?.Value?.Name,
                Contacts = this.Contacts?.ToArray(),
                Responsible = this.Responsible.Value,
                ResponsibleId = this.Responsible.Ref,
                ParticipantTeams = teams,
            };
        }

    }

    public class TournamentTeam {
        public TournamentTeam() {
            this.Key = Guid.NewGuid().ToString();
        }

        [JsonProperty("_key")]
        public string Key { get; set; }

        [Include]
        public SanityReference<SanityTeam> Team { get; set; }

        public List<string> Information { get; set; }

        public string ToornamentId { get; set; }
    }

    public class TournamentPlayer {
        public TournamentPlayer()
        {
            this.Key = Guid.NewGuid().ToString();
        }
        [JsonProperty("_key")]
        public string Key { get; set; }

        [Include]
        public SanityReference<Player> Player { get; set; }

        public List<string> Information { get; set; }
        
        public string ToornamentId { get; set; }
    }
}
