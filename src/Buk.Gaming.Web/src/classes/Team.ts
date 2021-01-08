import { Organization } from './Organization';
import api from '@/services/api';

export class Team {
    private Id: string;
    private Name: string;
    private Captain: Player;
    private Players: Player[];
    private Game: Game;
    private Organization: Organization;

    constructor(team?: TeamModel) {
        this.Id = team?._id || null;
        this.Name = team?.name || '';
        this.Captain = team?.captain || null;
        this.Players = team?.players || [];
        this.Organization = team?.organization ? new Organization(team?.organization) : {} as Organization;
        this.Game = team?.game || null;
    }

    public toModel(): TeamModel {
        return {
            _id: this.Id,
            name: this.Name,
            captain: this.Captain,
            players: this.Players,
            organization: this.Organization.toModel(),
            game: this.Game,
        };
    }

    // METHODS
    public delete() {
        return api.teams.deleteTeam(this);
    }

    public save() {
        return api.teams.updateTeam(this);
    }

    public create() {
        return api.teams.addTeam(this);
    }

    public addPlayer(player: Player) {
        if (this.Players.find((p) => p._id === player._id)) {
            return;
        } else {
            this.Players.push(player);
        }
    }

    public removePlayer(player: Player) {
        this.Players = this.Players.filter((p) => p._id !== player._id);
    }

    public setCaptain(player: Player) {
        if (this.Captain) {
            this.Players.push(this.Captain);
        }
        this.Captain = player;
        this.Players = this.Players.filter((p) => p._id !== this.Captain._id);
    }

    public setGame(game: Game) {
        this.Game = game;
    }

    public setOrganization(organization: Organization) {
        this.Organization = organization;
    }

    // GETTERS
    public get id() {
        return this.Id;
    }

    public get name() {
        return this.Name;
    }

    public set name(value: string) {
        this.Name = value;
    }

    public get captain() {
        return this.Captain;
    }

    public get players() {
        const allPlayers = [];
        if (this.captain) {
            allPlayers.push(this.captain);
        }
        this.Players.forEach((p) => allPlayers.push(p));
        return allPlayers;
    }

    public get gameId() {
        return this.Game._id;
    }

    public get icon() {
        return this.Game.icon;
    }

    public get organizationId() {
        return this.Organization?.id;
    }

    public get organizationIcon() {
        return this.Organization.image;
    }

    public get isPublic() {
        return this.Organization.isPublic;
    }
}
