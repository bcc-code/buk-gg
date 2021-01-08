<template>
    <div class="row">
        <div class="col-12">
            <div class="row">
                <div
                    v-for="game in games"
                    :key="game._id"
                    class="col-xl-2 col-lg-3 col-md-4 mb-lg-5 col-sm-6 col-6 link-tile"
                    :class="selectedGame._id != game._id ? '' : 'selected'"
                    
                >
                    <div @click="selectedGame._id != game._id ? onSelectGame(game) : ''"
                    
                    :style="
                        selectedGame._id != game._id
                            ? `cursor: pointer;`
                            : 'cursor: unset;'
                    ">
                        <div class="link-tile-title text-center">
                            {{ game.name.toUpperCase() }}
                        </div>
                        <card
                            text-variant="light"
                            bg-variant="secondary"
                            class="tournament-card"
                        >
                            <img
                                slot="image-bottom"
                                class="card-img-bottom"
                                :src="game.icon ? game.icon + '' : ''"
                            />

                            <!-- {{ item.introduction }} -->
                        </card>
                    </div>
                </div>
            </div>
        </div>
        <!-- <div class="col-md-6 col-12" style="margin:auto;">
            <h1 slot="header" class="text-center">{{ $t('common.teams').toUpperCase() }}</h1>
            <div class="text-center">
                <img style="max-height:200px; margin:auto; margin-bottom: 20px;" :src="selectedGame ? selectedGame.icon ? selectedGame.icon : '' : ''"/>
            </div>
            <br />
            <base-dropdown title-classes="btn btn-primary" :title="selectedGame ? selectedGame.name.toUpperCase() : $t('common.game').toUpperCase()">
                <a v-for="game in games" :key="game._id" class="dropdown-item" @click="onSelectGame(game)">{{ game.name }}</a>
            </base-dropdown>
        </div> -->
        <div class="col-12" v-if="!loading">
            <div class="row">
                <div
                    v-for="team in teams"
                    :key="team._id"
                    class="col-12 col-sm-6 col-lg-3 col-md-4"
                    @click="
                        $router.push(`/organization/${team.organizationId}`)
                    "
                >
                    <team-card
                        style="cursor: pointer;"
                        :team="team"
                        :list="true"
                    ></team-card>
                </div>
            </div>
        </div>
        <div
            class="col-12"
            v-if="selectedGame._id && teams.length < 1 && !loading"
        >
            <h3 class="text-muted">{{ `No teams in ${selectedGame.name}` }}</h3>
        </div>
        <div v-else></div>
    </div>
</template>
<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import TeamCard from './TeamCard.vue';
import { Team } from '../../classes';

@Component({
    components: {
        TeamCard,
    },
})
export default class TeamList extends Vue {
    public games: Game[] = [];
    public teams: Team[] = [];
    public loading: boolean = false;
    public selectedGame: Game = {} as Game;

    public mounted() {
        this.$teams.getGames().then((games: Game[]) => {
            this.games = games.filter((g) => g.hasTeams);
        });
    }

    public async onSelectGame(game: Game) {
        this.loading = true;
        this.selectedGame = game;
        this.teams = ((await this.$teams.getTeamsInGame(
            game,
        )) as Team[]).filter((t) => t.isPublic);
        this.loading = false;
    }
}
</script>
