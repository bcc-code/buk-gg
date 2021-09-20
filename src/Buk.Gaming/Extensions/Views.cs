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

        public static OrganizationView View(this Organization i, Dictionary<string, Player> players = null, List<Team> teams = null) => new()
        {
            Id = i.Id,
            Name = i.Name,
            Logo = i.Image,
            Members = i.Members.Select(m => m.View(players?.GetValueOrDefault(m.PlayerId))).ToList(),
            Teams = teams?.Select(t => t.View(players)).ToList(),
            Invitations = i.Invitations?.Select(m => m.View()).ToList(),
        };

        public static InvitationView View(this Invitation i, Player player = null) => new()
        {
            Player = player?.View(),
            PlayerId = i.PlayerId,
            Type = i.Type,
        };
    }
}
