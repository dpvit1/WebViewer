import path from 'path';
import webpack, { RuleSetRule } from 'webpack';
import CopyWebpackPlugin from 'copy-webpack-plugin';

/** @type { import('@storybook/react-webpack5').StorybookConfig } */
const config = {
  stories: ["../src/**/*.mdx", "../src/**/*.stories.@(js|jsx|mjs|ts|tsx)"],
  addons: [
    "@storybook/addon-webpack5-compiler-swc",
    "@storybook/addon-onboarding",
    "@storybook/addon-essentials",
    "@chromatic-com/storybook",
    "@storybook/addon-interactions",
  ],
  framework: {
    name: "@storybook/react-webpack5",
    options: {},
  },
  core: {
    builder: '@storybook/builder-webpack5',
  },
  staticDirs: ['./static'],
  previewHead: (head: string) => `
    ${head}
    <base href="/" />
    <link rel="stylesheet" type="text/css" href="./css/tflex-theme-light.css">
    <link rel="stylesheet" type="text/css" href="./css/configure-css-variables.css">
  `,
  previewBody: (body: string) => `
    ${body}
    <script src="./reflect/Reflect.js"></script>
    <script src="./tflex-viewer/tflex-viewer3d-widget.js"></script>
  `,

  webpackFinal: async (config: webpack.Configuration) => {
    config.resolve!.alias = {
      ...config.resolve!.alias,
      '@': path.resolve(__dirname, '../src'),
      react: path.resolve('./node_modules/react'),
      'react-dom': path.resolve('./node_modules/react-dom'),
      'react-router': path.resolve('./node_modules/react-router'),
      'react-router-dom': path.resolve('./node_modules/react-router-dom'),
      'quill': path.resolve('./node_modules/quill'),
    };

    const plugins = config.plugins ?? [];
    const modeValue = getModeValue(process.argv);

    plugins.push(
      new CopyWebpackPlugin({
        patterns: [
          {
            from: path.resolve(__dirname, '..', 'node_modules', "tflex-docs-uiwec-kit", "dist", "tflex-themes", 'tflex-theme-light.css'),
            to: path.resolve(__dirname, 'static', 'css', 'tflex-theme-light.css')
          },
          {
            from: path.resolve(__dirname, '..', 'src', 'styles', 'configure-css-variables.css'),
            to: path.resolve(__dirname, 'static', 'css', 'configure-css-variables.css')
          },
          {
            from: path.resolve(__dirname, `config.${modeValue}.json`),
            to: path.resolve(__dirname, 'static', 'config.json')
          },
          {
            from: path.resolve(__dirname, '..', 'node_modules', 'reflect-metadata', 'Reflect.js'),
            to: path.resolve(__dirname, 'static', 'reflect', 'Reflect.js')
          },
          {
            from: path.resolve(__dirname, '..', '..', 'resources', 'tflex-viewer'),
            to: path.resolve(__dirname, 'static', 'tflex-viewer')
          },
        ]
      })
    );

    config.module!.rules!.push({
      test: /\.s[ac]ss$/i,
      use: ['style-loader', 'css-loader', 'sass-loader'],
    });

    config.plugins = plugins;
    config.resolve!.extensions!.push('.ts', '.tsx');

    config.devtool = 'source-map';

    return config;
  },
};
export default config;

function getModeValue(args: string[]) {
  const modeElement = args.find(item => item.startsWith('--mode='));
  if (modeElement) {
    return modeElement.split('=')[1];
  }
  return undefined;
}
