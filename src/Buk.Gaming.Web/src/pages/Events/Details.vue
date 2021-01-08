<template>
    <div class="row">
        <div class="col-md-8 page-details">
            <div class="row">
                <card
                    class="col-md-8 col-12"
                    style="margin-left: auto; margin-right: auto;"
                >
                    <img
                        slot="image"
                        class="card-img-top"
                        :src="event.image ? event.image + '' : ''"
                    />
                    <div>
                        <h1>
                            {{ event.title }}
                            <button
                                class="float-right center-mobile btn btn -success"
                            >
                                {{ date.toLocaleString() }}
                            </button>
                        </h1>
                    </div>
                </card>
            </div>

            <div class="card">
                <div class="card-body p-0 p-md-4">
                    <div class="row m-0 mb-3 mt-2 p-md-0 p-3">
                        <a id="rules"></a>
                        <div class="page-details-body">
                            <div
                                v-html="
                                    event.description
                                        ? event.description.replace(
                                              /<p><\/p>/g,
                                              '<br/>',
                                          )
                                        : null
                                "
                            ></div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-if="tournaments ? tournaments.length > 0 : false">
                <h1>{{ $t('common.tournaments').toUpperCase() }}</h1>
                <div
                    class="col-xl-3 col-lg-4 col-md-4 mb-lg-5 col-sm-6 col-6 link-tile"
                    v-for="item in tournaments"
                    :key="item.id"
                >
                    <tournament-card :item="item"></tournament-card>
                </div>
            </div>
        </div>
        <div class="col">
            <user-card :user="event.responsible"></user-card>
        </div>
    </div>
    <!-- <div class="col-md-3 col-sm-12 links">
    <card>
      <template slot="header">
        <h3 class="card-title">{{$t('common.tournaments')}}</h3>
      </template>
      <div class="col-md-3" v-for="(item) in $ievents.state.all" :key="item.id">
        <base-button fill @click="$router.push({ name: 'tournament-details', params: { tournamentId: item.id } })">{{item.title}}</base-button>
      </div>
    </card>
  </div> -->
</template>

<style lang="scss"></style>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import UserCard from '../Profile/UserCard.vue';
import TournamentCard from '../Tournaments/Card.vue';

@Component({
    components: {
        UserCard,
        TournamentCard,
    },
})
export default class EventDetails extends Vue {
    public currentUrl: string = document.location.hostname;
    public event: IEvent | any = {};
    public tournaments: TournamentInfo[] = [];
    public date: Date = new Date();

    public async mounted() {
        if (this.$route.params.eventId) {
            await this.$ievents.loadEvent(this.$route.params.eventId);
            this.event = this.$ievents.current;
            this.date = new Date(this.event.date);
            if (this.$tournaments.state?.all?.length < 1) {
                await this.$tournaments.loadAll();
            }
            this.tournaments = this.$tournaments.state?.all?.filter((t) =>
                t.categoryIds.includes(this.event.categoryId),
            );
        } else {
            this.$router.push('/events');
        }
    }
}
</script>
