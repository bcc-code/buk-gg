<template>
    <div>
        <div v-if="!loading.page">
            <div class="row tournament-list">
                <div class="col-12" v-if="type !== 'player'">
                    <h1 class="page-header">
                        {{ $t('common.organizations').toUpperCase() }}
                    </h1>
                </div>

                <div
                    :class="
                        type == 'player'
                            ? 'col-xl-3 col-lg-4 col-md-6 col-sm-6 col-6'
                            : 'col-xl-2 col-lg-3 col-md-4 col-sm-6 col-6'
                    "
                    v-for="item in organizations"
                    :key="item.id"
                >
                    <div class="link-tile organization-card" 
                        @click="
                            $router.push({
                                name: 'organization',
                                params: { id: item.id },
                            })
                        ">
                        <div class="link-tile-title text-center">{{ item.name }}</div>
                        <card
                            style="padding-top: 100%;"
                            text-variant="light"
                            bg-variant="secondary"
                            :style="`background-image: url(${item.image ? item.image + '?h=250' : ''});${!item.image ? 'background-color: #2c2f33;' :''}`"
                        >
                        </card>
                    </div>
                </div>
            </div>
            <div class="row" v-if="type !== 'player'">
                <div class="col-12" v-if="$session.state.currentUser.isO18">
                    <base-button type="success" :disabled="!$session.state.currentUser.discordIsConnected" @click="modal.createOrg = true">{{
                        `${$t('organizations.create')}`.toUpperCase()
                    }}</base-button>
                    <base-button class="ml-3" type="info" @click="openOrganizationIntro()">{{$t('common.information').toUpperCase()}}</base-button>
                </div>
                <div class="col-12" v-if="!$session.state.currentUser.isO18">
                    <h5 class="text-white">
                        {{ $t('organizations.contactForCreation') }}
                    </h5>
                </div>
                <div class="col-12 text-white">
                    {{ $t('organizations.privateInProfile') }}
                </div>
            </div>
        </div>
        <div
            v-if="loading.page"
            style="position: fixed; width: 100%; height: 100%; top: 0; left: 0;"
        >
            <semipolar-spinner
                style="margin: auto; top: 40%;"
                :animation-duration="2000"
                :size="200"
                color="#ff1d5e"
            />
        </div>
        <modal
            :show.sync="modal.createOrg"
            body-classes="p-0"
            modal-classes="modal-dialog-centered modal-sm"
        >
            <card style="margin-bottom: 0;">
                <h3>{{ $t('organizations.create').toUpperCase() }}</h3>
                <div class="card-body text-white">
                    {{ $t('organizations.createInformation') }}
                </div>
                <base-input
                    v-model="createOrg.name"
                    type="name"
                    placeholder="Name"
                >
                </base-input>
                <base-alert type="danger" dismissible v-if="nameNotFilled"
                    >Name invalid</base-alert
                >
                <div class="card-body">
                    <base-button
                        type="success"
                        :disabled="!$session.state.currentUser.phoneNumber"
                        @click="createOrganization()"
                        >Create</base-button
                    >
                    <base-button
                        class="ml-lg-3 center-mobile float-right"
                        @click="modal.createOrg = false"
                        >Close</base-button
                    >
                    <div class="text-white" v-if="!$session.state.currentUser.phoneNumber">
                        Add a phone number to your account to be able to create an organization.
                    </div>
                </div>
            </card>
        </modal>
    </div>
</template>

<script lang="ts">
import Modal from '@/components/Modal.vue';
import { mapActions, mapState } from 'vuex';
import { Component, Vue, Prop } from 'vue-property-decorator';
import { BaseTable, BaseAlert } from '../../components/';
import { Organization } from '../../classes';
import api from '../../services/api';
import { SemipolarSpinner } from 'epic-spinners';

@Component({
    components: {
        Modal,
        BaseTable,
        BaseAlert,
        SemipolarSpinner,
    },
    props: {
        type: {
            type: String,
            default: () => {
                return 'all';
            },
        },
    },
    computed: {
        organizations() {
            return this.type !== 'all' ? this.$organizations.state.playerOrganizations : this.$organizations.state.all.filter(
                (o) => o.isPublic,
            );
        },
    },
})
export default class OrganizationsList extends Vue {
    public type: string;
    // public organizations: Organization[] = [];
    public modal = {
        createOrg: false,
    };
    public loading = {
        page: false,
    };
    public createOrg: Organization = new Organization();
    public nameNotFilled = false;

    public async createOrganization() {
        const player = this.$session.state.currentUser;

        if (player.isO18) {
            if (this.createOrg.name !== '' || this.createOrg.name) {
                const result = await api.organizations.createOrganization(
                    this.createOrg,
                );
                if (result.id) {
                    this.$router.push(`/organization/${result.id}`);
                }
                this.nameNotFilled = false;
            } else {
                this.nameNotFilled = true;
            }
        }
    }

    public openOrganizationIntro() {
        window.open('https://buk.gg/pdf/BUK-Gaming-Organizations.pdf');
    }

    // private async mounted() {
    //     this.loading.page = true;
    //     // if (this.$organizations.state?.all.length == 0) {
    //     //     await this.$organizations.loadAll();
    //     // }
    //     // if (this.$props.type !== 'all') {
    //     //     this.organizations = this.$organizations.state.playerOrganizations;
    //     // } else {
    //     //     this.organizations = this.$organizations.state.all.filter(
    //     //         (o) => o.isPublic,
    //     //     );
    //     // }
    //     this.loading.page = false;
    // }
}
</script>

<style lang="scss">
.org-list tbody tr:hover {
    background: #2a2e30;
    cursor: pointer;
}
/* // .player-organizations {
//     &__grid {
//         > div:not(:first-child) {
//             background-color: rgba(255, 255, 255, 0.2);
//             padding: 0.5em;
//             border-radius: 5px;
//         }
//     }
//     &__organization {
//         display: grid;
//         grid-template-columns: 1fr;
//         grid-auto-columns: 1fr;
//         grid-auto-flow: column;
//     }
// } */
</style>
