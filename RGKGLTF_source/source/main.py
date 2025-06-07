import struct
import operator
from gltflib import (
    GLTF, GLTFModel, Asset, Scene, Node, Mesh, Primitive, Attributes, Buffer, BufferView, Accessor, AccessorType,
    BufferTarget, ComponentType)
import base64
import RGKPY
from Model import Model

def Init():
    RGKPY.Common.Instance.Start()
    session = RGKPY.Common.Session()
    RGKPY.Common.Instance.CreateSession(session)
    global context
    context = RGKPY.Common.Context()
    session.CreateMainContext(context)
    global model
    model = Model()

def Shutdown():
    RGKPY.Common.Instance.End()

def CreateGLTF(FName: str):
    vertices = GetVertices()

    vertex_bytearray = bytearray()
    for vertex in vertices:
        for value in vertex:
            vertex_bytearray.extend(struct.pack('f', value))

    bytelen = len(vertex_bytearray)

    mins = [min([operator.itemgetter(i)(vertex) for vertex in vertices]) for i in range(3)]
    maxs = [max([operator.itemgetter(i)(vertex) for vertex in vertices]) for i in range(3)]

    model = GLTFModel(
        asset=Asset(version='2.0', copyright="Топ Системы"),
        scenes=[Scene(nodes=[0])],
        nodes=[Node(mesh=0)],
        meshes=[Mesh(primitives=[Primitive(attributes=Attributes(POSITION=0))])],
        buffers=[Buffer(byteLength=bytelen, uri=f"data:application/octet-stream;base64,{base64.b64encode(vertex_bytearray).decode('utf-8')}")],
        bufferViews=[BufferView(buffer=0, byteOffset=0, byteLength=bytelen, target=BufferTarget.ARRAY_BUFFER.value)],
        accessors=[Accessor(bufferView=0, byteOffset=0, componentType=ComponentType.FLOAT.value, count=len(vertices),
                            type=AccessorType.VEC3.value, min=mins, max=maxs)]
    )

    gltf = GLTF(model=model)
    gltf.export_gltf(FName, save_file_resources=False)

    print("Export success")

def GetVertices():
    bodies = model.GetBodies()

    faceterData = RGKPY.Generators.Faceter.Data()
    for body in bodies:
        faceterData.AddBody(body)
    faceterData.SetSideLengthTolerance(0.2)
    faceterData.SetNormalAngleTolerance(0.5)
    faceterData.SetMeshMode(RGKPY.Generators.Faceter.MeshMode.TriangleMeshing)
    faceterReport = RGKPY.Generators.Faceter.Report()

    result = RGKPY.Generators.Faceter.Create(context, faceterData, faceterReport)

    surfaceMesh = faceterReport.GetMesh()

    vertices = []

    for i in range(surfaceMesh.GetFaceCount()):
        index = RGKPY.Mesh.FaceIndex(i)
        triangle  = RGKPY.Mesh.Triangle3D()
        result = surfaceMesh.GetTriangle(index, triangle)
        if (result):
            vertex1 = triangle.pt1()
            vertex2 = triangle.pt2()
            vertex3 = triangle.pt3()
            vertices.append([vertex1.GetX(),
                            vertex1.GetY(), 
                            vertex1.GetZ()])
            vertices.append([vertex2.GetX(), 
                            vertex2.GetY(), 
                            vertex2.GetZ()])
            vertices.append([vertex3.GetX(), 
                            vertex3.GetY(), 
                            vertex3.GetZ()])
    return vertices

def ParseUserCode(user_code: str):

    execution_context = {
        "RGKPY": RGKPY,
        "model": model,
        "context": context
    }

    exec(user_code, execution_context)

    print("Successful python code execution")