<template>
    <div>
        <div class="row">
            <div class="col-lg-4" :class="{ 'text-right': false }">
                <card style="min-height: 290px;">
                    <!-- <h5 class="card-category">{{$t('dashboard.totalShipments')}}</h5> -->
                    <h2 class="card-title card-body" style="margin-bottom: 0; font-weight: 700;">
                        <!-- <img src="/img/icons/ms-icon-310x310.png" alt="" /> -->
                        BUK GAMING
                    </h2>

                    <p
                        v-html="$t('dashboard.introduction')"
                        class="card-body"
                    ></p>
                </card>
            </div>

            <div class="col-lg-4 link-tile" @click="$router.push('/camps/rc2020')">
                <div class="link-tile-title text-center">{{ $t('hc2020.title') }}</div>
                <card style="cursor: pointer;">
                    <template slot="header">
                    </template>
                    <img
                        slot="image-bottom"
                        class="card-img-bottom"
                        src="/img/rc2020-header.jpg"
                        style="max-height: 290px; object-fit: cover;"
                    />
                </card>
            </div>
            <div class="col-lg-4" v-if="liveStreams.length > 0">
                <b-carousel
                    id="carousel"
                    ref="carouselEmbed"
                    fade
                    :interval="0"
                    controls
                >
                    <div v-for="liveStream in liveStreams" :key="liveStream">
                        <b-carousel-slide>
                            <template v-slot:img>
                                <div
                                    v-if="
                                        liveStream.includes(
                                            'twitch.tv',
                                        )
                                    "
                                    style="width: 100%;"
                                >
                                    <div
                                        class="embed-responsive embed-responsive-16by9"
                                    >
                                        <iframe
                                            :src="`https://player.twitch.tv/?channel=${liveStream.replace(
                                                'https://www.twitch.tv/',
                                                '',
                                            )}&parent=${currentUrl}&muted=true`"
                                            frameborder="0"
                                            scrolling="no"
                                            allowfullscreen="true"
                                            class="embed-responsive-item"
                                        >
                                        </iframe>
                                    </div>
                                </div>
                                <div
                                    v-if="
                                        liveStream.includes(
                                            'youtube.com',
                                        )
                                    "
                                >
                                    <div
                                        class="embed-responsive embed-responsive-16by9"
                                    >
                                        <iframe
                                            class="embed-responsive-item"
                                            :src="`${liveStream}`"
                                            frameborder="0"
                                            allowfullscreen
                                        ></iframe>
                                    </div>
                                </div>
                            </template>
                        </b-carousel-slide>
                    </div>
                </b-carousel>
            </div>
            <!-- <div class="col-lg-4">
        <card>
           <div class="card-img">
          <iframe
              src="https://player.twitch.tv/?channel=throughust&parent=buk.gg&muted=true"
              height="300px"
              width="100%"
              frameborder="0"
              scrolling="no"
              allowfullscreen="true">
          </iframe>
          </div> 
        </card>
      </div> -->
        </div>
        <div class="row">
            <div class="col-xl-4 col-lg-4 col-md-4">
                <discord-account></discord-account>
            </div>

            <tournament-card
                class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-6 link-tile"
                v-for="item in $tournaments.state.all"
                :key="item.id"
                :item="item"
            >
            </tournament-card>
        </div>
    </div>
</template>
<script lang="ts">
import config from '../config';
import DiscordAccount from './Profile/DiscordAccount.vue';
import api from '../services/api';
import { Component, Vue } from 'vue-property-decorator';
import TournamentCard from './Tournaments/Card.vue';

@Component({
    components: {
        DiscordAccount,
        TournamentCard,
    },
    computed: {
        introduction() {
            return this.$t('home.introduction').toString();
        },
        currentUrl() {
            return document.location.hostname;
        },
    },
})
export default class Dashboard extends Vue {
    public readmore: boolean = false;
    public liveStreams: string[] = [];

    public async mounted() {
        await this.$tournaments.loadAll();
        let liveStreams = 0;
        for (const i of this.$tournaments.state.all) {
            // Add back in if necessary
            this.$tournaments.loadTournament(i.id);
            if (i.liveStream) {
                if (!this.liveStreams.includes(i.liveStream)) { this.liveStreams.push(i.liveStream); }
                liveStreams = liveStreams + 1;
            }
        }
        const random = Math.floor(Math.random() * liveStreams);

        (this.$refs.carouselEmbed as any)?.setSlide(random);

        const player = Object.assign({}, this.$session.state.currentUser);
    }
}
</script>
<style lang="scss">
.primarycontentlinks {
    div.card {
        min-height: 230px;
    }
}
.card-title.display-3 {
    font-weight: bold !important;
}
.card-title img {
    margin-right: 10px;
    height: 50px;
}
.carousel-control-prev,
.carousel-control-next {
    bottom: 50px !important;
    top: 50px !important;
}
</style>
