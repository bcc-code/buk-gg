<template>
    <div class="row tournament-list">
        <div class="col-12">
            <h1 class="page-header">
                {{ $t('common.camps').toUpperCase() }}
            </h1>
        </div>

        <div
            class="col-xl-4 col-lg-4 col-md-6 mb-lg-6 col-sm-12 col-12 link-tile"
            v-for="item in camps"
            :key="item.id"
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
                    @click="
                        $router.push(`/camps/${item.name}`).catch((err) => {
                            return;
                        })
                    "
                    slot="image-bottom"
                    class="card-img-bottom"
                    :src="item.image ? item.image + '' : ''"
                />

                <!-- {{ item.introduction }} -->
            </card>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component
export default class CampList extends Vue {
    public window = window;

    public camps: Array<{
        name: string;
        title: any;
        image: string;
    }>;

    public beforeMount() {
        this.camps = [
            {
                name: 'rj2020',
                title: this.$t('rc2020.title'),
                image: '/img/rc2020-header.png',
            },
            {
                name: 'hc2020',
                title: this.$t('hc2020.title'),
                image: '/img/hc2020-header.png',
            },
            {
                name: 'pinse2020',
                title: this.$t('pinse2020.title'),
                image: '/img/pinse20bg-1080.png',
            },
            {
                name: 'pc2020',
                title: this.$t('pc2020.title'),
                image: '/img/vpc20.png',
            },
            {
                name: 'rc2019',
                title: this.$t('rc2019.title'),
                image: '/img/rc19.png',
            },
        ];
    }
}
</script>
