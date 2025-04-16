import React from 'react';
import { StoryFn } from "@storybook/react";

import type { IServerConfig } from 'tflex-docs-uiwec-kit/models-view';
import { ApiService } from 'tflex-docs-uiwec-kit/services';
import { container } from 'tsyringe';
import { FileService, GltfConverterService, SERVICES_NAMES } from '../../src/services';

fetch('/config.json').then(res => res.json()).then((config: IServerConfig) => {

  const apiService = new ApiService({ BaseUrl: config.BaseUrl });
  container.register(ApiService, { useValue: apiService });
  container.register(SERVICES_NAMES.GLTF_CONVERTER, { useValue: new GltfConverterService(apiService) });
  container.register(SERVICES_NAMES.FILE, { useValue: new FileService(apiService) });
})

export const di = (Story: StoryFn) => {
  return <Story />
}