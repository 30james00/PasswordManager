<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
    name: "CustomIconButton",
    props: {
        icon: {
            type: String,
            required: true
        },
        bg: {
            type: String,
            required: false
        }
    }
})
</script>

<template>
  <div :class="$style.component" :style="{ 'background-color': bg }">
    <font-awesome-icon :class="$style.icon" :icon="icon" />
  </div>
</template>

<style lang="scss" module>
@import "@/scss/variables.scss";

.component {
  @include col;
  height: 1em;
  width: 1em;
  background-color: $light;
  border: none;
  border-radius: 0.5em;
  color: white;
  cursor: pointer;
  font-weight: bold;
  margin: 0;
  padding: 0.5em 0.5em;
  transition: .4s;

  &:hover {
    box-shadow: inset 0 0 100px 100px rgba(255, 255, 255, 0.4);
  }

}
</style>