<script lang="ts">
import type { IChangePasswordDto, IRegisterDto } from '@/models/accountModels';
import { useAccountStore } from '@/stores/accountStore';
import { defineComponent } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';

const toast = useToast();

export default defineComponent({
  name: "ChangePassword",
  data() {
    return {
      changePasswordDto: {} as IChangePasswordDto,
      repeatPassword: '',
      accountStore: useAccountStore(),
    }
  },
  methods: {
    async handleSubmit(): Promise<void> {
      if (this.changePasswordDto.newPassword != this.repeatPassword) {
        toast.error("New passwords doesn't match");
        return;
      }
      try {
        let response = await this.$axios.patch('/account/change-password', this.changePasswordDto);
        this.accountStore.login(response.data);
      } catch (e) {
        toast.error('Error changing password');
        return;
      }
    }
  }
});
</script>

<template>
  <form v-on:submit.prevent="handleSubmit" :class="$style.component">
    <input v-model="changePasswordDto.oldPassword" required type="password" name="old-password"
      placeholder="Old Password">
    <input v-model="changePasswordDto.newPassword" required type="password" name="new-password"
      placeholder="New Password">
    <input v-model="repeatPassword" type="password" name="repassword" required placeholder="Repeat New Password">
    <div :class="$style['label-row']">
      <input v-model="changePasswordDto.isPasswordKeptAsHash" type="checkbox" class="checkbox" name="is-hash"
        id="is-hash">
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