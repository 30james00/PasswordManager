<script lang="ts">
import type { ISavedPassword } from '@/models/savedPasswordModels';
import { defineComponent } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';

import CustomIconButton from '@/components/CustomIconButton.vue';
import type { ISharedPasswordMiniDto } from '@/models/sharedPasswordModels';

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
    async refresh(): Promise<void> {
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
    async handleShare(): Promise<void> {
      try {
        await this.$axios.post('/sharedPasswords', {
          id: this.savedPassword.id,
          login: this.newShareLogin,
        });
        this.refresh();
      } catch (e) {
        this.toast.error('Error editing SavedPasswords');
        return;
      }
    },
    async handleDelete(sharedPassword: ISharedPasswordMiniDto): Promise<void> {
      try {
        await this.$axios.delete(`/sharedPasswords/${sharedPassword.id}`);
        this.refresh();
      } catch (e) {
        this.toast.error(`Error removing ${sharedPassword.login} share`);
        return;
      }
    },
  },
  async created() {
    await this.refresh();
  },
  components: { CustomIconButton },
});
</script>

<template>
  <div :class="$style.component">
    <h2>Share password to {{ savedPassword.webAddress }}</h2>
    <form v-on:submit.prevent="handleShare" :class="$style.form">
      <input
        v-model="newShareLogin"
        required
        type="text"
        name="login"
        placeholder="Login"
      />
      <input type="submit" name="submit" id="submit" value="Share" />
    </form>
    <h3>Password is already shared with:</h3>
    <div :class="$style.shareList">
      <div
        :class="$style.shareBox"
        v-for="share in savedPassword.sharedTo"
        :key="share.id"
      >
        <span :class="$style.login">{{ share.login }}</span>
        <CustomIconButton
          icon="fa-solid fa-trash"
          bg="red"
          @click="handleDelete(share)"
        />
      </div>
    </div>
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

.shareList {
  @include col;
  gap: 0.5em;
}

.shareBox {
  @include row;
  align-items: center;
  border: 3px solid $light;
  border-radius: 0.5em;
  padding: 0.4em 1em;
}

.login {
  padding-right: 1em;
}
</style>
