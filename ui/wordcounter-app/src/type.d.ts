interface IWordCount {
  word: string;
  summary: string;
}

interface INotification {
  corelationId: string;
  created: string;
}

type WordCount = {
  word: string;
  summary: string;
  nWords: number;
};
