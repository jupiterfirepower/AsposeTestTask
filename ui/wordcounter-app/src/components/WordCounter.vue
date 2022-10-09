<template>
  <v-content>
    <v-container fill-height width="400">
      <v-card width="400">
        <v-toolbar color="teal" dark>
          <v-app-bar-nav-icon></v-app-bar-nav-icon>
          <v-toolbar-title>TOP WORDS DENSITY</v-toolbar-title>
          <v-btn icon>
            <v-icon>mdi-magnify</v-icon>
          </v-btn>
          <v-btn icon>
            <v-icon>mdi-dots-vertical</v-icon>
          </v-btn>
        </v-toolbar>
        <v-toolbar color="blue" dark>
          <v-row>
            <v-col>
              <v-container>
                <label>Top</label>&nbsp;
                <input
                  type="text"
                  id="topn"
                  name="topn"
                  required
                  minlength="1"
                  maxlength="3"
                  size="1"
                  v-model="data.topn"
                  @change="handleChange"
                  width="2px"
                />&nbsp;
                <label>Exclude Grammar</label>
              </v-container>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-container>
                <v-switch
                  color="success"
                  v-model="data.switch1"
                  hide-details
                  @click="handleSwitchChange"
                ></v-switch>
              </v-container>
            </v-col>
          </v-row>
          <template v-slot:extension>
            <v-tabs v-model="tab" align-with-title>
              <v-tab v-for="item in items" :key="item.tab" :value="item">
                {{ item.tab }}
              </v-tab>
            </v-tabs>
          </template>
        </v-toolbar>
        <v-window v-model="tab">
          <v-window-item v-for="itemt in items" :key="itemt.tab" :value="itemt">
            <v-card flat>
              <v-card-text>
                <div style="max-height: 25vh; overflow: auto">
                  <v-simple-table fixed-header height="300px">
                    <template v-slot:default>
                      <tbody>
                        <tr
                          v-for="(item, index) in (data.words as WordCount[]).filter((w)=>w.nWords===itemt.content).slice(0, data.topn)"
                          :key="index"
                        >
                          <td>{{ index + 1 + ". " + item.word }}</td>
                          <td>&nbsp;</td>
                          <td>&nbsp;</td>
                          <td>{{ item.summary }}</td>
                        </tr>
                      </tbody>
                    </template>
                  </v-simple-table>
                </div>
              </v-card-text>
            </v-card>
          </v-window-item>
        </v-window>
      </v-card>
    </v-container>
  </v-content>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { reactive, onMounted } from "vue";
import { getWordsSummary } from "../Words.api";
import store from "../store";
import * as signalR from "@microsoft/signalr";
import axios from "axios";

export default defineComponent({
  name: "WordCounterApp",
  emits: ["changeExludeGrammar"],
  methods: {
    emitToParent: function () {
      this.$emit("changeExludeGrammar", 3);
    },
    handleSwitchChange: function (evt: Event) {
      let switchv = (evt.target as HTMLInputElement).value;
      console.log("switch val - " + switchv);
      store.commit("changeExcludeGrammar", !store.getters.excludeGrammar);
      console.log("switch excludeGrammar - " + store.getters.excludeGrammar);
      let f = async () => {
        this.emitToParent();
      };
      f();
    },
  },
  setup() {
    const data = reactive({
      words: [],
      topn: store.getters.topn,
      switch1: store.getters.excludeGrammar,
      ntabwords: store.getters.nTabWords,
    });

    onMounted(async () => {
      axios.get("config.json").then((config) => {
        store.commit("changeNTabWords", config.data.ntabwords);
      });
      getRecords();
    });

    const getRecords = async () => {
      console.log(
        "getRecords corellationId -" + store.state.wordCountState.corellationId
      );
      console.log("getRecords excludeGrammar -" + store.getters.excludeGrammar);
      getWordsSummary(store.getters.corellationId, store.getters.excludeGrammar)
        .then((datares) => {
          data.words = datares as never[];
          data.switch1 = store.getters.excludeGrammar;
        })
        .catch((err: Error) => console.log(err));
    };

    const handleChange = (evt: Event) => {
      let topn = (evt.target as HTMLInputElement).value;
      console.log(topn);
      store.commit("changeTopN", topn);
    };

    return {
      data,
      handleChange,
    };
  },
  data: () => {
    return {
      tab: null,
      items: [
        { tab: "1 Word", content: 1 },
        { tab: "2 Words", content: 2 },
        { tab: "3 Words", content: 3 },
      ],
    };
  },
  created() {
    axios.get("config.json").then((response) => {
      let url = response.data.serviceGatewaySignalRUrl;
      console.info("signalr url from config.json - " + url);

      const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(response.data.serviceGatewaySignalRUrl)
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect()
        .build();

      store.commit("changeHubConnection", hubConnection);

      hubConnection.start().then((a) => {
        console.info("SignalR Connected.");
      });

      interface INotification {
        corellationId: string;
        created: string;
      }

      let counter = 0;

      hubConnection.on("nscevents", (message) => {
        console.info("hubConnection recived message : " + message);
        var notif: INotification = JSON.parse(message);
        console.info(
          "hubConnection recived corellationId : " + notif.corellationId
        );
        console.info("hubConnection counter++ : " + counter++);
        hubConnection.stop();
        if (store.getters.corellationId === notif.corellationId) {
          this.$emit("changeExludeGrammar", counter++);
        }
      });
    });
  },
});
</script>
