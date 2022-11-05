<script lang="ts">
import type { ILoginDto } from '@/models/accountModels';
import { useAccountStore } from '@/stores/accountStore';
import { defineComponent } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';

const toast = useToast();

export default defineComponent({
  name: "Login",
  data() {
    return {
      loginDto: {} as ILoginDto,
      accountStore: useAccountStore(),
    }
  },
  methods: {
    async handleSubmit(): Promise<void> {
      try {
        let response = await this.$axios.post('/account/login', this.loginDto);
        this.accountStore.login(response.data);
      } catch (e) {
        toast.error('Error logging in');
        return;
      }
    }
  }
});
</script>

<template>
  <form v-on:submit.prevent="handleSubmit" :class="$style.component">
    <input v-model="loginDto.login" required type="text" name="login" placeholder="Login">
    <input v-model="loginDto.password" required type="password" name="password" placeholder="Password">
    <input type="submit" name="submit" id="submit" value="Login">
  </form>
  <RouterLink class="nav-link" :to="{ name: 'register' }">Register instead</RouterLink>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}
</style>