<script lang="ts">
import { defineComponent } from '@vue/runtime-dom';
import type { ISavedPassword } from '@/models/savedPasswordModels';
import { useToast } from 'vue-toastification';
import CustomButton from '@/components/CustomButton.vue';
import CustomIconButton from '@/components/CustomIconButton.vue';
import type { IIpAddressBlock } from '@/models/ipAddressBlockModels';

const toast = useToast();

export default defineComponent({
  name: 'IpAddressBlockList',
  data() {
    return {
      ipAddresses: [] as IIpAddressBlock[],
    };
  },
  methods: {
    async handleRefresh(): Promise<void> {
      try {
        let response = await this.$axios.get('/account/ip-block');
        this.ipAddresses = response.data;
      } catch (e) {
        toast.error('Error refreshing blocked IP addresses');
        return;
      }
    },
    async handleDelete(savedPassword: IIpAddressBlock): Promise<void> {
      try {
        await this.$axios.delete(`/account/ip-block/${savedPassword.id}`);
        this.handleRefresh();
      } catch (e) {
        toast.error(`Failed to unlock ${savedPassword.ipAddress}`);
        return;
      }
    },
  },
  async created() {
    await this.handleRefresh();
  },
  components: { CustomButton, CustomIconButton },
});
</script>

<template>
  <div :class="$style.component">
    <div
      :class="$style.ipBox"
      v-for="ipAddress in ipAddresses"
      :key="ipAddress.id"
    >
      <span :class="$style.ipAddress">{{ ipAddress.ipAddress }}</span>
      <CustomIconButton icon="fa-solid fa-trash" bg="red" @click="handleDelete(ipAddress)" />
    </div>
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
  gap: .5em
}

.ipBox {
  @include row;
  align-items: center;
  border: 3px solid $light;
  border-radius: .5em;
  padding: 0.4em 1em;
}

.ipAddress {
  padding-right: 0.5em;
}
</style>
