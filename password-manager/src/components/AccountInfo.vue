<script lang="ts">
import { login } from '@/services/accountService';
import { useAccountStore } from '@/stores/counter';
import { defineComponent } from '@vue/runtime-dom';

export default defineComponent({
  name: "AccountInfo",
  data() {
    return {
      account: useAccountStore().$state.account,
    }
  }
})
</script>

<template>
  <div :class="$style.component">
    <p>{{ account.login ?? "Login" }}</p>
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include row;
  position: fixed;
  right: 1em;
}
</style>