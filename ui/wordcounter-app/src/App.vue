<!-- eslint-disable prettier/prettier -->
<template>
  <div id="app">
    <v-form>
    <v-container>
      <v-row class="ma-2">
        <v-col cols="12" sm="6">
          <v-text-field
            label="Url"
            id="url"
            size="100"
            color="blue"
            outlined
          ></v-text-field>
        </v-col>
        <v-btn
          class="ma-2"
          :loading="loading"
          :disabled="loading"
          color="blue-grey"
          v-on:click="loadData"
        >
          Word Count
        </v-btn>
      </v-row>
    </v-container>
  </v-form>
  <div align="left"><wordCounter id="wcnt" :key="updateKey" @changeExludeGrammar="counterEvent($event)"/></div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import wordCounter from "./components/WordCounter.vue";
import store from "./store";
import { getCorellationId, getWordsSummary } from "./Words.api";

export default defineComponent({
  name: "WordCounterApp",
  components: { wordCounter },
  data: () => ({
    loader: 0,
    loading: false,
    updateKey: 0,
  }),
  // определяйте методы в объекте `methods`
  methods: {
    handleChange(evt: Event) {
      console.log((evt.target as HTMLInputElement).value);
    },
    reloadUpdate() {
      this.updateKey++;
    },
    counterEvent: function (val: number) {
      console.log(val);
      console.log(
        "counterEvent excludeGrammar - " + store.getters.excludeGrammar
      );
      this.reloadUpdate();
    },
    loadData(event: Event) {
      if (event) {
        const input = document.getElementById("url") as HTMLInputElement | null;
        const url = input?.value;

        console.log("url:" + url);
        console.log("old corellationId:" + store.getters.corellationId);
        if (url) {
          getCorellationId(url).then((res) => {
            store.commit("changeCorellationId", res);
            console.log("new corellationId:" + store.getters.corellationId);
          });
        }
      }
    },
  },
  watch: {
    loader() {
      const l = this.loader;
    },
  },
  created() {
    //this.  bus.("my-event", (evt) => {
    //  this.testEvent = evt.eventContent;
    //});
  },
});
</script>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  text-align: left;
  margin-top: 4px;
}

nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
</style>
