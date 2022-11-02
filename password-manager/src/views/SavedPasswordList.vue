<script lang="ts">

import { defineComponent } from '@vue/runtime-dom';
import type { ISavedPassword } from '@/models/savedPasswordModels'

export default defineComponent({
  name: "SavedPasswordList",
  data() {
    return {
      savedPasswords: [] as ISavedPassword[],
    }
  },
  methods: {
    async handleRefresh(): Promise<void> {
      try {
        let responce = await this.$axios.get('/savedPasswords');
        this.savedPasswords = responce.data;
      } catch (e) {
        console.log('Error refreshing SavedPasswords');
        return;
      }
    }
  },
  async created() {
    await this.handleRefresh();
  }
});
</script>

<template>
  <RouterLink :to="{name: 'password-create'}">Save password</RouterLink>
  <table>
    <tr>
      <th>Login</th>
      <th>Password</th>
      <th>Website</th>
      <th>Description</th>
      <th>Decrypt</th>
      <th>Edit</th>
      <th>Delete</th>
    </tr>
    <tr v-for="savedPassword in savedPasswords" :key="savedPassword.id">
      <td>{{savedPassword.login}}</td>
      <td>***</td>
      <td>{{savedPassword.webAddress}}</td>
      <td>{{savedPassword.description}}</td>
      <td>Decrypt</td>
      <td>Edit</td>
      <td>Delete</td>
    </tr>
  </table>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}
</style>