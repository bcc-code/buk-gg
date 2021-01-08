<template>
    <div class="row">
        <card>
            <h1 slot="header" class="display-1">
                {{ $t('common.events').toUpperCase() }}
            </h1>
        </card>
        <div
            v-for="date in dates"
            :key="date"
            class="col-12 row"
            style="margin-left: auto; margin-right: auto;"
        >
            <h1 class="col-12 text-center">{{ date }}</h1>
            <div
                :class="`col-xl-2 col-lg-2 col-md-6 mb-lg-6 col-sm-12 col-12 link-tile`"
                v-for="item in getEvents(date)"
                :key="item.id"
                @click="$router.push(`events/${item.id}`)"
            >
                <div class="link-tile-title text-center">
                    {{ item.title.toUpperCase() }}
                </div>
                <card
                    style="cursor: pointer;"
                    text-variant="light"
                    bg-variant="secondary"
                    class="mb-2"
                >
                    <img
                        slot="image"
                        class="card-img-top"
                        :src="item.image ? item.image + '' : ''"
                    />
                    <div class="text-center">
                        <button class="btn btn-secondary">
                            {{
                                (() => {
                                    const eDate = new Date(item.date);
                                    return `${eDate
                                        .getHours()
                                        .toLocaleString()
                                        .padStart(
                                            2,
                                            '0',
                                        )}:${eDate
                                        .getMinutes()
                                        .toLocaleString()
                                        .padStart(2, '0')}`;
                                })()
                            }}
                        </button>
                    </div>

                    <!-- {{ item.introduction }} -->
                </card>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component
export default class Events extends Vue {
    public events: IEvent[] = [];
    public dates: string[] = [];

    public async mounted() {
        await this.$ievents.loadAll();
        this.$ievents.state.all.forEach((event: IEvent) => {
            this.events.push(event);
            if (
                this.dates.includes(
                    new Date(event.date).toLocaleString().split(', ')[0],
                )
            ) {
                return;
            }
            this.dates.push(
                new Date(event.date).toLocaleString().split(', ')[0],
            );
        });
        this.events = this.events
            .filter((e) => new Date(e.date).getTime() > new Date().getTime())
            .sort((a, b) => {
                return new Date(a.date).getTime() - new Date(b.date).getTime();
            });
    }
    public getEvents(date: string) {
        return this.events.filter((e) =>
            new Date(e.date).toLocaleString().split(', ')[0].startsWith(date),
        );
    }
}
</script>
