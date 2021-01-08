import api from '@/services/api';
import { RootStore } from '@/store';
import CrudStore, { CrudState } from './base/CrudStore';
import { Team } from '@/classes';

export interface TournamentState extends CrudState<TournamentInfo, string> {}

export class TournamentStore extends CrudStore<
    TournamentInfo,
    TournamentState,
    string
> {
    constructor(rootStore: RootStore) {
        super('tournaments', rootStore, {
            all: [],
            item: {},
            currentId: '',
        });

        this.mutations = {
            ...this.mutations,
            setAll: (state: TournamentState, items: TournamentInfo[]) => {
                state.all = items;
            },
            addTeamToTournament: (state: TournamentState, item: TeamModel) => {
                this.current.teams.push(item);
            },
            addPlayerToTournament: (state: TournamentState, item: string) => {
                this.current.playerIds.push(item);
            },
        };

        this.actions = {
            ...this.actions,
            loadTournament: async (
                store,
                tournamentId: string,
            ): Promise<TournamentInfo> => {
                const tournament = store.state.all.find((t) => t.id === tournamentId);
                if (tournament?.id) {
                    this.setCurrent(tournament.id);
                    return tournament;
                } else {
                    const item = await api.tournaments.getTournament(tournamentId);

                    item.stages = item.toornamentId
                        ? await api.tournaments.getStages(item.toornamentId)
                        : undefined;

                    this.updateItem(item);
                    this.setCurrent(item.id);
                    return item;
                }
            },
            addTeamToTournament: async (
                { commit },
                object: { tournamentId: string; team: Team; information: string[] },
            ): Promise<boolean> => {
                const result = await api.tournaments.addTeamToTournament(
                    object.tournamentId,
                    object.team,
                    object.information,
                );
                if (result) {
                    this.commit('addTeamToTournament', object.team);
                }
                return result;
            },
            addPlayerToTournament: async (
                { commit },
                object: {
                    tournamentId: string;
                    player: Player;
                    information: string[];
                },
            ): Promise<boolean> => {
                const result = await api.tournaments.addPlayerToTournament(
                    object.tournamentId,
                    object.player,
                    object.information,
                );
                if (result) {
                    this.commit('addPlayerToTournament', object.player._id);
                }
                return result;
            },
        };

        // GETTERS //
        this.getters = {
            ...this.getters,
        };
    }

    public loadTournament = (tournamentId: string) =>
        this.dispatch('loadTournament', tournamentId) as Promise<
            TournamentInfo
        >

    public addTeamToTournament = (object: {
        tournamentId: string;
        team: Team;
        information: string[];
    }) => this.dispatch('addTeamToTournament', object) as Promise<boolean>

    public addPlayerToTournament = (tournamentId: string, player: Player, information: string[]) =>
        this.dispatch('addPlayerToTournament', {
            tournamentId,
            player,
            information,
        }) as Promise<boolean>

    protected getItemId(item?: object | Tournament | undefined) {
        if (item) {
            return (item as Tournament).id;
        }
    }
    protected loadAllFromSource(): Promise<TournamentInfo[]> {
        return api.tournaments.getAll();
    }
}
