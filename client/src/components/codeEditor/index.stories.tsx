import React from 'react';
import type { Meta, StoryObj } from '@storybook/react';

import { CodeEditor } from '.';


const meta = {
  title: 'Components/CodeEditor',
  component: CodeEditor,
} satisfies Meta<typeof CodeEditor>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Primary: Story = {
  args: {
    language: "javascript"
  },
  decorators: [(Story) => <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', height: '100%' }}>
    <p>
      todo: <a href="https://github.com/TypeFox/monaco-languageclient/tree/main/packages/examples/src/python" target='_blank'>автозаполнение и подсветка синтаксиса для питона </a>
    </p>
    <p>
      <a href='https://github.com/alankrantas/monaco-python-live-editor?tab=readme-ov-file' target='_blank'>Monaco Python Live Editor</a>
    </p>
    <p>
      <a href='https://www.npmjs.com/package/@monaco-editor/react' target='_blank'>npm пакет</a>
    </p>

    <div style={{ flexGrow: 1 }}>
      <Story height="100%" />
    </div>
  </div>],
}
