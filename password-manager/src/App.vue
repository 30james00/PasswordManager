<script setup lang="ts">
import { RouterView } from 'vue-router';
import AccountInfo from './components/AccountInfo.vue';
</script>

<template>
  <header :class="$style.header">
    <RouterLink class="nav-link" :to="{ name: 'home' }">
      <div :class="$style.top">
        <img :class="$style.logo" src="@/assets/logo.svg" alt="Logo" />
        <h1 :class="$style.title">PasswordManager</h1>
      </div>
    </RouterLink>
    <AccountInfo />
  </header>
  <router-view v-slot="{ Component }">
    <transition name="fade" mode="out-in">
      <component :is="Component" />
    </transition>
  </router-view>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.header {
  @include row;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.5em;
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
  background: $gradient;
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  font-size: 1.15em;
  margin-left: 0.5em;
}

@media screen and (min-width: 400px) {
  .title {
    font-size: 1.25em;
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
    margin-left: 0;
  }
}
</style>
