import { IApiService } from "tflex-docs-uiwec-kit/services";
import { urlJoin } from "url-join-ts";
import { API_URLS } from "../constants";

export class FileService {
  private readonly _url = API_URLS.FILES

  constructor(private apiService: IApiService) { }

  public GetUrl(fileId: string): string {
    const baseUrl = this.apiService.ApiUrl;
    return urlJoin(baseUrl, this._url, fileId);
  }
}