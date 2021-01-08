<template>
    <div class="row">
        <div class="col-lg-8 discord-details blog-page">
            <discord-account></discord-account>
            <card>
                <div class="card-body">
                    <div v-html="$t('discord.introduction')"></div>
                </div>
            </card>
            <card>
                <div class="card-body page-details-body">
                    <div
                        v-html="
                            $t('discord.body').replace(/<p><\/p>/g, '<br/>')
                        "
                    ></div>
                </div>
            </card>
        </div>
    </div>
</template>
<script lang="ts">
import DiscordAccount from './Profile/DiscordAccount.vue';
import { Component, Vue } from 'vue-property-decorator';

@Component({
    components: {
        DiscordAccount,
    },
})
export default class DiscordPage extends Vue {}
</script>
<style lang="scss"></style>
