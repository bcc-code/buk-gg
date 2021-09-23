using Buk.Gaming.Models;
using Buk.Gaming.Models.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Buk.Gaming.Extensions
{
    public static class Views
    {
        public static MemberView View(this Member i, Player player = null) => new()
        {
            PlayerId = i.PlayerId,
            Role = i.Role.ToString(),
            Player = player?.View(),
        };

        public static TeamView View(this Team i, Dictionary<string, Player> players = null) => new()
        {
            Id = i.Id,
            Name = i.Name,
            OrganizationId = i.OrganizationId,
            Members = i.Members?.Select(m => m.View(players?.GetValueOrDefault(m.PlayerId))).ToList(),
        };

        public static PlayerView View(this Player i) => new()
        {
            DiscordTag = i.DiscordUser,
            DisplayName = i.DisplayName,
            Id = i.Id,
        };

        public static PlayerAdminView AdminView(this Player i) => new()
        {
            Id = i.Id,
            DiscordTag = i.DiscordUser,
            DisplayName = i.DisplayName,
            Name = i.Name,
            Email = i.Email,
            Location = i.Location,
            PhoneNumber = i.PhoneNumber,
        };

        public static OrganizationView View(this Organization i, Dictionary<string, Player> players = null, List<Team> teams = null) => new()
        {
            Id = i.Id,
            Name = i.Name,
            Logo = i.Image,
            Members = i.Members.Select(m => m.View(players?.GetValueOrDefault(m.PlayerId))).ToList(),
            Teams = teams?.Select(t => t.View(players)).ToList(),
            Invitations = i.Invitations.Select(m => m.View()).ToList(),
        };

        public static InvitationView View(this Invitation i, Player player = null) => new()
        {
            Player = player?.View(),
            PlayerId = i.PlayerId,
            Type = i.Type.ToString(),
        };

        public static BaseItemView BaseView(this BaseItem i) => new()
        {
            Id = i.Id,
            Name = i.Name.GetForCurrentCulture(),
        };

        public static ContactView View(this Contact i) => new()
        {
            Discord = i.Discord,
            Email = i.Email,
            Name = i.Name,
            PhoneNumber = i.PhoneNumber,
        };

        public static BaseTournamentView BaseView(this Tournament i, string userId) => i.View(userId);

        public static TournamentView View(this Tournament i, string userId, List<Team> teams = null) => new()
        {
            Id = i.Id,
            LiveStream = i.LiveStream,
            Logo = i.Logo,
            Title = i.Title.GetForCurrentCulture(),
            SignedUp = i.Participants.Any(p => p.Type == ParticipantType.Player && p.Id == userId),
            Responsible = i.ResponsibleId == userId,
            Slug = i.Slug,
            Body = i.Body.GetForCurrentCulture(),
            Contacts = i.Contacts.Select(c => c.View()).ToList(),
            MaxPlayers = i.TeamSize.Max,
            MinPlayers = i.TeamSize.Min,
            RegistrationOpen = i.RegistrationOpen,
            RequiredInfo = i.RequiredInformation.Select(r => r.GetForCurrentCulture()).ToList(),
            SignupType = i.SignupType.ToString(),
            Teams = teams?.Select(t => t.View()).ToList(),
            Winner = teams?.FirstOrDefault(t => t.Id == i.WinnerId)?.View(),
            TelegramLink = i.TelegramLink,
            ToornamentId = i.ToornamentId,
            Platform = i.Platform,
        };

        public static ParticipantView View(this Participant i, Team team = null, Dictionary<string, Player> players = null) => new()
        {
            Id = i.Id,
            Information = i.Information,
            ToornamentId = i.ToornamentId,
            Type = i.Type.ToString(),
            Team = team?.View(players),
        };

        public static ParticipantView View(this Participant i, List<Player> players) => new()
        {
            Id = i.Id,
            Information = i.Information,
            ToornamentId = i.ToornamentId,
            Type = i.Type.ToString(),
            Players = players.Select(i => i.AdminView()).ToList(),
        };
    }
}
