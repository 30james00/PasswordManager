<script lang="ts">
import type { IRegisterDto } from '@/models/accountModels';
import { useAccountStore } from '@/stores/counter';
import { defineComponent } from '@vue/runtime-dom';

export default defineComponent({
  name: "Register",
  data() {
    return {
      registerDto: {} as IRegisterDto,
      repeatPassword: '',
      accountStore: useAccountStore(),
    }
  },
  methods: {
    async handleSubmit(): Promise<void> {
      if (this.registerDto.password == this.repeatPassword) return;
      try {
        let responce = await this.$axios.post('/account/register', this.registerDto);
        this.accountStore.$patch({ account: responce.data });
      } catch (e) {
        console.log('Error registering new Account');
        return;
      }
    }
  }
});
</script>

<template>
  <form v-on:submit.prevent="handleSubmit" :class="$style.component">
    <input v-model="registerDto.login" required type="text" name="login" placeholder="Login">
    <input v-model="registerDto.password" required type="password" name="password" placeholder="Password">
    <input v-model="repeatPassword" type="password" name="repassword" required placeholder="Repeat password">
    <div :class="$style['label-row']">
      <input v-model="registerDto.isPasswordKeptAsHash" type="checkbox" class="checkbox" name="is-hash" id="is-hash">
      <label for="id-hash">Is password kept as hash?</label>
    </div>
    <input type="submit" name="submit" id="submit">
  </form>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
}

.label-row {
  @include row;
  align-items: center;
}
</style>