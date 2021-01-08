<template>
    <div class="privacypage">
        <div class="privacycontent">
            <card>
                <!-- <template slot="header">
          <h1 class="card-title text-center">{{ $t('privacypolicy.title') }}</h1>
        </template> -->
                <div class="card-body page-details-body">
                    <div v-html="$t('privacypolicy.introduction')"></div>
                    <br />
                    <div v-html="$t('privacypolicy.body')"></div>
                </div>
            </card>
        </div>
    </div>
</template>
<script lang="ts">
export default {};
</script>
<style lang="scss">
.privacypage {
    display: flex;
    align-items: center;
    justify-content: center;
}
.privacycontent {
    max-width: 900px;
}
</style>
