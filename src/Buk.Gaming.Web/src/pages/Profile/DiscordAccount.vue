<template>
    <card>
        <!-- <h5 slot="header" class="card-title">Connections</h5> -->
        <div class="">
            <div v-if="player">
                <img
                    slot="image"
                    @click="pushToDiscord()"
                    style="cursor: pointer; width: 390px;"
                    src="https://discordapp.com/assets/e7a3b51fdac2aa5ec71975d257d5c405.png"
                />
                <div class="card-body">
                    <div v-if="player.discordId">
                        <base-button
                            type="primary"
                            class="animation-on-hover"
                            v-if="
                                player.discordUser && player.discordIsConnected
                            "
                            fill
                            @click="openDiscord()"
                            >{{ player.discordUser }}</base-button
                        >
                        <base-button
                            type="info"
                            class="animation-on-hover"
                            v-else-if="player.discordUser"
                            fill
                            @click="openDiscord()"
                            >{{ player.discordUser }} (click for
                            invite)</base-button
                        >
                        <!-- <base-button
                            type="info"
                            v-if="player.discordUser"
                            class="animation-on-hover ml-md-2"
                            simple
                            @click="updateRoles()"
                            ><i class="fas fa-spinner"></i
                        ></base-button> -->
                        <base-button
                            type="danger"
                            class="animation-on-hover ml-md-2 mr-2"
                            id="registration-discord-login"
                            simple
                            @click="disconnectDiscordLogin()"
                            >{{ $t('discord.disconnect') }}</base-button
                        >
                        <br />
                        <base-alert v-if="updatedRoles" type="success">{{
                            $t('discord.updatedRoles')
                        }}</base-alert>
                    </div>
                    <base-button
                        v-else
                        type="primary"
                        class="animation-on-hover"
                        id="registration-discord-login"
                        @click="initDiscordLogin()"
                        >{{ $t('discord.connect') }}</base-button
                    >
                </div>
            </div>
        </div>
    </card>
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import BaseAlert from '@/components/BaseAlert.vue';
import Modal from '@/components/Modal.vue';
import Discord from '../../services/discord';
import api from '../../services/api';
export default {
    components: {
        BaseAlert,
    },
    data() {
        return {
            type: ['', 'info', 'success', 'warning', 'danger'],
            updatedRoles: false,
        };
    },
    computed: {
        player() {
            return this.$session.state?.currentUser || null;
        },
    },
    async mounted() {
        if (await Discord.completeDiscordLogin()) {
            const discordUser = Discord.discordUser;
            if (discordUser) {
                // this.notifyVue('top', 'center');

                const invite = 'https://discord.gg/8cf8XjB';
                await this.$session.loginDiscord({discordUser, invite});
                // this.$bvModal.show("discord.modal0");
            }
        }
    },
    methods: {
        async disconnectDiscordLogin() {
            Discord.disconnectDiscordLogin();
            if (this.player) {
                await this.$session.removeDiscord();
            }
        },
        initDiscordLogin() {
            Discord.startDiscordLogin();
        },
        openDiscord() {
            window.open(this.$t('discord.invite'));
        },
        pushToDiscord() {
            if (this.$route.name !== 'discord') {
                this.$router.push('/discord');
            }
        },
    },
    props: {
        user: {
            type: Object,
            default: () => {
                return {};
            },
        },
    },
};
</script>
<style></style>
