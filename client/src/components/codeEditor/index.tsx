import React from 'react'
import Editor, { type OnChange } from '@monaco-editor/react';


type CodeEditorProps = {
  language?: 'javascript' | 'python' | 'html',
  defaultValue?: string,
  value?: string,
  onChange?: OnChange,
  height?: string,
}

export const CodeEditor: React.FC<CodeEditorProps> = React.memo(({
  language, defaultValue, value, onChange, height,
}) => {
  return (
    <>
      <Editor
        height={height ?? "100%"} language={language ?? 'python'}
        defaultValue={defaultValue}
        value={value}
        onChange={onChange}
      />
    </>
  )
})
