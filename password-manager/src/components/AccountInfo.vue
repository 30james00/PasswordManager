<script lang="ts">
import { useAccountStore } from '@/stores/accountStore';
import { defineComponent } from '@vue/runtime-dom';

export default defineComponent({
  name: "AccountInfo",
  data() {
    return {
      accountStore: useAccountStore(),
    }
  },
})
</script>

<template>
  <div :class="$style.component">
    <p>{{ accountStore.account != null ? "Account: " + accountStore.account.login : "Login" }}</p>
    <p v-if="accountStore.account != null" @click="accountStore.logout">Logout</p>
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
  position: fixed;
  right: 1em;
}
</style>