<script lang="ts">
import type { IRegisterDto } from '@/models/accountModels';
import { useAccountStore } from '@/stores/accountStore';
import { defineComponent } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';

const toast = useToast();

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
      if (this.registerDto.password != this.repeatPassword) {
        toast.error("Passwords doesn't match");
        return;
      }
      try {
        let response = await this.$axios.post('/account/register', this.registerDto);
        this.accountStore.login(response.data);
      } catch (e) {
        toast.error('Error registering new Account');
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
    <input type="submit" name="submit" id="submit" value="Register">
  </form>
  <RouterLink class="nav-link" :to="{ name: 'login' }">Login instead</RouterLink>

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