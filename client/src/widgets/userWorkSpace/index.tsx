import React, { useState } from 'react'
import { ReflexContainer, ReflexElement, ReflexSplitter } from 'react-reflex';
import { CodeEditor, Viewer } from '@/components';
import { Components } from 'tflex-docs-uiwec-kit';
import { CODE_EXAMPLES } from '../../constants';
import { container } from 'tsyringe';
import { FileService, GltfConverterService, SERVICES_NAMES } from '../../services';

type UserWorkSpaceProps = {}

export const UserWorkSpace: React.FC<UserWorkSpaceProps> = ({

}) => {
  const [code, setCode] = useState<string>();
  const [name, setName] = useState<string>("Current");
  const [url, setUrl] = useState<string>();
  const gltfService = container.resolve<GltfConverterService>(SERVICES_NAMES.GLTF_CONVERTER);
  const fileService = container.resolve<FileService>(SERVICES_NAMES.FILE);

  return (<div style={{ height: '100%', width: '100%', display: 'flex', flexDirection: 'column', gap: '1rem', padding: '1rem' }}>
    <ReflexContainer orientation='vertical' style={{ height: 'calc(100% - 2rem)' }}>
      <ReflexElement>
        <Viewer url={url} />
      </ReflexElement>
      <ReflexSplitter />
      <ReflexElement>
        <CodeEditor value={code} onChange={(value) => setCode(value)} />
      </ReflexElement>
    </ReflexContainer>
    <div style={{ width: '100%', display: 'flex', gap: '1rem', height: '2.5rem' }}>
      <div style={{ flexGrow: 1, display: 'flex', gap: '1rem', alignItems: 'center' }}>
        <span>Шаблоны:</span>
        <Components.TFlexButton textOption={{ caption: 'BOX', isVisible: true }} onClick={() => {
          setCode(CODE_EXAMPLES.BOX);
          setName('BOX');
        }} />
        <Components.TFlexButton textOption={{ caption: 'PRISM', isVisible: true }} onClick={() => {
          setCode(CODE_EXAMPLES.PRISM);
          setName('PRISM');
        }} />
        <Components.TFlexButton textOption={{ caption: 'COMPLEX', isVisible: true }} onClick={() => {
          setCode(CODE_EXAMPLES.COMPLEX);
          setName('COMPLEX');
        }} />
      </div>
      <div style={{ textAlign: 'right' }}>
        {code && name ? <Components.TFlexButton textOption={{ caption: 'Конвертировать', isVisible: true }} style={{ width: '100%', }} onClick={async () => {
          try {
            await gltfService.create(name, code);
            const url = fileService.GetUrl(name);
            setUrl(url);
          } catch (e) {
            alert(e);
          }

        }} /> : <></>}
      </div>
    </div>
  </div>)
}
