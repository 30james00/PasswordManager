<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "CustomButton",
  props: {
    text: {
      type: String,
      required: true
    },
    bg: {
      type: String,
      required: false
    }
  },
})
</script>

<template>
  <div :class="$style.component" :style="{ 'background-color': bg }">
    {{ text }}
  </div>
</template>

<style lang="scss" module>
@import "@/scss/variables.scss";

.component {
  display: flex;
  cursor: pointer;
  border-radius: 0.5em;
  background-color: $light;
  margin: 0;
  padding: 0.75em 0.75em;
  transition: .7s;
  border: none;
  color: white;
  font-weight: bold;

  &:hover {
    box-shadow: inset 0 0 100px 100px rgba(255, 255, 255, 0.4);
  }
}
</style>