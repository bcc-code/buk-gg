<template>
    <div class="row">
        <div class="col-lg-8 blog-page">
            <div
                v-if="
                    ['pc2020', 'rc2019', 'pinse2020', 'hc2020', 'rc2020'].includes(
                        $route.params.camp,
                    )
                "
            >
                <card class="organization-header">
                    <img
                        slot="image"
                        class="card-img-top"
                        style="height: 500px; object-fit: contain"
                        :src="`/img/${$route.params.camp}-logo.png`"
                    />
                </card>
                <card>
                    <div class="card-body">
                        <div
                            class="page-details-body"
                            v-if="$session.state.isAuthenticated"
                            v-html="
                                $t(`${$route.params.camp}.body`).replace(
                                    /<p><\/p>/g,
                                    '<br/>',
                                )
                            "
                        ></div>
                    </div>
                </card>
            </div>
        </div>
        <!-- <div class="col-lg-4">
            <card>
                <template slot="header"
                    ><h3 class="card-title">
                        {{ $t('common.camps') }}
                    </h3></template
                >
                <div @click="pushTo('pc2020')" style="cursor: pointer;">
                    <img style="width: 100%;" src="/img/vpc20.png" />
                </div>
                <br />
                <div @click="pushTo('rc2019')" style="cursor: pointer;">
                    <img style="width: 100%;" src="/img/rc19.png" />
                </div>
                <br />
                <div @click="pushTo('pinse2020')" style="cursor: pointer;">
                    <img style="width: 100%;" src="/img/pinse20bg.png" />
                </div>
            </card>
        </div> -->
    </div>
</template>

<style lang="scss">
.page-details {
    max-width: 1000px;
}
</style>

<script lang="ts">
import { Vue } from 'vue-property-decorator';

export default class CampDetails extends Vue {
    public pushTo(param) {
        if (this.$route.params.camp === param) {
            return;
        } else {
            this.$router.push({
                name: 'camp-details',
                params: { camp: param },
            });
        }
    }
}
</script>
