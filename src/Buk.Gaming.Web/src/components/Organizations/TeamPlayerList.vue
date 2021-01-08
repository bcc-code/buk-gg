<template>
    <base-table :data="team.players">
        <template slot="columns">
            <th v-if="edit">Captain</th>
            <th>Name</th>
            <th v-if="!edit">Discord</th>
            <th v-if="edit || isCaptain">Remove</th>
        </template>
        <template slot-scope="{ row }" class="row text-left">
            <td v-if="edit">
                <base-button
                    v-if="
                        captain
                            ? row._id !== captain._id
                            : false
                    "
                    icon
                    type="success"
                    ><i
                        class="fas fa-check"
                        @click="setCaptain(row)"
                /></base-button>
                <base-button v-else icon type="success" disabled
                    ><i class="fas fa-check"
                /></base-button>
            </td>
            <td>{{ row.nickname }}</td>
            <td
                v-if="!edit"
                :class="
                    row.id === captain.id ? 'text-bold' : ''
                "
            >
                {{ row.discordUser }}
            </td>
            <td v-if="edit || isCaptain">
                <base-button
                    v-if="
                        captain
                            ? row._id !== captain._id
                            : false || edit
                    "
                    class="btn-red"
                    type="danger"
                    icon
                    ><i
                        @click="removeTeamMember(row)"
                        class="fas fa-times"
                /></base-button>
            </td>
        </template>
    </base-table>
</template>
<script lang="ts">
import { BaseTable } from '..';
import { Component, Vue } from 'vue-property-decorator';
import { Team } from '../../classes';

@Component({
    components: {
        BaseTable,
    },
    props: {
        team: {
            required: true,
            type: Object,
            default: {},
        },
        edit: {
            type: Boolean,
            default: false,
        },
    },
    computed: {
        players() {
            return this.team?.players;
        },
        captain() {
            return this.team?.captain;
        },
        isCaptain() {
            return this.$route.name === 'organization' ? this.captain._id === this.$session.state.currentUser?._id : false;
        },
    },
})

export default class TeamPlayerList extends Vue {
    public players: Player[];
    public captain: Player[];
    public team: Team;

    public removeTeamMember(player: Player) {
        this.$teams.removePlayer(this.team, player);
    }

    public setCaptain(player: Player) {
        this.$teams.setCaptain(this.team, player);
    }
}
</script>