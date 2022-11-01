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
  <table>
    <tr>
      <th>Login</th>
      <th>Password</th>
      <th>Website</th>
      <th>Description</th>
      <th>Decrypt</th>
    </tr>
    <tr v-for="savedPassword in savedPasswords" :key="savedPassword.id">
      <td>{{savedPassword.login}}</td>
      <td>***</td>
      <td>{{savedPassword.webAddress}}</td>
      <td>{{savedPassword.description}}</td>
    </tr>
  </table>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}
</style>