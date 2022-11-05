<script setup lang="ts">
import { RouterView } from "vue-router";
import AccountInfo from "./components/AccountInfo.vue";
</script>

<template>
  <header :class="$style.component">
    <img :class="$style.logo" src="@/assets/logo.svg" alt="Logo">
    <h1 :class="$style.title">PasswordManager</h1>
  </header>
  <AccountInfo />
  <RouterView />
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
  align-items: center;
}

.logo {
  max-width: 200px;
}

.title {
  /* Gradient */
  background: $gradient;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
</style>
