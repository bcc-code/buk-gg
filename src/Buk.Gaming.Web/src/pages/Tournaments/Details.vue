<template>
    <div class="row" v-if="tournament.body && tournament.title">
        <div class="col-md-8 page-details">
            <card>
                <img
                    slot="image"
                    class="card-img-top"
                    :src="tournament.mainImage ? tournament.mainImage + '?h=600' : ''"
                />
                <div
                    v-if="
                        tournament.registrationOpen || tournament.telegramLink
                    "
                >
                    <p v-if="!tournament.registrationOpen">
                        {{ $t('registration.closed') }}
                    </p>
                    <registration-modal
                        class="float-left center-mobile"
                        v-if="tournament.registrationOpen"
                        :tournament="tournament"
                    ></registration-modal>
                    <base-button
                        class="center-mobile ml-lg-2"
                        v-if="tournament.telegramLink"
                        type="info"
                        style="background: #0088cc;"
                        @click="telegram()"
                        >TELEGRAM</base-button
                    >
                    <base-button
                        class="center-mobile ml-lg-2"
                        v-if="tournament.registrationForm"
                        type="secondary"
                        @click="openRegistrationForm()"
                        >{{ $t('registration.singleRegistration').toUpperCase() }}</base-button
                    >
                    <base-button
                        class="center-mobile float-right"
                        type="primary"
                        v-if="tournament.platform">
                        {{ tournament.platform }}
                        </base-button>
                </div>
                <base-button v-if="currentUserIsResponsible" @click="$router.push({
                    name: 'tournament-admin-info',
                    params: {tournamentId: tournament.id}
                })">Get Info</base-button>
            </card>

            <div v-if="tournament.liveStream" style="padding-bottom: 15px;">
                <div
                    v-if="tournament.liveStream.includes('twitch.tv')"
                    style="width: 100%;"
                >
                    <div class="embed-responsive embed-responsive-16by9">
                        <iframe
                            :src="`https://player.twitch.tv/?channel=${tournament.liveStream.replace(
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
                    <div
                        class="embed-responsive embed-responsive-16by9"
                        v-if="tournament.liveChat"
                    >
                        <iframe
                            frameborder="0"
                            scrolling="no"
                            :id="
                                tournament.liveStream.replace(
                                    'https://www.twitch.tv/',
                                    '',
                                )
                            "
                            :src="`https://www.twitch.tv/embed/${tournament.liveStream.replace(
                                'https://www.twitch.tv/',
                                '',
                            )}/chat?parent=${currentUrl}`"
                            class="embed-responsive-item"
                        >
                        </iframe>
                    </div>
                </div>
                <div v-if="tournament.liveStream.includes('youtube.com')">
                    <div class="embed-responsive embed-responsive-16by9">
                        <iframe
                            class="embed-responsive-item"
                            :src="`${tournament.liveStream}`"
                            frameborder="0"
                            allowfullscreen
                        ></iframe>
                    </div>
                    <div
                        class="embed-responsive embed-responsive-16by9"
                        v-if="tournament.liveChat"
                    >
                        <iframe
                            class="embed-responsive-item"
                            :src="`https://www.youtube.com/live_chat?v=${tournament.liveStream.replace(
                                'https://www.youtube.com/embed/',
                                '',
                            )}&embed_domain=${currentUrl}`"
                        ></iframe>
                    </div>
                </div>
            </div>

            <div class="card">
                <!-- <template slot="header">
        <h3 class="card-title">{{ $t('common.rules') }}</h3>
      </template> -->
                <div class="card-body p-0 p-md-4">
                    <div class="row m-0 mb-3 mt-2 p-md-0 p-3">
                        <a id="rules"></a>
                        <div class="bg-light-variant page-details-body">
                            <div
                                v-html="
                                    tournament.body.replace(
                                        /<p><\/p>/g,
                                        '<br/>',
                                    )
                                "
                            ></div>
                        </div>
                    </div>
                    <div class="row m-0 p-0" v-if="tournament.toornamentId">
                        <div class="col-12 p-0 mb-3">
                            <div
                                class="col bg-dark text-light bg-light-variant p-0 pb-4 pt-4 m-0 p-lg-4"
                            >
                                <a id="schedule"></a>
                                <h2 class="ml-4 ml-lg-0">
                                    {{ $t('common.schedule') }}
                                </h2>
                                <iframe
                                    height="330"
                                    style="width: 100%; max-width: 100%;"
                                    :src="`https://widget.toornament.com/tournaments/${tournament.toornamentId}/matches/schedule/?_locale=en_US&theme=dark`"
                                    frameborder="0"
                                    allowfullscreen="true"
                                ></iframe>
                            </div>
                        </div>
                        <div
                            v-for="stage in tournament.stages"
                            :key="stage.id"
                            class="col-12 p-0 mb-3"
                        >
                            <div
                                v-if="stage.id && tournament.toornamentId"
                                class="col bg-dark text-light bg-light-variant p-0 pb-4 pt-4 m-0 p-lg-4"
                            >
                                <a :id="'stage-' + stage.id"></a>
                                <h2 class="ml-4 ml-lg-0">{{ stage.name }}</h2>
                                <iframe
                                    height="600"
                                    style="width: 100%; max-width: 100%;"
                                    :src="`https://widget.toornament.com/tournaments/${tournament.toornamentId}/stages/${stage.id}?_locale=en_US&theme=dark`"
                                    frameborder="0"
                                    allowfullscreen="true"
                                ></iframe>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <card type="user" v-if="tournament.winner">
                <div class="author tournament-winner-card">
                    <div class="block block-one"></div>
                    <div class="block block-two"></div>
                    <div class="block block-three"></div>
                    <div class="block block-four"></div>
                    <i
                        class="avatar fas fa-trophy"
                        style="
                            max-width: 100%;
                            font-size: 70px;
                            margin: auto;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            color: #d4af37;
                        "
                    />
                    <h3 class="title">WINNER</h3>
                    <h1>{{ tournament.winner }}</h1>
                </div>
            </card>

            <card
                type="user"
                v-if="
                    tournament.contacts ? tournament.contacts.length > 0 : false
                "
            >
                <div class="author">
                    <div class="block block-one"></div>
                    <div class="block block-two"></div>
                    <div class="block block-three"></div>
                    <div class="block block-four"></div>
                    <i
                        class="avatar fas fa-user"
                        style="
                            max-width: 100%;
                            font-size: 70px;
                            margin: auto;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                        "
                    />
                    <h3 class="title">
                        {{ $t('common.contact').toUpperCase() }}
                    </h3>
                    <div
                        v-for="contact in tournament.contacts"
                        :key="contact._key"
                    >
                        <h3 v-if="contact.name" style="margin-bottom: 10px;">
                            {{ contact.name }}
                        </h3>
                        <h4 v-if="contact.email">
                            <a
                                target="_blank"
                                :href="`mailto:${contact.email}`"
                                >{{ contact.email }}</a
                            >
                        </h4>
                        <h4 v-if="contact.phoneNumber">{{ contact.phoneNumber }}</h4>
                        <h4 type="primary" v-if="contact.discord">
                            <i class="fab fa-discord"></i> {{ contact.discord }}
                        </h4>
                        <br />
                    </div>
                </div>
            </card>
            <base-table
                v-if="tournament.teams.length > 0"
                :data="tournament.teams"
                :columns="['name', '_id']"
                class="org-list"
            >
                <template slot="columns">
                    <th>Team</th>
                    <th>Captain</th>
                </template>
                <template slot-scope="{ row }">
                    <td
                        @click="row.organization ? row.organization.isPublic ?
                            $router.push(
                                `/organization/${row.organization._id}`,
                            ) : '' : ''
                        "
                    >{{ row.name }}</td>
                    <td
                        @click="row.organization ? row.organization.isPublic ?
                            $router.push(
                                `/organization/${row.organization._id}`,
                            ) : '' : ''
                        "
                    >{{ row.captain ? row.captain.nickname : '' }}</td>
                </template>
            </base-table>
        </div>
    </div>
    <!-- <div class="col-md-3 col-sm-12 links">
    <card>
      <template slot="header">
        <h3 class="card-title">{{$t('common.tournaments')}}</h3>
      </template>
      <div class="col-md-3" v-for="(item) in $tournaments.state.all" :key="item.id">
        <base-button fill @click="$router.push({ name: 'tournament-details', params: { tournamentId: item.id } })">{{item.title}}</base-button>
      </div>
    </card>
  </div> -->
</template>

<style lang="scss"></style>
<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { BaseTable, Modal, BaseAlert } from '../../components';
import { SemipolarSpinner } from 'epic-spinners';
import { Team } from '../../classes';
import RegistrationModal from './Registration.vue';
import api from '../../services/api';

@Component({
    components: {
        BaseTable,
        Modal,
        BaseAlert,
        RegistrationModal,
        SemipolarSpinner,
    },
    computed: {
        currentUserIsResponsible() {
            return this.tournament?.responsibleId === this.$session.state.currentUser._id;
        },
    },
})
export default class TournamentDetails extends Vue {
    public viewRegistration: boolean = false;
    public teams: Team[] = [];
    public currentUrl: string = document.location.hostname;
    public tournament: TournamentInfo | any = { body: '', teams: [] };
    public eligibleTeams: string[] = [];

    public openRegistration() {
        this.viewRegistration = true;
    }

    public telegram() {
        if (this.tournament.telegramLink) {
            window.open(this.tournament.telegramLink);
        }
    }

    public openRegistrationForm() {
        if (this.tournament.registrationForm) {
            window.open(this.tournament.registrationForm);
        }
    }

    public async mounted() {
        if (this.$route.params.tournamentId) {
            await this.$tournaments.loadTournament(
                this.$route.params.tournamentId,
            );
            this.tournament = this.$tournaments.current;
            // this.teams = await this.$tournaments.getEligibleTeamsForCaptain(this.$tournaments.current.game._id);
        }
    }
}
</script>
