import { di, style } from "./decorators";

/** @type { import('@storybook/react').Preview } */
const preview = {
  decorators: [
    di,
    style,
  ],
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
    layout: 'fullscreen',
  },
};

export default preview;
