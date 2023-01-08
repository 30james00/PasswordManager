<script lang="ts">
import { defineComponent } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';

import ChangePassword from '@/components/ChangePassword.vue';
import IpAddressBlockList from '@/components/IpAddressBlockList.vue';

const toast = useToast();

export default defineComponent({
  name: 'AccountDetails',
  components: {
    ChangePassword,
    IpAddressBlockList,
  },
});
</script>

<template>
  <div :class="$style.component">
    <div>
      <h2>Change password</h2>
      <ChangePassword></ChangePassword>
    </div>
    <div :class="$style.right">
      <h2>IP address blacklist</h2>
      <IpAddressBlockList />
    </div>
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include row;
}

.right {
  margin-left: 3em;
}

@media screen and (max-width: 600px) {
  .component {
    @include col;
  }
  .right {
    margin-left: 0;
  }
}
</style>
