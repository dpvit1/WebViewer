import React from 'react';
import type { Meta, StoryObj } from '@storybook/react';

import { Viewer } from '.';


const meta = {
  title: 'Components/Viewer',
  component: Viewer,
} satisfies Meta<typeof Viewer>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Primary: Story = {
  args: {},
  decorators: [(Story) => <Story />],
}
