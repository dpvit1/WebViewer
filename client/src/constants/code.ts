const prismCode = `
RGKPY.Common.Instance.Start()
session = RGKPY.Common.Session()
RGKPY.Common.Instance.CreateSession(session)
context = RGKPY.Common.Context()
session.CreateMainContext(context)

side = RGKPY.Math.Vector3D(1, 1, 1)
lcs = RGKPY.Math.LCS3D()

data = RGKPY.Generators.PrimitiveGenerator.PrismData(0, 1.0, 1.5, 5, lcs, False, False)
report = RGKPY.Generators.PrimitiveGenerator.PrismReport()
result = RGKPY.Generators.PrimitiveGenerator.CreatePrism(context, data, report)
body1 = report.GetBody()

data = RGKPY.Generators.PrimitiveGenerator.BoxData(0, lcs, side, False)
report = RGKPY.Generators.PrimitiveGenerator.BoxReport()
result = RGKPY.Generators.PrimitiveGenerator.CreateBox(context, data, report)
body2 = report.GetBody()

func = RGKPY.Generators.Boolean.Function.Subtract
data = RGKPY.Generators.Boolean.Data(0, body1, body2, func)
report = RGKPY.Generators.Boolean.Report()
result = RGKPY.Generators.Boolean.Run(context, data, report)

body3 = report.GetBody(0)
model.append(body3)
`

const box = `
RGKPY.Common.Instance.Start()
session = RGKPY.Common.Session()
RGKPY.Common.Instance.CreateSession(session)
context = RGKPY.Common.Context()
session.CreateMainContext(context)

side = RGKPY.Math.Vector3D(1, 1, 1)
lcs = RGKPY.Math.LCS3D()

boxData = RGKPY.Generators.PrimitiveGenerator.BoxData(0, lcs, side, False)
boxReport = RGKPY.Generators.PrimitiveGenerator.BoxReport()
result = RGKPY.Generators.PrimitiveGenerator.CreateBox(context, boxData, boxReport)
body = boxReport.GetBody()

edges = body.GetEdges()
data = RGKPY.Generators.EdgeBlend.Data(1)
report = RGKPY.Generators.EdgeBlend.Report()
blendingOptions = RGKPY.Generators.EdgeBlend.Data.EdgeBlendingOptions.CreateConstRollingBall(0.15)
data.AddEdges(edges, blendingOptions)
result = RGKPY.Generators.EdgeBlend.Run(context, data, report)

model.append(body)
`

const complex = `
RGKPY.Common.Instance.Start()
session = RGKPY.Common.Session()
RGKPY.Common.Instance.CreateSession(session)
context = RGKPY.Common.Context()
session.CreateMainContext(context)

lcs1 = RGKPY.Math.LCS3D(RGKPY.Math.Vector3D(0.0, 0.0, 0.0))
lcs2 = RGKPY.Math.LCS3D(RGKPY.Math.Vector3D(0.0, 0.0, 1.0))
lcs3 = RGKPY.Math.LCS3D(RGKPY.Math.Vector3D(0.0, 0.0, 2.0))

circle1 = RGKPY.Geometry.Circle()
circle2 = RGKPY.Geometry.Circle()
circle3 = RGKPY.Geometry.Circle()

result = RGKPY.Geometry.Circle.Create(context, lcs1, 0.4, circle1)
result = RGKPY.Geometry.Circle.Create(context, lcs2, 1.5, circle2)
result = RGKPY.Geometry.Circle.Create(context, lcs3, 1.0, circle3)

interval1 = RGKPY.Math.Interval()
result = circle1.GetInterval(interval1)
interval2 = RGKPY.Math.Interval()
result = circle1.GetInterval(interval2)
interval3 = RGKPY.Math.Interval()
result = circle1.GetInterval(interval3)

curveData1 = RGKPY.Generators.WireBodyCreator.CurveData(0)
curveReport1 = RGKPY.Generators.WireBodyCreator.CurveReport()
curveData1.AddCurve(circle1, interval1, True)
result = RGKPY.Generators.WireBodyCreator.Create(context, curveData1, curveReport1)
body1 = curveReport1.GetBody()

curveData2 = RGKPY.Generators.WireBodyCreator.CurveData(0)
curveReport2 = RGKPY.Generators.WireBodyCreator.CurveReport()
curveData2.AddCurve(circle2, interval2, True)
result = RGKPY.Generators.WireBodyCreator.Create(context, curveData2, curveReport2)
body2 = curveReport2.GetBody()

curveData3 = RGKPY.Generators.WireBodyCreator.CurveData(0)
curveReport3 = RGKPY.Generators.WireBodyCreator.CurveReport()
curveData3.AddCurve(circle3, interval3, True)
result = RGKPY.Generators.WireBodyCreator.Create(context, curveData3, curveReport3)
body3 = curveReport3.GetBody()


section1 = RGKPY.Generators.Loft.Data.Section(body1, body1.GetVertices()[0], False)
section2 = RGKPY.Generators.Loft.Data.Section(body2, body2.GetVertices()[0], False)
section3 = RGKPY.Generators.Loft.Data.Section(body3, body3.GetVertices()[0], False)
sections = [ section1, section2, section3 ]

data = RGKPY.Generators.Loft.Data(1, sections)
report = RGKPY.Generators.Loft.Report()
result = RGKPY.Generators.Loft.Run(context, data, report)

body = report.GetBody()
model.append(body)
`

export const CODE_EXAMPLES = {
  PRISM: prismCode,
  BOX: box,
  COMPLEX: complex,
}