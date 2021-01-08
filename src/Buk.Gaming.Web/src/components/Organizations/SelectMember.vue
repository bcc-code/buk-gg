<template>
    <SlideYUpTransition :duration="100">
        <div
            class="modal fade"
            @click.self="closeModal"
            :class="[
                { 'show d-block': show },
                { 'd-none': !show },
            ]"
            v-show="show"
            tabindex="-1"
            role="dialog"
            :aria-hidden="!show"
        >
            <div
                class="modal-dialog"
            >
                <div
                    class="modal-content"
                >
                    <div
                        class="modal-header"
                        v-if="$slots.header"
                    >
                        <slot name="header"></slot>
                        <slot name="close-button">
                            <button
                                type="button"
                                class="close"
                                v-if="true"
                                @click="closeModal"
                                data-dismiss="modal"
                                aria-label="Close"
                            >
                                <i class="tim-icons icon-simple-remove"></i>
                            </button>
                        </slot>
                    </div>

                    <div
                        class="modal-body p-0"
                    >        
                        <card
                            type="secondary"
                            header-classes="bg-white pb-5"
                            body-classes="px-lg-5 py-lg-5"
                            class="border-0 mb-0"
                        >
                            <template>
                                <div class="text-muted text-center mb-3">
                                    <h3 class="modal-title">
                                        SELECT CAPTAIN
                                    </h3>
                                </div>
                                <div>
                                    <base-input
                                        :placeholder="$t('common.search')"
                                        v-model="searchField"
                                    ></base-input>
                                </div>
                            </template>
                            <div class="card-body">
                                <base-table
                                    :data="filteredMembers"
                                    class="org-list"
                                    :columns="['name', 'discordUser']"
                                >
                                    <template slot="columns">
                                        <th>Name</th>
                                        <th>Discord</th>
                                    </template>
                                    <template slot-scope="{ row }">
                                        <td @click="handleMember(row)">
                                            {{ row.player.nickname }}
                                        </td>
                                        <td @click="handleMember(row)">
                                            {{ row.player.discordUser }}
                                        </td>
                                    </template>
                                </base-table>
                            </div>
                            <template slot="footer">
                                <base-button
                                    type="secondary"
                                    @click="closeModal()"
                                    >Close</base-button
                                >
                            </template>
                        </card>
                    </div>

                    <div
                        class="modal-footer"
                        v-if="$slots.footer"
                    >
                        <slot name="footer"></slot>
                    </div>
                </div>
            </div>
        </div>
    </SlideYUpTransition>
</template>
<script lang="ts">
import { Modal, BaseTable } from '../';
import { Component, Vue } from 'vue-property-decorator';
import { SlideYUpTransition } from 'vue2-transitions';

@Component({
    components: {
        Modal,
        BaseTable,
        SlideYUpTransition,
    },
    props: {
        members: {
            type: Array,
            default: [],
        },
        show: {
            type: Boolean,
        },
        handleMember: {
            type: Function,
            default: (member) => null,
        },
    },
    methods: {
        closeModal() {
            this.$emit('update:show', false);
            this.$emit('close');
        },
    },
    computed: {
        filteredMembers() {
            return this.members?.filter((member) => {
                const search = this.searchField
                    ?.toLowerCase();
                return (
                    member.player.nickname
                        ?.toLowerCase()
                        .includes(search) ||
                    member.player.discordUser
                        ?.toLowerCase()
                        .includes(search) ||
                    member.player.name
                        ?.toLowerCase()
                        .includes(search)
                );
            });
        },
    },
    watch: {
        show(val) {
            const documentClasses = document.body.classList;
            if (val) {
                documentClasses.add('modal-open');
            } else {
                documentClasses.remove('modal-open');
            }
        },
    },
})

export default class SelectMember extends Vue {
    public members: Member[];
    public searchField: string = '';
    public show: boolean;
    public handleMember: (member) => void;
}

</script>