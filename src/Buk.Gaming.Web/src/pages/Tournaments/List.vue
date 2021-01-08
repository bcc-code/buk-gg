<template>
    <div class="row tournament-list">
        <div class="col-12">
            <h1 class="page-header">
                {{ $t('common.tournaments').toUpperCase() }}
            </h1>
        </div>

        <div
            class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-6"
            v-for="item in $tournaments.state.all"
            :key="item.id"
        >
            <tournament-card classes="link-tile" :item="item"></tournament-card>
        </div>
    </div>
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import TournamentCard from './Card.vue';

@Component({
    components: {
        TournamentCard,
    },
})
export default class TournamentList extends Vue {
    public window = window;
    public mounted() {
        this.$tournaments.loadAll();
    }
}
</script>
