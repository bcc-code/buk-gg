<template>
    <div>
        <base-button
            type="success"
            class="center-mobile"
            :disabled="isSignedUp || signedUp"
            @click="viewModal = true"
            >{{ !(signedUp || isSignedUp) ? $t('registration.title').toUpperCase() : "SIGNED UP"}}</base-button
        >
        <base-alert type="success" v-if="successAdd"
            >SUCCESSFULLY SIGNED UP</base-alert
        >
        <modal
            :show.sync="viewModal"
            body-classes="p-0"
            modal-classes="modal-dialog-centered modal-sm"
        >
            <card
                v-if="tournament.signupType == 'team'"
                style="margin-bottom: 0;"
            >
                <template slot="header">{{ $t('tournaments.myTeams').toUpperCase() }}</template>
                <template>
                <div
                    class="card-body text-white"
                    v-if="!$session.state.currentUser.phoneNumber"
                >
                    {{ $t('tournaments.teamRequiredInformation') }}
                    <base-input
                        :label="$t('registration.phoneNumber')"
                        v-model="player.phoneNumber"
                        :placeholder="$t('registration.phoneNumber')"
                    >
                        <small slot="helperText" class="text-muted">{{
                            $t('registration.phoneNumberDescription')
                        }}</small>
                    </base-input>
                    <base-button

                        :loading="loading.saveUser"
                        type="info"
                        @click="savePhonenumber()"
                        >{{ $t('common.save').toUpperCase() }}</base-button
                    >
                </div>
                    <div class="card-body text-white" v-else>Only showing your teams in {{ tournament.title }}.<br/>Required members: {{ tournament.teamSize.min }}</div>
                    <base-table :data="teams" :columns="['name']" v-if="teams.length > 0">
                        <template slot="columns">
                            <th>Name</th>
                        </template>
                        <template slot-scope="{ row }">
                            <td>{{ row.name }}</td>
                            <td>
                                <base-button
                                    :type="
                                        !eligibleTeams.includes(row.id)
                                            ? 'warning'
                                            : 'success'
                                    "
                                    :disabled="
                                        !eligibleTeams.includes(row.id) ||
                                        tournament.teams.find(
                                            (t) => t._id === row.id,
                                        )
                                            ? true
                                            : tournament.teams.find(
                                                  (t) => t.id === row.id,
                                              )
                                            ? true
                                            : false
                                    "
                                    @click="fillInfo(row)"
                                    >{{
                                        !eligibleTeams.includes(row.id)
                                            ? 'NOT ELIGIBLE'
                                            : tournament.teams.find(
                                                  (t) => t.id === row.id,
                                              )
                                            ? 'SIGNED UP'
                                            : 'SIGN UP'
                                    }}</base-button
                                >
                            </td>
                        </template>
                    </base-table>
                    <base-button @click="viewModal = false">Close</base-button>
                </template>
            </card>
            <card
                v-if="tournament.signupType == 'solo'"
                style="margin-bottom: 0;"
            >
                <h3>{{ $t('registration.title') }}</h3>
                <div
                    class="card-body text-white"
                    v-if="!$session.state.currentUser.phoneNumber"
                >
                    {{ $t('tournaments.phoneNumberIsRequired') }}
                    <base-input
                        :label="$t('registration.phoneNumber')"
                        v-model="player.phoneNumber"
                        :placeholder="$t('registration.phoneNumber')"
                    >
                        <small slot="helperText" class="text-muted">{{
                            $t('registration.phoneNumberDescription')
                        }}</small>
                    </base-input>
                    <base-button

                        :loading="loading.saveUser"
                        type="info"
                        @click="savePhonenumber()"
                        >{{ $t('common.save').toUpperCase() }}</base-button
                    >
                </div>

                <div class="card-body" v-if="$session.state.currentUser.phoneNumber">
                    <div
                        class="text-white"
                        v-for="field in requiredFields"
                        :key="field.number"
                    >
                        {{ field.question }}
                        <base-input
                            :class="
                                !field.answer ? 'has-danger' : 'has-success'
                            "
                            v-model="field.answer"
                        ></base-input>
                    </div>
                </div>
                <div class="card-body">
                    <base-button
                        type="success"
                        :disabled="
                            !$session.state.currentUser.phoneNumber ||
                            fieldsNotAnswered
                        "
                        v-if="$session.state.currentUser.phoneNumber"
                        @click="playerSignUp()"
                        >Sign Up</base-button
                    >
                    <base-button
                        class="ml-lg-3 center-mobile float-right"
                        @click="viewModal = false"
                        >Close</base-button
                    >
                </div>
            </card>
        </modal>
        <modal
            :show.sync="viewInfoModal"
            body-classes="p-0"
            modal-classes="modal-dialog-centered modal-sm"
        >
            <card style="margin-bottom: 0;">
                <h3>{{ $t('registration.title') }}</h3>
                <div class="card-body">
                    <div
                        class="text-white"
                        v-for="field in requiredFields"
                        :key="field.number"
                    >
                        {{ field.question }}
                        <base-input v-model="field.answer"></base-input>
                    </div>
                </div>
                <div class="card-body">
                    <base-button
                        type="success"
                        :disabled="!$session.state.currentUser.phoneNumber"
                        v-if="$session.state.currentUser.phoneNumber"
                        @click="teamSignUp(selectedTeam)"
                        >Sign Up</base-button
                    >
                    <base-button
                        class="ml-lg-3 center-mobile float-right"
                        @click="viewInfoModal = false"
                        >Close</base-button
                    >
                </div>
            </card>
        </modal>
    </div>
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { Modal, BaseAlert, BaseTable } from '../../components';
import { Team } from '../../classes';
import api from '../../services/api';

@Component({
    props: {
        tournament: {
            type: Object,
            default: () => {
                return {};
            },
        },
    },
    components: {
        Modal,
        BaseAlert,
        BaseTable,
    },
    computed: {
        fieldsNotAnswered() {
            if (this.requiredFields?.find((f) => !f.answer)) {
                return true;
            }
            return false;
        },
        isSignedUp() {
            if (this.tournament.signupType === 'solo') {
                if (this.tournament.playerIds?.includes(this.player._id)) { return true; }
            }
            return false;
        },
    },
})
export default class TournamentRegistration extends Vue {
    public player: User = {} as User;
    public tournament: TournamentInfo;
    public eligibleTeams: string[] = [];
    public teams: Team[] = [];
    public successAdd: boolean = false;
    public viewModal: boolean = false;
    public viewInfoModal: boolean = false;
    public requiredFields: Array<{
        number: number;
        question: string;
        answer: string;
    }> = [];
    public selectedTeam: Team = {} as Team;
    public loading = {
        saveUser: false,
    };
    public signedUp = false;

    public async mounted() {
        this.player = Object.assign({}, this.$session.state.currentUser);
        let i = 0;
        this.tournament.requiredInformation?.forEach((req) => {
            this.requiredFields.push({
                number: i,
                question: req,
                answer: '',
            });
            i++;
        });

        this.teams = this.$teams.state.myTeams.filter(
            (t) => t.gameId === this.tournament.game?._id,
        );
        this.teams.forEach((t) => {
            if (
                t.players.length >= this.tournament.teamSize.min &&
                t.players.length <= this.tournament.teamSize.max
            ) {
                this.eligibleTeams.push(t.id);
            }
        });
    }

    public async savePhonenumber() {
        this.loading.saveUser = true;
        if (this.player?.phoneNumber) {
            await this.$session.updateUser(this.player);
        }
        this.loading.saveUser = false;
    }

    public fillInfo(team: Team) {
        this.selectedTeam = team;
        if (this.tournament.requiredInformation?.length > 0) {
            this.viewModal = false;
            this.viewInfoModal = true;
        } else {
            this.teamSignUp(team);
        }
    }

    public async playerSignUp(player?: Player) {
        const information = [];
        this.requiredFields?.forEach((field) => {
            information.push(`${field.question} | ${field.answer}`);
        });
        player = player || this.$session.state.currentUser;
        const result = await this.$tournaments.addPlayerToTournament(
            this.$tournaments.current.id,
            player,
            information,
        );
        if (result) {
            this.successAdd = true;
            this.signedUp = true;
            setTimeout(() => {
                this.successAdd = false;
            }, 5000);
        }
        this.viewModal = false;
    }

    public async teamSignUp(row: any) {
        const information = [];
        this.requiredFields?.forEach((field) => {
            information.push(`${field.question} | ${field.answer}`);
        });

        const result = await this.$tournaments.addTeamToTournament({
            tournamentId: this.$tournaments.current.id,
            team: this.selectedTeam,
            information,
        });
        if (result) {
            this.successAdd = true;
            setTimeout(() => {
                this.successAdd = false;
            }, 5000);
        }
        this.viewInfoModal = false;
    }
}
</script>
<style></style>
