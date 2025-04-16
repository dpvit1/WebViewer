import struct
import operator
from gltflib import (
    GLTF, GLTFModel, Asset, Scene, Node, Mesh, Primitive, Attributes, Buffer, BufferView, Accessor, AccessorType,
    BufferTarget, ComponentType)
import base64
import sys

def CreateGLTF(vertices: list, FName: str):
    #Создаем массив байтов
    vertex_bytearray = bytearray()
    for vertex in vertices:
        for value in vertex:
            vertex_bytearray.extend(struct.pack('f', value))

    #Получаем длину массива байтов для выделения памяти буфера
    bytelen = len(vertex_bytearray)

    #Находим минимальную и максимальную координаты
    mins = [min([operator.itemgetter(i)(vertex) for vertex in vertices]) for i in range(3)]
    maxs = [max([operator.itemgetter(i)(vertex) for vertex in vertices]) for i in range(3)]

    #Создаем GLTF Модель
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

def UserCodeToGLTF(path_to_includes : str, user_code: str, gltf_path: str):
    path_to_bin = path_to_includes + "\\bin"
    path_to_python_libs = path_to_includes + "\python\PythonLib"
    print("path_to_bin = ",path_to_bin)
    print("path_to_python_libs = ",path_to_python_libs)
    sys.path.insert(0,path_to_bin)
    sys.path.insert(1,path_to_python_libs)
    import RGKPY

    bodies, context = ParseUserCode(user_code=user_code)

    faceterData = RGKPY.Generators.Faceter.Data()
    for body in bodies:
        faceterData.AddBody(body)                                          #Добавляем тело
    faceterData.SetSideLengthTolerance(0.2)                                #Устанавливаем максимальную длину треугольника
    faceterData.SetNormalAngleTolerance(0.5)                               #Устанавливаем максимальный угол
    faceterData.SetMeshMode(RGKPY.Generators.Faceter.MeshMode.TriangleMeshing)   #Устанавливаем триагональный режим вычисления
    faceterReport = RGKPY.Generators.Faceter.Report()

    result = RGKPY.Generators.Faceter.Create(context, faceterData, faceterReport)    #Создаем сетку

    surfaceMesh = faceterReport.GetMesh()                                  #Получаем сетку из результата построения

    vertices = []                                                   #Массив вершин созданных треугольников

    for i in range(surfaceMesh.GetFaceCount()):                     #Проходимся по каждой грани фасетного тела
        index = RGKPY.Mesh.FaceIndex(i)
        triangle  = RGKPY.Mesh.Triangle3D()
        result = surfaceMesh.GetTriangle(index, triangle)           #Получаем треугольник
        if (result):
            vertex1 = triangle.pt1()                                #Получаем вершины треугольника
            vertex2 = triangle.pt2()
            vertex3 = triangle.pt3()
            vertices.append([vertex1.GetX(),                        #Записываем координаты вершин в массив
                            vertex1.GetY(), 
                            vertex1.GetZ()])
            vertices.append([vertex2.GetX(), 
                            vertex2.GetY(), 
                            vertex2.GetZ()])
            vertices.append([vertex3.GetX(), 
                            vertex3.GetY(), 
                            vertex3.GetZ()])

    CreateGLTF(vertices, gltf_path)

    RGKPY.Common.Instance.End()

def ParseUserCode(user_code: str):
    import RGKPY

    bodies = []
    model=[]
    execution_context = {
        "RGKPY": RGKPY,
        "model": model
    }

    exec(user_code,execution_context)

    for body in model:
        bodies.append(body)

    for line in user_code.splitlines():
        if ('Common.Context' in line):
            var_name = line[:line.find("=")].strip()
            context = execution_context[var_name]
            break

    return bodies, context