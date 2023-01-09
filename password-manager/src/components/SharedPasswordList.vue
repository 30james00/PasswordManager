<script lang="ts">
import { defineComponent } from '@vue/runtime-dom';
import type { ISavedPassword } from '@/models/savedPasswordModels';
import { useToast } from 'vue-toastification';
import CustomButton from '@/components/CustomButton.vue';
import CustomIconButton from '@/components/CustomIconButton.vue';
import type { ISharedPassword } from '@/models/sharedPasswordModels';

const toast = useToast();

export default defineComponent({
  name: 'SharedPasswordList',
  data() {
    return {
      sharedPasswords: [] as ISharedPassword[],
    };
  },
  methods: {
    async handleRefresh(): Promise<void> {
      try {
        let response = await this.$axios.get('/sharedPasswords');
        this.sharedPasswords = response.data;
      } catch (e) {
        toast.error('Error refreshing SavedPasswords');
        return;
      }
    },
    async handleDecrypt(sharedPassword: ISharedPassword): Promise<void> {
      try {
        let response = await this.$axios.get(
          `sharedPasswords/decrypt/${sharedPassword.id}`
        );
        sharedPassword.password = response.data;
      } catch (e) {
        toast.error(`Failed to decrypt ${sharedPassword.webAddress} password`);
        return;
      }
    },
  },
  async created() {
    await this.handleRefresh();
  },
  components: { CustomButton, CustomIconButton },
});
</script>

<template>
  <div :class="$style.component">
    <p v-if="sharedPasswords.length == 0"><i>No Shared Passwords</i></p>
    <table v-if="sharedPasswords.length > 0">
      <thead>
        <tr>
          <th scope="col">Login</th>
          <th scope="col">Password</th>
          <th scope="col">Website</th>
          <th scope="col">Description</th>
          <th scope="col">Decrypt</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="sharedPassword in sharedPasswords" :key="sharedPassword.id">
          <td scope="row" data-label="Login">{{ sharedPassword.login }}</td>
          <td data-label="Password">{{ sharedPassword.password ?? '***' }}</td>
          <td data-label="Website">{{ sharedPassword.webAddress }}</td>
          <td data-label="Description">{{ sharedPassword.description }}</td>
          <td data-label="Decrypt" @click="handleDecrypt(sharedPassword)">
            <div :class="$style.tbutton">
              <CustomIconButton icon="fa-solid fa-key" />
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
  width: 100%;
}

.tbutton {
  @include col;
}

@media screen and (max-width: 700px) {
  .tbutton {
    align-items: flex-end;
  }
}
</style>

<style lang="scss" scoped>
@import '@/scss/variables.scss';

table {
  border-collapse: collapse;
  padding: 0;
}

table tr {
  padding: 0.35em;
}

table th,
table td {
  padding: 0.625em;
  text-align: center;
}

table th {
  color: $light;
}

@media screen and (max-width: 700px) {
  table {
    border: 0;
    width: 100%;
  }

  table thead {
    border: none;
    clip: rect(0 0 0 0);
    height: 1px;
    margin: -1px;
    overflow: hidden;
    padding: 0;
    position: absolute;
    width: 1px;
  }

  table tr {
    border: 3px solid $light;
    display: block;
    margin-bottom: 0.625em;
    border-radius: 1em;
  }

  table td {
    @include row;
    align-items: center;
    border-bottom: 1px solid #ddd;
    color: $light;
    font-size: 0.8em;
    justify-content: space-between;
  }

  table td::before {
    content: attr(data-label);
    font-weight: bold;
  }

  table td:last-child {
    border-bottom: 0;
  }

  .tbutton {
    align-items: flex-end;
  }
}
</style>