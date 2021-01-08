import Sidebar from './SideBar.vue';
import SidebarLink from './SidebarLink.vue';

const SidebarStore = {
    showSidebar: false,
    sidebarLinks: [],
    displaySidebar(value: any) {
        this.showSidebar = value;
    },
};

const SidebarPlugin = {
    install(Vue: any) {
        const app = new Vue({
            data: {
                sidebarStore: SidebarStore,
            },
        });

        Vue.prototype.$sidebar = app.sidebarStore;
        Vue.component('side-bar', Sidebar);
        Vue.component('sidebar-link', SidebarLink);
    },
};

export default SidebarPlugin;
