<script lang="ts">
import type { ISavedPassword } from '@/models/savedPasswordModels';
import { defineComponent } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';

export default defineComponent({
  name: 'SharePassword',
  data() {
    return {
      savedPassword: {} as ISavedPassword,
      newShareLogin: '',
      toast: useToast(),
    };
  },
  props: {
    passwordId: {
      type: String,
      required: true,
    },
  },
  methods: {
    async handleEdit(): Promise<void> {
      try {
        await this.$axios.patch('/savedPasswords', {
          id: this.savedPassword.id,
          login: this.newShareLogin,
        });
        this.$router.push({ name: 'home' });
      } catch (e) {
        this.toast.error('Error editing SavedPasswords');
        return;
      }
    },
  },
  async created() {
    try {
      let response = await this.$axios.get(
        `/savedPasswords/${this.passwordId}`
      );
      this.savedPassword = response.data;
    } catch (e) {
      this.toast.error('Error loading SavedPassword');
      this.$router.push({ name: 'password-list' });
      return;
    }
  },
});
</script>

<template>
  <div :class="$style.component">
    <h2>Share password to {{ savedPassword.webAddress }}</h2>
    <form v-on:submit.prevent="handleEdit" :class="$style.form">
      <input
        v-model="newShareLogin"
        required
        type="text"
        name="login"
        placeholder="Login"
      />
      <input type="submit" name="submit" id="submit" value="Share" />
    </form>
    <p>Password is already shared with:</p>
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}

.form {
  @include row;
  gap: 0.5em;
}
</style>
