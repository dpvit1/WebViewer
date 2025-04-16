import React from 'react'
import { Widgets } from 'tflex-docs-uiwec-kit'


type ViewerProps = {
  url?: string,
}

export const Viewer: React.FC<ViewerProps> = React.memo(({
  url
}) => {
  return (<Widgets.TFlexViewer3D url={url} style={{ width: '100%', height: '100%' }} />)
})
