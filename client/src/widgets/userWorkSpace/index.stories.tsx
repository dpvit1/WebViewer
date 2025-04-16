import React from 'react';
import type { Meta, StoryObj } from '@storybook/react';

import { UserWorkSpace } from '.';


const meta = {
  title: 'Components/UserWorkSpace',
  component: UserWorkSpace,
} satisfies Meta<typeof UserWorkSpace>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Primary: Story = {
  args: {},
  decorators: [(Story) => <Story />],
}
