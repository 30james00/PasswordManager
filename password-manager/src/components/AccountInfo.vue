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
  <div :class="$style.outer">
    <div :class="$style.inner">
      <font-awesome-icon :class="$style.icon" icon="fa-solid fa-user" />
      <RouterLink :class="$style.text" :to="{ name: accountStore.isLoggedIn ? 'account' : 'login' }">{{
          accountStore.isLoggedIn ?
            accountStore.account!.login : "Login"
      }}</RouterLink>
    </div>
    <span :class="$style.logout" v-if="accountStore.account != null" @click="accountStore.logout">Logout</span>
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.outer {
  @include col;
  align-self: flex-start;
  padding: .25em .5em;
  border: 3px solid $light;
  border-radius: .5em;
}

.inner {
  @include row;
  align-items: center;
  padding-bottom: .25em;
}

.text {
  color: $light;
  font-weight: bold;
  margin: .25em 0;
  text-decoration: none;
  cursor: pointer;
}

.logout {
  font-size: .85em;
  color: $light;
  font-weight: bold;
  margin-bottom: .25em;
}

.icon {
  color: $light;
  height: 1em;
  margin-right: .35em;
}

@media screen and (max-width: 375px) {
  .text {
    font-size: .85em;
  }
}

@media screen and (min-width: 600px) {
  .outer {
    position: absolute;
    top: 1em;
    right: 1em;
  }
}
</style>