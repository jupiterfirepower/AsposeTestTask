import { createStore } from "vuex";

export default createStore({
  state: {
    wordCountState: {
      corellationId: "6cd7de5d-819b-4b5b-9bc4-1de54deb8807",
      data: [],
      topn: 5,
      excludeGrammar: false,
      hubConnection: null,
      setupContext: null,
    },
  },
  getters: {
    corellationId: (state) => {
      return state.wordCountState.corellationId;
    },
    data: (state) => {
      return state.wordCountState.data;
    },
    topn: (state) => {
      return state.wordCountState.topn;
    },
    excludeGrammar: (state) => {
      return state.wordCountState.excludeGrammar;
    },
    hubConnection: (state) => {
      return state.wordCountState.hubConnection;
    },
    setupContext: (state) => {
      return state.wordCountState.setupContext;
    },
  },
  mutations: {
    changeCorellationId(state, guid) {
      state.wordCountState.corellationId = guid;
      console.info("changeCorellationId guid - " + guid);
    },
    changeTopN(state, top) {
      state.wordCountState.topn = top;
      console.info("change topn - " + top);
    },
    changeExcludeGrammar(state, excludeGrammar) {
      state.wordCountState.excludeGrammar = excludeGrammar;
      console.info("changeExcludeGrammar excludeGrammar - " + excludeGrammar);
    },
    changeHubConnection(state, hub) {
      state.wordCountState.hubConnection = hub;
      //console.info("changeHubConnection excludeGrammar - " + excludeGrammar);
    },
    changeSetupContext(state, context) {
      state.wordCountState.setupContext = context;
      //console.info("changeHubConnection excludeGrammar - " + excludeGrammar);
    },
    UPDATE_WORD_ITEMS(state, payload) {
      state.wordCountState.data = payload;
    },
  },
  actions: {},
  modules: {},
});
