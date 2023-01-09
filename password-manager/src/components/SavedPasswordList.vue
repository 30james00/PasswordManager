<script lang="ts">
import { defineComponent } from '@vue/runtime-dom';
import type { ISavedPassword } from '@/models/savedPasswordModels';
import { useToast } from 'vue-toastification';
import CustomButton from '@/components/CustomButton.vue';
import CustomIconButton from '@/components/CustomIconButton.vue';

const toast = useToast();

export default defineComponent({
  name: 'SavedPasswordList',
  data() {
    return {
      savedPasswords: [] as ISavedPassword[],
    };
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
    handleAddNew(): void {
      this.$router.push({ name: 'password-create' });
    },
    async handleDecrypt(savedPassword: ISavedPassword): Promise<void> {
      try {
        let response = await this.$axios.get(
          `savedPasswords/decrypt/${savedPassword.id}`
        );
        savedPassword.password = response.data;
      } catch (e) {
        toast.error(`Failed to decrypt ${savedPassword.login} password`);
        return;
      }
    },
    handleEdit(savedPassword: ISavedPassword): void {
      this.$router.push({
        name: 'password-edit',
        params: { passwordId: savedPassword.id },
      });
    },
    handleShare(savedPassword: ISavedPassword): void {
      this.$router.push({
        name: 'password-share',
        params: { passwordId: savedPassword.id },
      });
    },
    async handleDelete(savedPassword: ISavedPassword): Promise<void> {
      try {
        await this.$axios.delete(`savedPasswords/${savedPassword.id}`);
        this.handleRefresh();
      } catch (e) {
        toast.error(`Failed to delete ${savedPassword.login} password`);
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
    <CustomButton @click="handleAddNew" text="Add New Password" />
    <p v-if="savedPasswords.length == 0"><i>No Saved Passwords</i></p>
    <table v-if="savedPasswords.length > 0">
      <thead>
        <tr>
          <th scope="col">Login</th>
          <th scope="col">Password</th>
          <th scope="col">Website</th>
          <th scope="col">Description</th>
          <th scope="col">Decrypt</th>
          <th scope="col">Edit</th>
          <th scope="col">Share</th>
          <th scope="col">Delete</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="savedPassword in savedPasswords" :key="savedPassword.id">
          <td scope="row" data-label="Login">{{ savedPassword.login }}</td>
          <td data-label="Password">{{ savedPassword.password ?? '***' }}</td>
          <td data-label="Website">{{ savedPassword.webAddress }}</td>
          <td data-label="Description">{{ savedPassword.description }}</td>
          <td data-label="Decrypt" @click="handleDecrypt(savedPassword)">
            <div :class="$style.tbutton">
              <CustomIconButton icon="fa-solid fa-key" />
            </div>
          </td>
          <td data-label="Edit" @click="handleEdit(savedPassword)">
            <div :class="$style.tbutton">
              <CustomIconButton icon="fa-solid fa-pen" bg="#279AF1" />
            </div>
          </td>
          <td data-label="Share" @click="handleShare(savedPassword)">
            <div :class="$style.tbutton">
              <CustomIconButton icon="fa-solid fa-share-nodes" bg="#2e0142" />
            </div>
          </td>
          <td data-label="Delete" @click="handleDelete(savedPassword)">
            <div :class="$style.tbutton">
              <CustomIconButton icon="fa-solid fa-trash" bg="red" />
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
  margin-top: 1.5em;
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
}
</style>
