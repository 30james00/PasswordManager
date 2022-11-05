<script lang="ts">

import { defineComponent } from '@vue/runtime-dom';
import type { ISavedPassword } from '@/models/savedPasswordModels'
import { useToast } from 'vue-toastification';

const toast = useToast();

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
        let response = await this.$axios.get('/savedPasswords');
        this.savedPasswords = response.data;
      } catch (e) {
        toast.error('Error refreshing SavedPasswords');
        return;
      }
    },
    async handleDecrypt(savedPassword: ISavedPassword): Promise<void> {
      try {
        let response = await this.$axios.get(`savedPasswords/decrypt/${savedPassword.id}`);
        savedPassword.password = response.data;
      }
      catch (e) {
        toast.error(`Failed to decrypt ${savedPassword.login} password`);
        return;
      };
    },
    handleEdit(savedPassword: ISavedPassword): void {
      this.$router.push({ name: 'password-edit', params: { passwordId: savedPassword.id } })
    },
    async handleDelete(savedPassword: ISavedPassword): Promise<void> {
      try {
        await this.$axios.delete(`savedPasswords/${savedPassword.id}`);
        this.handleRefresh()
      }
      catch (e) {
        toast.error(`Failed to delete ${savedPassword.login} password`);
        return;
      }
    },
  },
  async created() {
    await this.handleRefresh();
  }
});
</script>

<template>
  <RouterLink :to="{ name: 'password-create' }">Save password</RouterLink>
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
      <td>{{ savedPassword.login }}</td>
      <td>{{ savedPassword.password ?? "***" }}</td>
      <td>{{ savedPassword.webAddress }}</td>
      <td>{{ savedPassword.description }}</td>
      <td @click="handleDecrypt(savedPassword)">Decrypt</td>
      <td @click="handleEdit(savedPassword)"> Edit</td>
      <td @click="handleDelete(savedPassword)">Delete</td>
    </tr>
  </table>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}
</style>