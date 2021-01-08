<template>
    <div class="tournament-admin-info">
        <base-table
            :data="tournament.soloPlayers || []"
            :columns="['id', 'nickname', 'discordUser']"
            v-if="tournament.signupType == 'solo'"
        >
            <template slot="columns">
                <th>Name</th>
                <th>Nickname</th>
                <th>Phone</th>
                <th>Location</th>
                <th>Discord</th>
                <th v-for="field in tournament.requiredInformation" :key="field">{{field}}</th>
            </template>
            <template slot-scope="{ row }">
                <td>{{ row.item.name }}</td>
                <td>{{ row.item.nickname }}</td>
                <td>{{ row.item.phoneNumber }}</td>
                <td>{{ row.item.location }}</td>
                <td>{{ row.item.discordUser }}</td>
                <td v-for="field in row.information" :key="field">{{field.split(' | ').splice(1).join(' | ')}}</td>
            </template>
        </base-table>
        <base-table
            :data="tournament.participantTeams || []"
            :columns="['id', 'nickname', 'discordUser']"
            v-if="tournament.signupType == 'team'"
        >
            <template slot="columns">
                <th>Name</th>
                <th>ToornamentId</th>
                <th>Captain</th>
                <th>Location</th>
                <th>Organization</th>
                <th>Players</th>
                <th v-for="field in tournament.requiredInformation" :key="field">{{field}}</th>
            </template>
            <template slot-scope="{ row }">
                <td>{{ row.item.name }}</td>
                <td>{{ row.toornamentId }}</td>
                <td style="cursor: pointer" @click="showCaptain(row.item.captain)">{{ row.item.captain ? row.item.captain.name : 'NO CAPTAIN' }}</td>
                <td>{{ row.item.captain.location }}</td>
                <td style="cursor: pointer" @click="$router.push(`/organization/${row.item.organization._id}`)">{{ row.item.organization ? row.item.organization.name : 'NO ORGANIZATION' }}</td>
                <td><base-button  @click="showPlayers(row.item.players)">Players</base-button></td>
                <td v-for="field in row.information" :key="field">{{field.split(' | ').splice(1).join(' | ')}}</td>
            </template>
        </base-table>
        <modal :show.sync="modal.captain"
            body-classes="p-0"
            modal-classes="modal-sm">
            <card style="margin-bottom: 0" type="user">
                
                <p class="card-text"></p>
                <div class="author">
                    <div class="block block-one"></div>
                    <div class="block block-two"></div>
                    <div class="block block-three"></div>
                    <div class="block block-four"></div>
                    <a>
                        <img
                            class="avatar"
                            src="/img/icons/ms-icon-310x310.png"
                            alt="..."
                        />
                    </a>
                    <h3 style="margin-bottom: 10px;">
                        {{ captain.name }}
                    </h3>
                    <h3 style="margin-bottom: 10px;">
                        {{ captain.nickname }}
                    </h3>
                    <h4>
                        <a
                            target="_blank"
                            :href="`mailto:${captain.email}`"
                            >{{ captain.email }}</a
                        >
                    </h4>
                    <h4 v-if="captain.phoneNumber">{{ captain.phoneNumber }}</h4>
                    <h4 type="primary" v-if="captain.discordUser">
                        <i class="fab fa-discord"></i> {{ captain.discordUser }}
                    </h4>
                </div>
                <p></p>
                <base-button
                    class="ml-lg-3 center-mobile float-right"
                    @click="modal.captain = false"
                    >Close</base-button
                >
            </card>
        </modal>
        <modal :show.sync="modal.players"
            body-classes="p-0"
            modal-classes="modal-lg">
            <card style="margin-bottom: 0">
                <base-button
                    class="ml-lg-3 center-mobile float-right"
                    @click="modal.players = false"
                    >Close</base-button
                >
                <base-table
                    :data="players"
                    :columns="['_id', 'nickname', 'discordUser']"
                >
                    <template slot="columns">
                        <th>Name</th>
                        <th>Nickname</th>
                        <th>Phone</th>
                        <th>Location</th>
                        <th>Discord</th>
                        <th>ToornamentId</th>
                    </template>
                    <template slot-scope="{ row }">
                        <td>{{ row.name }}</td>
                        <td>{{ row.nickname }}</td>
                        <td>{{ row.phoneNumber }}</td>
                        <td>{{ row.location }}</td>
                        <td>{{ row.discordUser }}</td>
                    </template>
                </base-table>
            </card>
        </modal>
    </div>
</template>

<script lang="ts">
import { LayoutPlugin } from 'bootstrap-vue';
import { Component, Vue } from 'vue-property-decorator';
import { BaseTable, Modal } from '../../components';
import api from '../../services/api';
import UserCard from '../Profile/UserCard.vue';

@Component({
    props: {
    },
    components: {
        BaseTable,
        Modal,
        UserCard,
    },
})

export default class TournamentAdminDetails extends Vue {
    public tournament: TournamentAdminInfo = {} as TournamentAdminInfo;
    public captain: Player = {} as Player;
    public players: Player[] = [];
    public modal = {
        captain: false,
        players: false,
    };

    public async mounted() {
        if (this.$route.params.tournamentId) {
            this.tournament = await api.tournaments.getAdminInfo(this.$route.params.tournamentId);
        }
    }

    public showCaptain(captain: any) {
        this.captain = captain;
        this.modal.captain = true;
    }

    public showPlayers(players: any) {
        this.players = players;
        this.modal.players = true;
    }
}
</script>
