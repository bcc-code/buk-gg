<template>
    <div class="tournament-card" :class="classes" @click="onClick()">
        <div class="link-tile-title text-center">{{ item.title.toUpperCase() }}</div>
        <card
            style="padding-top: 100%"
            text-variant="light"
            bg-variant="secondary"
            class="tournament-card"
            :style="`background-image: url(${item.logo ? item.logo + '?h=250' : ''});`"
        >
            <!-- <img
                @click="
                "
                slot="image-bottom"
                class="card-img-bottom"
                :src="item.logo ? item.logo + '' : ''"
            /> -->

            <!-- {{ item.introduction }} -->
        </card>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component({
    props: {
        item: {
            type: Object,
            default: () => {
                return {};
            },
        },
        classes: {
            type: String,
            default: () => {
                return '';
            },
        },
    },
})
export default class TournamentList extends Vue {
    public item: TournamentInfo;

    public onClick() {
        this.$router.push({
            name: 'tournament-details',
            params: { tournamentId: this.item.slug },
        });
    }
}
</script>
