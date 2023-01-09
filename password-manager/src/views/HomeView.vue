<script lang="ts">
import { defineComponent } from '@vue/runtime-dom';
import { useToast } from 'vue-toastification';
import SavedPasswordList from '@/components/SavedPasswordList.vue';
import SharedPasswordList from '@/components/SharedPasswordList.vue';

const toast = useToast();

export default defineComponent({
  name: 'HomeView',
  components: { SavedPasswordList, SharedPasswordList },
});
</script>

<template>
  <div :class="$style.component">
    <h2>Saved Password</h2>
    <SavedPasswordList/>
    <h2>Saved Password</h2>
    <SharedPasswordList />
  </div>
</template>

<style lang="scss" module>
@import '@/scss/variables.scss';

.component {
  @include col;
  width: 100%;
}
</style>
