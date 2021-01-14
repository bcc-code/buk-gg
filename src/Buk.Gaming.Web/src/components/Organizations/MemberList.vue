<template>
    <div class="org-members">
        <div class="list">
            <div class="member-list">
                <base-table
                    class="col-12"
                    :data="members"
                    :columns="['id', 'nickname', 'discordUser']"
                    :style="pending.lenght > 0 ? 'height:50%' : ''"
                >
                    <template slot="columns">
                        <th>Name</th>
                        <th v-if="!edit">Discord</th>
                        <th>{{ !edit ? 'Role' : 'Edit'}}</th>
                    </template>
                    <template slot-scope="{ row }">
                        <td :style="rowStyle[row.role]">{{ row.player.nickname }}</td>
                        <td v-if="!edit" :style="rowStyle[row.role]">{{ row.player.discordUser }}</td>
                        <td :style="rowStyle[row.role]">
                            <base-dropdown
                                v-if="edit && row.role !== 'owner' && loading.selectRole != row.player._id && $session.state.currentUser._id !== row.player._id"
                                title-classes="btn btn-primary btn-simple td-buttons"
                                :title="row.role.toUpperCase()"
                            >
                                <a
                                    class="dropdown-item"
                                    @click="setRole(row.player, 'officer')"
                                    style="cursor: pointer;"
                                    >OFFICER</a
                                >
                                <a
                                    class="dropdown-item"
                                    @click="setRole(row.player, 'captain')"
                                    style="cursor: pointer;"
                                    >CAPTAIN</a
                                >
                                <a
                                    class="dropdown-item"
                                    @click="setRole(row.player, 'member')"
                                    style="cursor: pointer;"
                                    >MEMBER</a
                                >
                            </base-dropdown>
                            <base-button
                                type="primary"
                                simple
                                disabled
                                v-if="edit && (row.role === 'owner' || loading.selectRole == row.player._id || $session.state.currentUser._id === row.player._id)"
                                :loading="loading.selectRole == row.player._id"
                                style="max-width: 150px; margin: 0"
                                class="td-buttons"
                            >
                                {{ loading.selectRole == row.player._id ? '' : row.role.toUpperCase() }}
                            </base-button>
                            {{ !edit ? row.role == 'owner' ? "MANAGER" : row.role.toUpperCase() : ""}}
                        </td>
                        <td>
                            <base-button
                                v-if="edit && row.role !== 'owner' && $session.state.currentUser._id !== row.player._id"
                                class="btn-red"
                                type="danger"
                                icon
                                :loading="loading.deleteMember == row.player._id"
                                ><i
                                    v-if="loading.deleteMember != row.player._id"
                                    @click="deleteMember(row)"
                                    class="fas fa-times"
                            /></base-button>
                        </td>
                    </template>
                </base-table> 
                <h3 v-if="pending.length > 0 && isOwner">JOIN REQUESTS</h3>
                <base-table
                    class="col-12"
                    :data="pending"
                    style="height:50%;"
                    v-if="pending.length > 0 && isOwner"
                >
                    <template slot="columns">
                        <th>Name</th>
                        <th v-if="!edit">Discord</th>
                        <th>Pending</th>
                    </template>
                    <template slot-scope="{ row }" v-if="row">
                        <td>{{ row.player.nickname }}</td>
                        <td v-if="!edit">{{ row.player.discordUser }}</td>
                        <td>
                            <base-button
                                v-if="$session.state.currentUser._id !== row.player._id"
                                class="btn-red mr-2"
                                type="info"
                                icon
                                :loading="loading.addMember == row.player._id"
                                ><i
                                    v-if="loading.addMember != row.player._id"
                                    @click="addMember(row.player)"
                                    class="fas fa-check"
                            /></base-button>
                            <base-button
                                v-if="$session.state.currentUser._id !== row.player._id"
                                class="btn-red"
                                type="danger"
                                icon
                                :loading="loading.deleteMember == row.player._id"
                                ><i
                                    v-if="loading.deleteMember != row.player._id"
                                    @click="deletePendingMember(row.player)"
                                    class="fas fa-times"
                            /></base-button>
                        </td>
                    </template>
                </base-table>
            </div>
            <base-button @click="leaveOrganization()" :loading="loading.leaveOrganization" v-if="!isOwner && !edit && members.find(m => m.player._id == $session.state.currentUser._id)" class="float-right mr-2" type="danger">Leave</base-button>
        </div>
        <div class="float-right">
            <loading-button v-if="!isMember" :func="() => requestToJoin()" :disabled="isPending || !$session.state.currentUser.discordId" type="success" :title="$t('organizations.requestToJoin').toUpperCase()"></loading-button>
            <loading-button v-if="isPending" :func="() => deletePendingMember($session.state.currentUser)" :title="$t('common.cancel').toUpperCase()"></loading-button>

        </div>
    </div>
</template>
<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { BaseTable, LoadingButton } from '../../components';

@Component({
    components: {
        BaseTable,
        LoadingButton,
    },
    props: {
        edit: {
            type: Boolean,
            default: () => false,
        },
        members: {
            type: Array,
            default: () => [],
        },
    },
    computed: {
        rowStyle() {
            return {
                owner: 'color: #37b5ff',
                officer: 'color: #a7d252',
                captain: 'color: #ff9c2b',
                member: '',
            };
        },
        pending() {
            return this.$organizations.current?.pending || [];
        },
        isOwner() {
            return ['owner', 'officer'].includes(this.$organizations.current?.members?.find((m) => m.player._id === this.$session.state.currentUser._id)?.role);
        },
        isMember() {
            return this.members.find((m) => m.player._id === this.$session.state.currentUser._id) ? true : false;
        },
        isPending() {
            return this.pending.find((m) => m.player._id === this.$session.state.currentUser._id) ? true : false;
        },
    },
})

export default class OrganizationMemberList extends Vue {
    public edit: boolean;
    public members: Member[];
    public pending: PendingMember[];
    public loading = {
        selectRole: '',
        deleteMember: '',
        addMember: '',
        joinRequest: false,
        leaveOrganization: false,
    };

    public async setRole(player: Player, role: string) {
        // this.organization.members.find(m => m.player._id == player._id).role = role;
        this.loading.selectRole = player._id;
        const obj: Member = {
            _key: player._id,
            player,
            role,
        };
        await this.$organizations.updateMember(obj);
        this.loading.selectRole = '';
        // this.$organizations.saveOrganization(this.organization);
    }

    public async deleteMember(member: Member) {
        this.loading.deleteMember = member.player._id;
        const result = await this.$organizations.removeMember(member);
        this.loading.deleteMember = '';
    }

    public async deletePendingMember(player: Player) {
        this.loading.deleteMember = player._id;
        await this.$organizations.removePendingMember(player._id);
        this.loading.deleteMember = '';
    }

    public async addMember(player: Player) {
        this.loading.addMember = player._id;
        await this.$organizations.addMember({player, role: 'member'} as Member);
        this.loading.addMember = '';
    }

    public async requestToJoin(player?: Player) {
        this.loading.joinRequest = true;
        if (!player) {
            player = this.$session.state.currentUser;
        }
        await this.$organizations.joinRequest(player);

        this.loading.joinRequest = false;
    }

    public async leaveOrganization() {
        this.loading.leaveOrganization = true;
        await this.$organizations.leaveOrganization(this.$organizations.current?.id);
        this.loading.leaveOrganization = false;
    }
}

</script>