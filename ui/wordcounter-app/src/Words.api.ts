import axios, { AxiosInstance, AxiosRequestConfig } from "axios";

export default class WordsApiService {
  private axiosInstance: AxiosInstance;
  private serviceUrl: string;

  constructor() {
    this.serviceUrl = "http://localhost:5241/Gateway";
    axios.get("config.json").then((config) => {
      console.info(
        "config.data.serviceGatewayUrl - " + config.data.serviceGatewayUrl
      );
      this.serviceUrl = config.data.serviceGatewayUrl;
    });
    this.axiosInstance = axios.create({
      baseURL: this.serviceUrl,
    });
  }

  private async axiosCall<T>(config: AxiosRequestConfig) {
    try {
      const { data } = await this.axiosInstance.request<T>(config);
      return [null, data];
    } catch (error) {
      return [error];
    }
  }

  async getWordsStatistics(corellationId: string, excludeGrammar: boolean) {
    return this.axiosCall<WordCount[]>({
      method: "get",
      url: `${corellationId}?excludedGrammar=${excludeGrammar}`,
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
    });
  }

  async getCorellationId(url: string) {
    const data = JSON.stringify(url);
    return this.axiosCall<string>({
      method: "post",
      data: data,
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
    });
  }
}

export const getWordsSummary = async (
  corellationId: string,
  excludeGrammar: boolean
): Promise<WordCount[]> => {
  try {
    const wordsApiService = new WordsApiService();
    const [, words] = await wordsApiService.getWordsStatistics(
      corellationId,
      excludeGrammar
    );
    return words as WordCount[];
  } catch (error) {
    const result = (error as Error).message;
    throw new Error(result);
  }
};

export const getCorellationId = async (url: string): Promise<string> => {
  try {
    const wordsApiService = new WordsApiService();
    const [, guid] = await wordsApiService.getCorellationId(url);
    return guid as string;
  } catch (error) {
    const result = (error as Error).message;
    throw new Error(result);
  }
};
