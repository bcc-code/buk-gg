using Buk.Gaming.Classes;
using Buk.Gaming.Extensions;
using Buk.Gaming.Models;
using Buk.Gaming.Sanity.Models;
using Sanity.Linq.BlockContent;
using Sanity.Linq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buk.Gaming.Sanity.Extensions
{
    public static class ModelConversions
    {
        public static Team ToTeam(this SanityTeam team)
        {
            MemberList members = new();

            members.Add(new()
            {
                Role = Role.Captain,
                PlayerId = team.Captain?.Ref ?? throw new Exception("Team has no captain? " + team.Name),
            });

            members.AddRange(team.Players.Select(p => new Member
            {
                Role = Role.Member,
                PlayerId = p.Ref ?? throw new Exception("PlayerId not found??")
            }));


            return new()
            {
                Id = team.Id,
                Name = team.Name,
                GameId = team.Game.Ref,
                OrganizationId = team.Organization.Ref,
                Members = members,
            };
        }

        public static LocaleDictionary ToLocaleDictionary(this SanityLocaleString i)
        {
            var result = new LocaleDictionary();

            foreach (var e in i)
            {
                result[e.Key] = e.Value;
            }

            return result;
        }

        public static LocaleDictionary ToLocaleDictionary(this SanityLocaleBlock i, SanityHtmlBuilder builder)
        {
            var result = new LocaleDictionary();

            foreach (var e in i)
            {
                result[e.Key] = e.Value.ToHtml(builder);
            }

            return result;
        }

        public static Tournament ToTournament(this SanityTournament i, SanityHtmlBuilder builder) => new()
        {
            Id = i.Id,
            Title = i.Title?.ToLocaleDictionary() ?? new(),
            Body = i.Body?.ToLocaleDictionary(builder) ?? new(),
            CategoryIds = i.Categories?.Select(c => c.Ref).ToList() ?? new(),
            Contacts = i.Contacts?.Select(c => c.ToContact()).ToList() ?? new(),
            GameId = i.Game?.Ref,
            Introduction = i.Introduction?.ToLocaleDictionary(builder) ?? new(),
            LiveChat = i.LiveChat,
            LiveStream = i.LiveStream,
            Logo = i.Logo?.Asset?.Value?.Url,
            MainImage = i.MainImage?.Asset?.Value?.Url,
            Platform = i.Platform,
            RegistrationOpen = i.RegistrationOpen,
            RequiredInformation = i.RequiredInfo.Select(r => r.ToLocaleDictionary()).ToList(),
            SignupType = SignupType.Validate(i.SignupType),
            ResponsibleId = i.Responsible?.Ref,
            Slug = i.Slug?.Current,
            TeamSize = i.TeamSize,
            ToornamentId = i.ToornamentId,
            TelegramLink = i.TelegramLink,
            WinnerId = i.Winner?.Ref,
            Participants = i.SignupType == "team" ? i.Teams.Select(t => t.ToParticipant()).ToList() : i.SoloPlayers.Select(s => s.ToParticipant()).ToList(),
        };

        public static Contact ToContact(this SanityContact i) => new()
        {
            Discord = i.Discord,
            Email = i.Email,
            Name = i.Name,
            PhoneNumber = i.PhoneNumber,
        };

        public static Player ToPlayer(this SanityPlayer i) => new()
        {
            Id = i.Id,
            DateLastActive = i.DateLastActive,
            DateRegistered = i.DateRegistered,
            DiscordId = i.DiscordId,
            DiscordIsConnected = i.DiscordIsConnected,
            DiscordUser = i.DiscordUser,
            Email = i.Email,
            EnableMoreDiscords = i.EnableMoreDiscords,
            IsO18 = i.IsO18,
            Location = i.Location,
            MoreDiscordUsers = i.MoreDiscordUsers.Select(m => new Player.ExtraDiscordUser
            {
                DiscordId = m.DiscordId,
                Name = m.Name,
            }).ToList(),
            Name = i.Name,
            Nickname = i.Nickname,
            NoNbIsStandard = i.NoNbIsStandard,
            PersonId = i.PersonId,
            PhoneNumber = i.PhoneNumber,
        };

        public static Invitation ToInvitation(this SanityInvitation i) => new()
        {
            PlayerId = i.Player?.Ref,
            Type = InvitationType.Validate(i.Type),
        };

        public static Member ToMember(this SanityMember i) => new()
        {
            PlayerId = i.Player?.Ref,
            Role = Role.Validate(i.Role, false),
        };

        public static Organization ToOrganization(this SanityOrganization i) => new()
        {
            Id = i.Id,
            Image = i.Image?.Asset?.Value?.Url,
            Invitations = i.Pending?.Select(p => p.ToInvitation()).ToList(),
            IsPublic = i.IsPublic,
            Members = i.Members?.Select(m => m.ToMember()).ToMemberList(),
            Name = i.Name,
        };

        public static Game ToGame(this SanityGame i) => new()
        {
            Id = i.Id,
            HasTeams = i.HasTeams,
            Icon = i.Icon?.Asset?.Value?.Url,
            Name = i.Name,
        };

        public static Category ToCategory(this SanityCategory i) => new()
        {
            Id = i.Id,
            Name = i.Title?.ToLocaleDictionary(),
        };

        public static Camp ToCamp(this SanityCamp i) => new()
        {
            Id = i.Id,
            Name = i.Title?.ToLocaleDictionary(),
        };

        public static Participant ToParticipant(this SanityParticipant i) => new()
        {
            Type = i.Player != null ? ParticipantType.Player : ParticipantType.Team,
            Id = i.Player?.Ref ?? i.Team?.Ref,
            Information = i.Information,
            ToornamentId = i.ToornamentId,
        };
    }
}
