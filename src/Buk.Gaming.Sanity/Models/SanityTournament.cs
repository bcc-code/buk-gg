using Buk.Gaming.Models;
using Sanity.Linq;
using Newtonsoft.Json;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Sanity.Linq.Extensions;

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
            return new TournamentInfo {
                Slug = Slug?.Current,
                MainImage = MainImage?.Asset?.Value?.Url,
                Logo = Logo?.Asset?.Value?.Url,
                Platform = Platform,
                Title = Title?.GetForCurrentCulture(),
                Body = Body?.GetForCurrentCulture(htmlBuilder),
                ToornamentId = ToornamentId,
                RegistrationForm = RegistrationForm?.GetForCurrentCulture(),
                RegistrationOpen = RegistrationOpen,
                TelegramLink = TelegramLink,
                LiveStream = LiveStream,
                LiveChat = LiveChat,
                Game = Game?.Value?.ToGame(),
                Id = Id,
                SignupType = SignupType,
                RequiredInformation = RequiredInfo.Select(r => r.GetForCurrentCulture()).ToList(),
                Teams = Teams.Select(t => t.Team.Value.ToTeam()).ToList(),
                TeamSize = TeamSize,
                CategoryIds = Categories.Select(c => c.Ref).ToList(),
                Winner = Winner?.Value?.Name,
                Contacts = Contacts,
                ResponsibleId = Responsible?.Ref,
                PlayerIds = SoloPlayers.Select(p => p.Player.Ref).ToList(),
            };
        }

        public TournamentAdminInfo ToTournamentAdminInfo(SanityHtmlBuilder htmlBuilder)
        {
            return new TournamentAdminInfo {
                Slug = Slug?.Current,
                MainImage = MainImage?.Asset?.Value?.Url,
                Logo = Logo?.Asset?.Value?.Url,
                Title = Title?.GetForCurrentCulture(),
                Body = Body?.GetForCurrentCulture(htmlBuilder),
                ToornamentId = ToornamentId,
                RegistrationForm = RegistrationForm?.GetForCurrentCulture(),
                RegistrationOpen = RegistrationOpen,
                TelegramLink = TelegramLink,
                LiveStream = LiveStream,
                LiveChat = LiveChat,
                Game = Game?.Value?.ToGame(),
                Id = Id,
                SignupType = SignupType,
                RequiredInformation = RequiredInfo.Select(s => s.GetForCurrentCulture()).ToList(),
                TeamSize =TeamSize,
                SoloPlayers = SoloPlayers.Select(p => p.ToParticipant()).ToList(),
                CategoryIds = Categories.Select(c => c.Ref).ToList(),
                Winner = Winner?.Value?.Name,
                Contacts = Contacts,
                Responsible = Responsible.Value,
                ResponsibleId = Responsible.Ref,
                ParticipantTeams = Teams.Select(t => t.ToParticipant()).ToList(),
            };
        }

    }

    public class TournamentTeam {
        public TournamentTeam() {
            Key = Guid.NewGuid().ToString();
        }

        [JsonProperty("_key")]
        public string Key { get; set; }

        [Include]
        public SanityReference<SanityTeam> Team { get; set; }

        public List<string> Information { get; set; }

        public string ToornamentId { get; set; }

        public Participant<Team> ToParticipant()
        {
            return new Participant<Team>
            {
                Information = Information,
                Item = Team.Value.ToTeam(),
                ToornamentId = ToornamentId,
            };
        }
    }

    public class TournamentPlayer {
        public TournamentPlayer()
        {
            Key = Guid.NewGuid().ToString();
        }
        [JsonProperty("_key")]
        public string Key { get; set; }

        [Include]
        public SanityReference<Player> Player { get; set; }

        public List<string> Information { get; set; }
        
        public string ToornamentId { get; set; }

        public Participant<Player> ToParticipant()
        {
            return new Participant<Player>
            {
                Information = Information,
                Item = Player.Value,
                ToornamentId = ToornamentId
            };
        }
    }
}
