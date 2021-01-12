import http from '@/services/http';
import Vue, { PluginObject } from 'vue';

import { LocaleMessageObject } from 'vue-i18n';
import { Organization, Team } from '@/classes';

export class Api implements PluginObject<any> {
    public localization = {
        getLang(lang: string) {
            return http.get<LocaleMessageObject>(`localization/${lang}`);
        },
    };
    // Session //
    public session = {
        getCurrentSession() {
            return http.get<Session>('session/currentsession');
        },
        updateCurrentUser(user: Player) {
            return http.put<User>('session/currentuser', user);
        },
    };

    // Tournaments //
    public tournaments = {
        getAll() {
            return http.get<TournamentInfo[]>('tournaments');
        },
        getTournament(tournamentId: string) {
            return http.get<TournamentInfo>(`tournaments/${tournamentId}`);
        },
        getStages(toornamentId: string) {
            return http.get<Stage[]>(`tournaments/${toornamentId}/stages`);
        },
        getParticipants(toornamentId: string) {
            return http.get<any[]>(`tournaments/${toornamentId}/participants`);
        },
        getEligibleTeamsForCaptain(tournamentId: string) {
            return http.get<TeamModel[]>(`tournaments/${tournamentId}/captain`);
        },
        addTeamToTournament(tournamentId: string, team: Team, information: string[]) {
            return http.put<boolean>(
                `tournaments/${tournamentId}/teams`,
                {item: team.toModel(), information},
            );
        },
        addPlayerToTournament(tournamentId: string, player: Player, information: string[]) {
            return http.put<boolean>(
                `tournaments/${tournamentId}/players`,
                {item: player, information},
            );
        },
        getAdminInfo(tournamentId: string) {
            return http.get<TournamentAdminInfo>(`tournaments/${tournamentId}/admin`);
        },
    };

    public events = {
        getAll() {
            return http.get<IEvent[]>('events');
        },
        getEvent(eventId: string) {
            return http.get<IEvent>(`events/${eventId}`);
        },
    };

    public discord = {
        searchForMember(search: string) {
            return http.get<any>(`discord/search/${search}`);
        },
        isConnected(id: string) {
            return http.get<boolean>(`discord/connected/${id}`);
        },
    };

    public organizations = {
        uploadImage(organizationId: string, image: any) {
            return http.put<string>(`organizations/${organizationId}/image`, {
                image: image.split(',')[1],
            });
        },
        async leaveOrganization(organizationId: string) {
            return http.delete<boolean>(`organizations/${organizationId}/leave`);
        },
        async saveOrganization(organization: Organization) {
            return new Organization(
                await http.put<OrganizationModel>(
                    `organizations`,
                    organization.toModel(),
                ),
            );
        },
        async createOrganization(organization: Organization) {
            return new Organization(
                await http.put<OrganizationModel>(
                    `organizations/new`,
                    organization.toModel(),
                ),
            );
        },
        async getOrganizations() {
            const orgs: Organization[] = [];
            (
                await http.get<OrganizationModel[]>(`organizations`)
            ).forEach((org) => orgs.push(new Organization(org)));
            return orgs;
        },
        async getOrganization(organizationId: string) {
            return new Organization(
                await http.get<OrganizationModel>(
                    `organizations/${organizationId}`,
                ),
            );
        },
        async getPlayerOrganizations(player: Player, role?: string) {
            const orgs: Organization[] = [];
            (
                await http.post<OrganizationModel[]>(
                    `organizations/player/${role}`,
                    player,
                )
            ).forEach((org) => orgs.push(new Organization(org)));
            return orgs;
        },
        // MEMBERS
        getMember(player: Player) {
            return http.post<Member>(`organizations/members`, player);
        },
        addMember(organizationId: string, member: Member) {
            return http.put<Member>(
                `organizations/${organizationId}/members`,
                member.player,
            );
        },
        updateMember(organizationId: string, member: Member) {
            return http.put<Member>(
                `organizations/${organizationId}/members/update`,
                member,
            );
        },
        deleteMember(organizationId: string, member: Member) {
            return http.delete<boolean>(
                `organizations/${organizationId}/members/${member.player._id}`,
            );
        },
        addPendingMember(organizationId: string, player: Player, type: string) {
            return http.put<boolean>(`organizations/${organizationId}/pending/${type}`, player);
        },
        removePendingMember(organizationId: string, playerId: string) {
            return http.delete<boolean>(`organizations/${organizationId}/pending/${playerId}`);
        },
        // PLAYERS
        searchForPlayers(search: string) {
            return http.get<Player[]>(`organizations/players/${search}`);
        },
    };

    public teams = {
        async getTeamsInOrganization(organizationId: string) {
            const teams: Team[] = [];
            (
                await http.get<TeamModel[]>(`teams/${organizationId}`)
            ).forEach((team) => teams.push(new Team(team)));
            return teams;
        },
        async getTeams() {
            const teams: Team[] = [];
            (await http.get<TeamModel[]>(`teams/`)).forEach((team) => teams.push(new Team(team)));
            return teams;
        },
        async getMyTeams(): Promise<Team[]> {
            const teams: Team[] = [];
            (
                await http.get<TeamModel[]>(
                    `teams/mine`,
                )
            ).forEach((team) => teams.push(new Team(team)));
            return teams;
        },
        async getTeamsInGame(gameId: string) {
            const teams: Team[] = [];
            (
                await http.get<TeamModel[]>(`teams/game/${gameId}`)
            ).forEach((team) => teams.push(new Team(team)));
            return teams;
        },
        async addTeam(team: Team) {
            return new Team(
                await http.put<TeamModel>(`teams/add`, team.toModel()),
            );
        },
        async updateTeam(team: Team) {
            return new Team(
                (await http.put<SanityResult<TeamModel>>(`teams/update`, team.toModel())).item,
            );
        },
        deleteTeam(team: Team) {
            return http.post<boolean>(`teams/delete`, team.toModel());
        },
        // GAMES
        getGames() {
            return http.get<Game[]>(`teams/games`);
        },
    };

    // Installer
    public install(vue: typeof Vue, options?: any) {
        vue.prototype.$api = this;
    }
}

const api = new Api();

export default api;
