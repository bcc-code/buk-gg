<template>
    <card type="user">
        <p class="card-text"></p>
        <div class="author team-card">
            <div class="block block-one"></div>
            <div class="block block-two"></div>
            <div class="block block-three"></div>
            <div class="block block-four"></div>
            <img class="avatar" :src="$route.name == 'teams' ? team.organizationIcon + '?h=150' : team.icon + '?h=150'" alt="..." />
            <base-button :loading="loading.deleting" v-if="edit && team.Id && !deleted" type="danger" class="float-right" @click="deleteTeam()" style="position: absolute; right: 0; top: 0;" icon><i class="fas fa-times"></i></base-button>
            <base-input
                v-if="edit"
                v-model="team.name"
                type="name"
                placeholder="Team Name"
            >
            </base-input>
            <h3 v-else class="title">{{ `${team.name}` }}</h3>
            <div class="card-body">
                <team-player-list :edit="edit" :team="team"></team-player-list>
                <div class="row">
                    <div v-if="currentUserIsCaptain || edit" class="col-12">
                        <base-button
                            class="float-left"
                            type="primary"
                            :loading="loading.saving"
                            @click="saveTeam()"
                            >{{
                                !loading.saving
                                    ? $t('common.save').toUpperCase()
                                    : ''
                            }}</base-button
                        >
                        <base-button
                            class="float-right"
                            type="info"
                            icon
                            @click="showAddMemberModal = true"
                            ><i class="fas fa-plus"
                        /></base-button>
                    </div>
                    <div class="col-12">
                        <base-alert dismissible v-if="failedSave" type="danger"
                            >Failed to save Team</base-alert
                        >
                        <base-alert
                            dismissible
                            v-if="successSave"
                            type="success"
                            >Saved Team</base-alert
                        >
                    </div>
                </div>
            </div>
        </div>
        <div class="text-center" v-if="list">
            <h4 type="primary" class="text-muted">
                {{
                    team.organizationName != team.name
                        ? team.organizationName
                        : ''
                }}
            </h4>
        </div>
        <select-member :show.sync="showAddMemberModal" :members="members" :handleMember="(member) => { addPlayer(member.player) }"></select-member>
    </card>
</template>
<script lang="ts">
import { Vue, Prop, Component } from 'vue-property-decorator';
import { BaseTable, Modal, BaseAlert } from '../../components';
import { SelectMember, TeamPlayerList } from '../../components/Organizations';
import { Team } from '../../classes';

@Component({
    name: 'team-card',
    props: {
        team: {
            type: Object,
            default: () => {
                return {};
            },
        },
        edit: {
            type: Boolean,
            default: () => {
                return false;
            },
        },
        list: {
            type: Boolean,
            default: () => {
                return false;
            },
        },
    },
    components: {
        BaseTable,
        Modal,
        BaseAlert,
        SelectMember,
        TeamPlayerList,
    },
    computed: {
        members() {
            return this.currentUserIsCaptain || this.edit ? this.$organizations.current?.members || [] : [];
        },
    },
})
export default class TeamCard extends Vue {
    public loading = {
        saving: false,
        deleting: false,
    };
    public team: Team;
    public edit: boolean;
    public list: boolean;

    public deleted: boolean = false;

    public currentUserIsCaptain: boolean = false;
    public addMemberField: string = '';
    public failedAdd: boolean = false;
    public showAddMemberModal: boolean = false;
    public failedSave = false;
    public successSave = false;

    public players: Player[] = [];

    public mounted() {
        if (this.$organizations.current?.id === this.team.organizationId) {
            if (
                this.team.captain?._id === this.$session.state.currentUser._id
            ) {
                this.currentUserIsCaptain = true;
            }
        }
        if (this.team.captain) {
            this.players.push(this.team.captain);
        }
        this.team.players.forEach((player) => {
            this.players.push(player);
        });
    }

    public addPlayer(player: Player) {
        this.$teams.addPlayer(this.team, player);
        // this.players.push(player);
        this.showAddMemberModal = false;
        this.addMemberField = '';
    }

    public async saveTeam() {
        this.loading.saving = true;
        const result = await this.team.save();
        this.loading.saving = false;
        if (result?.id) {
            this.successSave = true;
            setTimeout(() => {
                this.successSave = false;
            }, 4000);
        } else {
            this.failedSave = true;
            setTimeout(() => {
                this.failedSave = false;
            }, 4000);
        }
    }

    public async deleteTeam() {
        this.loading.deleting = true;
        const result = await this.team.delete();
        this.loading.deleting = false;
        if (result) {
            this.deleted = true;
        }
    }
}
</script>
<style></style>
