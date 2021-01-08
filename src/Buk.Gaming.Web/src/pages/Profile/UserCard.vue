<template>
    <card type="user">
        <p class="card-text"></p>
        <div class="author">
            <div class="block block-one"></div>
            <div class="block block-two"></div>
            <div class="block block-three"></div>
            <div class="block block-four"></div>
            <a>
                <img
                    class="avatar"
                    src="/img/icons/ms-icon-310x310.png"
                    alt="..."
                />
                <h5 class="title">{{ `${user.name}` }}</h5>
            </a>
            <p class="description">
                {{ `${user.nickname}` }}
            </p>
        </div>
        <p></p>
        <!-- <div slot="footer" class="button-container">
      <base-button icon round class="">
        <i class="fas fa-globe"></i>
      </base-button>
    </div> -->
    </card>
</template>
<script lang="ts">
export default {
    props: {
        user: {
            type: Object,
            default: () => {
                return {};
            },
        },
    },
};
</script>
<style></style>
