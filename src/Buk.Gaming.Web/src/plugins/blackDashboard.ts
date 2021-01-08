import SideBar from '@/components/SidebarPlugin';
import Notify from '@/components/NotificationPlugin';
import clickOutside from '../directives/click-outside';
import GlobalComponents from './globalComponents';
import Vue, { PluginObject } from 'vue';
// import RTLPlugin from './RTLPlugin';

// css assets
import '@/assets/sass/black-dashboard.scss';
// import '@/assets/css/nucleo-icons.css';
// import '@/assets/demo/demo.css';

const GlobalDirectives = {
    install(vue: any) {
        vue.directive('click-outside', clickOutside);
    },
};

export default {
    install(vue: typeof Vue) {
        vue.use(GlobalComponents);
        vue.use(GlobalDirectives);
        vue.use(SideBar);
        vue.use(Notify);
        // Vue.use(RTLPlugin);
    },
};
