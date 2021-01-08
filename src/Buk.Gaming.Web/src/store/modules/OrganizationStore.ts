import api from '@/services/api';
import { RootStore, organizations } from '@/store';
import CrudStore, { CrudState } from './base/CrudStore';
import { Organization } from '@/classes';

export interface OrganizationState extends CrudState<Organization, string> {
    playerOrganizations: Organization[];
}

export class OrganizationStore extends CrudStore<
    Organization,
    OrganizationState,
    string
> {
    constructor(rootStore: RootStore) {
        super('organizations', rootStore, {
            all: [],
            item: {},
            currentId: '',
            playerOrganizations: [],
        });

        this.mutations = {
            ...this.mutations,
            setPlayerOrganizations: (
                state: OrganizationState,
                items: Organization[],
            ) => {
                state.playerOrganizations = items;
            },
            leaveOrganization: (state, organizationId: string) => {
                state.playerOrganizations = state.playerOrganizations.filter((o) => o.id !== organizationId);
                this.current.removeMember(this.rootStore.state.session.currentUser._id);
            },
            setData: (state, data: { name: string }) => {
                this.current.name = data?.name || this.current.name;
            },
            addMember: (state, member: Member) => {
                this.current?.addMember(member);
            },
            updateMember: (state, member: Member) => {
                this.current?.updateMember(member);
            },
            removeMember: (state, playerId: string) => {
                this.current?.removeMember(playerId);
            },
            joinRequest: (state, player: Player) => {
                this.current?.addPendingMember(player, 'request');
            },
            inviteMember: (state, player: Player) => {
                this.current?.addPendingMember(player, 'invite');
            },
            removePendingMember: (state, playerId: string) => {
                this.current?.removePendingMember(playerId);
            },
            updateImage: (state, image: string) => {
                this.current?.setImage(image);
            },
        };

        this.actions = {
            ...this.actions,
            leaveOrganization: async ({ commit }, organizationId: string) => {
                const result = await api.organizations.leaveOrganization(organizationId);
                if (result) {
                    this.commit('leaveOrganization', organizationId);
                }
                return result;
            },
            addMember: async ({ commit }, member: Member) => {
                const result = await api.organizations.addMember(
                    this.current.id,
                    member,
                );
                if (result) {
                    this.commit('addMember', member);
                    this.commit('removePendingMember', member.player._id);
                }
                return result;
            },
            joinRequest: async ({ commit }, player: Player) => {
                if (!this.currentId) { return false; }
                const result = await api.organizations.addPendingMember(this.currentId, player, 'request');
                if (result) {
                    this.commit('joinRequest', player);
                }
                return result;
            },
            inviteMember: async ({ commit }, player: Player) => {
                if (!this.currentId) { return false; }
                const result = await api.organizations.addPendingMember(this.currentId, player, 'invite');
                if (result) {
                    this.commit('inviteMember', player);
                }
                return result;
            },
            removePendingMember: async ({ commit }, playerId: string) => {
                if (!this.currentId) { return false; }
                const result = await api.organizations.removePendingMember(this.currentId, playerId);
                if (result) {
                    this.commit('removePendingMember', playerId);
                }
                return result;
            },
            updateMember: async ({ commit }, member: Member) => {
                const result = await api.organizations.updateMember(
                    this.current.id,
                    member,
                );
                if (result.player?._id) {
                    this.commit('updateMember', member);
                }
                return result;
            },
            removeMember: async ({ commit }, member: Member) => {
                const result = await api.organizations.deleteMember(
                    this.current.id,
                    member,
                );
                if (result) {
                    this.commit('removeMember', member.player._id);
                }
                return result;
            },
            saveOrganization: async ({ commit }, organizationId: string) => {
                return this.state.all
                    .find((o) => o.id === organizationId)
                    ?.save();
            },
            createOrganization: ({ commit }, organization: Organization) => {
                return api.organizations.createOrganization(organization);
            },
            loadOrganization: async ({ commit }, organizationId: string) => {
                const org = this.state.all.find((o) => o.id === organizationId);
                if (org) {
                    this.setCurrent(org.id);
                    return org;
                } else {
                    const result = await api.organizations.getOrganization(
                        organizationId,
                    );
                    if (result?.id) {
                        this.updateItem(result);
                        this.setCurrent(result.id);
                        return result;
                    }
                }
                return {};
            },
            loadAll: async ({ commit }) => {
                this.setAll(await api.organizations.getOrganizations());
                this.commit('setPlayerOrganizations', await api.organizations.getPlayerOrganizations(this.rootStore.state.session.currentUser));
            },
            getTeams: async ({ commit }, organizationId: string) => {
                const items = await api.teams.getTeamsInOrganization(organizationId);
                return items;
            },
            uploadImage: async ({ commit }, image: string) => {
                const result = await api.organizations.uploadImage(
                    this.currentId,
                    image,
                );
                if (result) {
                    this.commit('updateImage', result);
                }
                return result;
            },
        };

        // GETTERS //
        this.getters = {
            ...this.getters,
        };
    }

    // STRAIGHT FROM API
    public searchForPlayers = (searchString: string) => {
        return api.organizations.searchForPlayers(searchString);
    }

    // MEMBER MANAGEMENT
    public leaveOrganization = (organizationId: string) => {
        return this.dispatch('leaveOrganization', organizationId) as Promise<boolean>;
    }

    public getMember = (player: Player) => {
        return api.organizations.getMember(player);
    }

    public addMember = (member: Member) => {
        return this.dispatch('addMember', member) as Promise<Member>;
    }

    public updateMember = (member: Member) => {
        return this.dispatch('updateMember', member) as Promise<Member>;
    }

    public joinRequest = (player: Player) => {
        return this.dispatch('joinRequest', player) as Promise<any>;
    }

    public removeMember = (member: Member) => {
        return this.dispatch('removeMember', member) as Promise<boolean>;
    }

    public removePendingMember = (playerId: string) => {
        return this.dispatch('removePendingMember', playerId) as Promise<boolean>;
    }

    public getTeams = (organizationId: string) => {
        this.dispatch('getTeams', organizationId);
    }

    public loadOrganization = (organizationId: string) => {
        return this.dispatch('loadOrganization', organizationId) as Promise<Organization>;
    }

    // GENERAL INFO
    public uploadImage = (image: string) => {
        return this.dispatch('uploadImage', image) as Promise<string>;
    }

    public setData = (data: { name: string }) => {
        return this.commit('setData', data);
    }

    protected getItemId(item?: object | Organization | undefined) {
        if (item) {
            return (item as Organization).id;
        }
    }
    protected loadAllFromSource(): Promise<Organization[]> {
        return api.organizations.getOrganizations();
    }
}
