<script lang="ts">

import { defineComponent } from '@vue/runtime-dom';
import type { ICreatePasswordDto, ISavedPassword } from '@/models/savedPasswordModels'
import { useToast } from 'vue-toastification';

export default defineComponent({
  name: "CreatePassword",
  data() {
    return {
      createPassword: {} as ICreatePasswordDto,
    }
  },
  methods: {
    async handleCreate(): Promise<void> {
      try {
        await this.$axios.post('/savedPasswords', this.createPassword);
        this.$router.push({ name: 'password-list' });
      } catch (e) {
        useToast().error('Error saving new Password');
        return;
      }
    }
  }
});
</script>

<template>
  <form v-on:submit.prevent="handleCreate" :class="$style.component">
    <input v-model="createPassword.login" required type="text" name="login" placeholder="Login">
    <input v-model="createPassword.password" required type="password" name="password" placeholder="Password">
    <input v-model="createPassword.webAddress" type="text" name="web-address" placeholder="Website">
    <input v-model="createPassword.description" type="text" name="description" placeholder="Description">
    <input type="submit" name="submit" id="submit">
  </form>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}
</style>