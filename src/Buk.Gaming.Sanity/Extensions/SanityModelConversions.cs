using Buk.Gaming.Classes;
using Buk.Gaming.Models;
using Buk.Gaming.Sanity.Models;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity.Extensions
{
    public static class SanityModelConversions
    {
        public static SanityOrganization ToSanity(this Organization i) => new()
        {
            Id = i.Id,
            Name = i.Name,
            IsPublic = i.IsPublic,
            Members = i.Members.Select(i => new SanityMember
            {
                Player = new()
                {
                    Ref = i.PlayerId,
                },
                Role = i.Role.ToString(),
            }).ToList(),
            Pending = i.Invitations.Select(i => new SanityInvitation
            {
                Player = new()
                {
                    Ref = i.PlayerId
                },
                Type = i.Type,
            }).ToList(),
        };

        public static SanityTeam ToSanity(this Team i) => new()
        {
            Id = i.Id,
            Captain = new()
            {
                Ref = i.Members?.FirstOrDefault(m => m.Role.Equals(Role.Captain))?.PlayerId ?? throw new Exception("Cannot create a team without a captain"),
            },
            Players = i.Members?.Where(m => !m.Role.Equals(Role.Captain)).Select(i => new SanityReference<SanityPlayer>
            {
                Ref = i.PlayerId,
            }).ToList(),
            Game = new()
            {
                Ref = i.GameId ?? throw new Exception("Team must have a game"),
            }
        };

        public static SanityParticipant ToSanity(this Participant i) => new()
        {
            Information = i.Information,
            Player = i.Type == ParticipantType.Player ? new()
            {
                Ref = i.Id,
            } : null,
            Team = i.Type == ParticipantType.Team ? new()
            {
                Ref = i.Id,
            } : null,
            ToornamentId = i.ToornamentId,
        };
    }
}
