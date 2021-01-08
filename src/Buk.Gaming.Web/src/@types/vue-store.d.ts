import { SessionState, SessionStore } from '@/store/modules/SessionStore';
import { Store } from 'vuex';
import { RootState, RootStore } from '@/store';
import { EventBus } from '@/services/eventBus';
import { Api } from '@/services/api';
import { TournamentStore } from '@/store/modules/TournamentStore';
import { EventStore } from '@/store/modules/EventStore';
import { OrganizationStore } from '@/store/modules/OrganizationStore';
import { TeamStore } from '@/store/modules/TeamStore';

declare module 'vue/types/vue' {
    interface Vue {
        // Store
        $state: RootState;
        $session: SessionStore;
        $tournaments: TournamentStore;
        $sidebar: any;
        $ievents: EventStore;
        $organizations: OrganizationStore;
        $teams: TeamStore;

        // Services
        $events: EventBus;
        $api: Api;
        $mq: string;
        $moment: any;
        $env: string; // defined in main.ts
    }
}
