<script setup lang="ts">
import { RouterView } from "vue-router";
import AccountInfo from "./components/AccountInfo.vue";
</script>

<template>
  <header :class="$style.header">
    <div :class="$style.top">
      <img :class="$style.logo" src="@/assets/logo.svg" alt="Logo">
      <h1 :class="$style.title">PasswordManager</h1>
    </div>
    <AccountInfo />
  </header>
  <RouterView />
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.header {
  @include row;
  align-items: center;
  justify-content: space-between;
  margin-bottom: .5em;
  width: 100%;
}

.top {
  @include row;
  align-items: center;
  justify-content: flex-start;
}

.logo {
  max-width: 4em;
}

.title {
  /* Gradient */
  font-size: 1em;
  background: $gradient;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

@media screen and (min-width: 400px) {
  .title {
    font-size: 1.15em;
  }
}

@media screen and (min-width: 600px) {
  .header {
    @include col;
    margin-bottom: 0;
  }

  .top {
    @include col;
  }

  .logo {
    max-width: 200px;
  }

  .title {
    font-size: 2em;
  }
}
</style>
