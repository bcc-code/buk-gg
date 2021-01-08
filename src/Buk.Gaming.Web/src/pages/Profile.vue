<template>
    <div class="row">
        <div class="col-md-8">
            <edit-profile-form> </edit-profile-form>
            <!-- <discord-account>
      </discord-account> -->
            <organizations type="player"></organizations>
        </div>
        <div class="col-md-4">
            <user-card :user="player"></user-card>
            <discord-account></discord-account>
            <more-discords v-if="player.enableMoreDiscords"></more-discords>
        </div>
    </div>
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import EditProfileForm from './Profile/EditProfileForm.vue';
import UserCard from './Profile/UserCard.vue';
import DiscordAccount from './Profile/DiscordAccount.vue';
import Organizations from './Organizations/ListCards.vue';
import MoreDiscords from './Profile/MoreDiscords.vue';

@Component({
    components: {
        EditProfileForm,
        UserCard,
        DiscordAccount,
        Organizations,
        MoreDiscords,
    },
})
export default class Profile extends Vue {
    public player = {};

    public mounted() {
        this.player = Object.assign({}, this.$session.state.currentUser);
    }
}
</script>
<style></style>
