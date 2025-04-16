import { IApiService } from "tflex-docs-uiwec-kit/services";
import { API_URLS } from "../constants";

export class GltfConverterService {
  private readonly _url = API_URLS.GLTF_CONVERTER;

  constructor(private apiService: IApiService) { }

  public async create(fileName: string, userCode: string) {
    return this.apiService.post({
      path: this._url,
      body: JSON.stringify({ fileName, code: userCode }),
      options: { credentials: 'omit', },
    });
  }
}
