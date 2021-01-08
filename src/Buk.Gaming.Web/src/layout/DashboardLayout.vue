<template>
    <div class="wrapper">
        <side-bar>
            <template slot="links">
                <sidebar-link
                    to="/dashboard"
                    :name="$t('module.dashboard')"
                    icon="fas fa-columns"
                />
                <sidebar-link
                    to="/discord"
                    name="Discord"
                    icon="fab fa-discord"
                />
                <sidebar-link
                    to="/tournaments"
                    :name="$t('common.tournaments')"
                    icon="fas fa-trophy"
                />
                <sidebar-link
                    :to="'/camps/rc2020'"
                    name="RC2020"
                    icon="fas fa-tv"
                />
                <sidebar-link
                    to="/organizations"
                    :name="$t('common.organizations')"
                    icon="fas fa-globe"
                />
                <sidebar-link
                    to="/teams"
                    :name="$t('common.teams')"
                    icon="fas fa-users"
                />
                <sidebar-link
                    to="/profile"
                    :name="$t('common.profile')"
                    icon="fas fa-user"
                />
            </template>
        </side-bar>
        <div class="background-image">
            <img src="/img/rc20-background.jpg" />
        </div>
        <div class="main-panel">
            <top-navbar></top-navbar>
            <dashboard-content
                style="max-width: 1890px;"
                @click.native="toggleSidebar"
            >
            </dashboard-content>
            <div style="position: fixed; width: 100%">
                <content-footer></content-footer>
            </div>
        </div>
    </div>
</template>
<style lang="scss">
.registeredmodal {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100vh;
}

.modal-content .modal-body p {
    color: #6c757d !important;
}

// .form-group {
//   input {
//     color: black;
//   }
// }
</style>
<script lang="ts">
import { Component, Vue, Watch } from 'vue-property-decorator';
import TopNavbar from './TopNavbar.vue';
import ContentFooter from './ContentFooter.vue';
import DashboardContent from './Content.vue';
import MobileMenu from './MobileMenu.vue';
import Modal from '@/components/Modal.vue';
import EditProfile from '@/pages/Profile/EditProfileForm.vue';
import BaseAlert from '@/components/BaseAlert.vue';
import { RootStore } from '../store';
import auth from '../services/auth';
import discord from '../services/discord';

@Component({
    components: {
        TopNavbar,
        ContentFooter,
        DashboardContent,
        MobileMenu,
        Modal,
        EditProfile,
        BaseAlert,
    },
})
export default class DashboardLayout extends Vue {
    public showMenu: boolean = false;

    public toggleSidebar() {
        if (this.$sidebar.showSidebar) {
            this.$sidebar.displaySidebar(false);
        }
    }
}
</script>
