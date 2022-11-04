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
  max-width: 50%;
}

.title {
  /* Gradient */
  background: linear-gradient(135deg, #F20000 0%, #C00000 0.01%, #230000 100%, #230000 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  text-fill-color: transparent;
}
</style>
