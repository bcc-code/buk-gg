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
        public static Team ToTeam(this SanityTeam team) => new()
        {
            Id = team.Id,
            Name = team.Name,
            CaptainId = team.Captain.Ref,
            GameId = team.Game.Ref,
            OrganizationId = team.Organization.Ref,
            PlayerIds = team.Players.Select(i => i.Ref).ToList()
        };

        public static Tournament ToTournament(this SanityTournament i, SanityHtmlBuilder builder) => new()
        {
            Id = i.Id,
            Title = i.Title,
            Body = i.Body?.ToDictionary(b => b.Key, b => b.ToHtml(builder)) ?? new(),
            CategoryIds = i.Categories?.Select(c => c.Ref).ToList() ?? new(),
            Contacts = i.Contacts?.Select(c => c.ToContact()).ToList() ?? new(),
            GameId = i.Game?.Ref,
            Introduction = i.Introduction?.ToDictionary(i => i.Key, i => i.Value.ToHtml(builder)) ?? new(),
            LiveChat = i.LiveChat,
            LiveStream = i.LiveStream,
            Logo = i.Logo?.Asset?.Value?.Url,
            MainImage = i.MainImage?.Asset?.Value?.Url,
            Platform = i.Platform,
            PlayerIds = i.SoloPlayers?.Select(p => p.Player.Ref).ToList() ?? new(),
            RegistrationOpen = i.RegistrationOpen,
            RequiredInformation = i.RequiredInfo.Select(r => r.ToDictionary(k => k.Key, v => v.Value)).ToList(),
            
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
    }
}
