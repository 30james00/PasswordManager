<script lang="ts">
import type { ILoginDto } from '@/models/accountModels';
import { useAccountStore } from '@/stores/counter';
import { defineComponent } from '@vue/runtime-dom';

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
        let responce = await this.$axios.post('/account/login', this.loginDto);
        this.accountStore.$patch({ account: responce.data });
      } catch (e) {
        console.log('Error loging in');
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
    <input type="submit" name="submit" id="submit">
  </form>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}
</style>