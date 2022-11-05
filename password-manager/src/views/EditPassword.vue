<script lang="ts">

import type { IEditPasswordDto } from '@/models/savedPasswordModels';
import { defineComponent, type PropType } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';

export default defineComponent({
  name: "EditPassword",
  data() {
    return {
      editPassword: {} as IEditPasswordDto,
      toast: useToast(),
    }
  },
  props: {
    passwordId: {
      type: String,
      required: true,
    }
  },
  methods: {
    async handleEdit(): Promise<void> {
      try {
        await this.$axios.patch('/savedPasswords', this.editPassword);
        this.$router.push({ name: 'password-list' });
      } catch (e) {
        this.toast.error('Error editing SavedPasswords');
        return;
      }
    }
  },
  async created() {
    try {
      let response = await this.$axios.get(`/savedPasswords/${this.passwordId}`);
      this.editPassword = response.data;
    } catch (e) {
      this.toast.error('Error loading SavedPassword');
      this.$router.push({ name: 'password-list' });
      return;
    }
  }
});
</script>

<template>
  <form v-on:submit.prevent="handleEdit" :class="$style.component">
    <input v-model="editPassword.login" required type="text" name="login" placeholder="Login">
    <input v-model="editPassword.password" required type="password" name="password" placeholder="Password">
    <input v-model="editPassword.webAddress" type="text" name="web-address" placeholder="Website">
    <input v-model="editPassword.description" type="text" name="description" placeholder="Description">
    <input type="submit" name="submit" id="submit">
  </form>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}
</style>