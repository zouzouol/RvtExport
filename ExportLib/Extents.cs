
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExportLib
{

    public static class Configuration
    {
        public const double Precision = 0.1;
        public static DisplayUnitType CacheDisplayUnitType = DisplayUnitType.DUT_MILLIMETERS;
        public static DisplayUnitType DisplayUnitType
        {
            get
            {
                return CacheDisplayUnitType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        public static void SetUintType(Document document)
        {
            try
            {
                CacheDisplayUnitType = document.GetDisplayUnitType();
            }
            catch
            { }
        }
    }

    public static class DoubleExt
    {
        private const double Precision = 0.0001;

        /// <summary>
        /// 另一半弧长
        /// </summary>
        /// <param name="arcLength"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetAnotherArcLength(this double arcLength, double radius)
        {
            return (2 * Math.PI * radius) - arcLength;
        }

        /// <summary>
        /// 弧长对应的圆心角角度
        /// </summary>
        /// <param name="arcLength"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetAngle(this double arcLength, double radius)
        {
            return arcLength / ((Math.PI * radius) / 180);
        }

        /// <summary>
        /// 弧长对应的圆心角弧度
        /// </summary>
        /// <param name="arcLength"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetRadian(this double arcLength, double radius)
        {
            return arcLength.GetAngle(radius).ToRadian();
            //double angle = arcLength / ((Math.PI * radius) / 180);
            //return angle.ToRadian();
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double ToAngle(this double v)
        {
            return v * (180d / Math.PI);
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double ToRadian(this double v)
        {
            return v * (Math.PI / 180d);
        }

        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsEqual(this double d1, double d2, double tolerance = 0)
        {
            if (tolerance == 0)
            {
                tolerance = Precision;
            }
            double diff = Math.Abs(d1 - d2);
            return diff <= tolerance;
        }

        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsLess(this double d1, double d2, double tolerance = 0)
        {
            if (tolerance == 0)
            {
                tolerance = Precision;
            }
            double diff = d2 - d1;
            return diff > tolerance;
        }

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool IsLessEq(this double d1, double d2, double tolerance = 0)
        {
            return d1.IsLess(d2, tolerance) || d1.IsEqual(d2, tolerance);

        }

        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsThan(this double d1, double d2, double tolerance = 0)
        {
            if (tolerance == 0)
            {
                tolerance = Precision;
            }
            double diff = d1 - d2;
            return diff > tolerance;
        }

        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool IsThanEq(this double d1, double d2, double tolerance = 0.01)
        {
            return d1.IsThan(d2, tolerance) || d1.IsEqual(d2, tolerance);
        }

        /// <summary>
        /// 介于之间
        /// </summary>
        /// <param name="d"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool IsBetween(this double d, double d1, double d2)
        {
            if (d.IsEqual(d1, 0.1) || d.IsEqual(d2, 0.1))
            {
                return true;
            }
            double dMin = Math.Min(d1, d2);
            double dMax = Math.Max(d1, d2);
            if (d.IsThanEq(dMin) && d.IsLessEq(dMax))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 相近的值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static double NearValue(this double d, double d1, double d2)
        {
            double dSub1 = Math.Abs(d - d1);
            double dSub2 = Math.Abs(d - d2);
            if (dSub1.IsLessEq(dSub2))
            {
                return d1;
            }
            return d2;
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="p1X"></param>
        /// <param name="p1Y"></param>
        /// <param name="p2X"></param>
        /// <param name="p2Y"></param>
        /// <returns></returns>
        public static double FindDistance(double p1X, double p1Y, double p2X, double p2Y)
        {
            double x = p1X - p2X;
            double y = p1Y - p2Y;
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="p1X"></param>
        /// <param name="p1Y"></param>
        /// <param name="p1Z"></param>
        /// <param name="p2X"></param>
        /// <param name="p2Y"></param>
        /// <param name="p2Z"></param>
        /// <returns></returns>
        public static double FindDistance(double p1X, double p1Y, double p1Z, double p2X, double p2Y, double p2Z)
        {
            double x = p1X - p2X;
            double y = p1Y - p2Y;
            double z = p1Z - p2Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// 取整，主要毫米
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round(this double value)
        {
            return Math.Round(value);
        }

        /// <summary>
        /// 取整,主要用于解决0.0000003，0.99999998问题
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double Round(this double value, int digits)
        {
            return Math.Round(value, digits);
        }

        /// <summary>
        /// 取整,主要用于解决0.0000003，0.99999998问题
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double Round(this double value, int digits, MidpointRounding midPoint)
        {
            return Math.Round(value, digits, midPoint);
        }

        /// <summary>
        /// 向上取整 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundUp(this double value)
        {
            return (int)Math.Ceiling(value);
        }

        /// <summary>
        /// 向下取整 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundDown(this double value)
        {
            //直接去掉小数
            return (int)value;
        }

        /// <summary>
        /// 毫米转成米 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double MmToM(this double value)
        {
            return value / 1000;
        }

        /// <summary>
        /// 米转成毫米 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double MToMm(this double value)
        {
            return value * 1000;
        }

        /// <summary>
        /// 是否为零
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsZero(this double value, double tolerance = 0)
        {
            return value.IsEqual(0d, tolerance);
        }

        /// <summary>
        /// 是否为象限角
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsQuadrantAngle(this double value)
        {
            //九十度
            double dQ = (Math.PI / 2).ToAngle().Round();
            return (value.ToAngle().Round() % dQ).IsEqual(0);
        }

        /// <summary>
        /// 差值百分比
        /// </summary>
        /// <param name="dValue1"></param>
        /// <param name="dValue2"></param>
        /// <returns></returns>
        public static double SubtractPercent(this double dValue1, double dValue2)
        {
            //差值百分比=|最大值-最小值|/最大值
            double dMax = Math.Max(dValue1, dValue2);
            if (dMax.IsEqual(0))
            {
                return 0;
            }

            return (Math.Abs(dValue1 - dValue2) / dMax).Round(3);
        }

        /// <summary>
        /// 超配率
        /// </summary>
        /// <param name="dFactValue1">实配</param>
        /// <param name="dCountValue2">计算</param>
        /// <returns></returns>
        public static double OverflowPercent(this double dFactValue1, double dCountValue2)
        {
            //超配率 =（实配-计算）/计算
            if (dCountValue2.IsEqual(0))
            {
                return 0;
            }

            return (dFactValue1 - dCountValue2) / dCountValue2;
        }



        /// <summary>
        /// 平方
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Square(this double value)
        {
            return value * value;
        }


        public static bool IsNotZero(this double value, double tolerance = 0.0)
        {
            return !value.IsZero(tolerance);
        }


        public static int ToInt(this double value)
        {
            int result = (int)value;
            return result;
        }
    }

    public static class XyzExt
    {

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        private static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        /// Return a string for an XYZ point
        /// or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(this XYZ p)
        {
            return string.Format("({0},{1},{2})",
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }
        public static XYZ NewX(this XYZ p1, double x = 0.0)
        {
            double y = p1.Y;
            double z = p1.Z;
            return new XYZ(x, y, z);
        }

        public static XYZ AddX(this XYZ p1, double x)
        {
            double y = p1.Y;
            double z = p1.Z;
            return new XYZ(p1.X + x, y, z);
        }

        public static XYZ NewY(this XYZ p1, double y = 0.0)
        {
            double x = p1.X;
            double z = p1.Z;
            return new XYZ(x, y, z);
        }

        public static XYZ AddY(this XYZ p1, double y)
        {
            double x = p1.X;
            double z = p1.Z;
            return new XYZ(x, p1.Y + y, z);
        }

        public static XYZ NewZ(this XYZ p1, double dZ = 0.0)
        {
            double x = p1.X;
            double y = p1.Y;
            return new XYZ(x, y, dZ);
        }

        public static XYZ AddZ(this XYZ p1, double z)
        {
            double x = p1.X;
            double y = p1.Y;
            return new XYZ(x, y, p1.Z + z);
        }

        /// <summary>
        /// 向量绕任意轴旋转 
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="origin">旋转所绕点</param>
        /// <param name="axis">旋转所绕的轴</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static XYZ VectorRotate(this XYZ vec, XYZ origin, XYZ axis, double angle)
        {
            Transform transform = Transform.CreateRotationAtPoint(axis, angle, origin);
            return transform.OfVector(vec);
        }

        /// <summary>
        /// 向量(点)绕任意轴旋转
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="axis">参照轴</param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static XYZ VectorRotate(this XYZ vec, XYZ axis, double angle)
        {
            return vec.VectorRotate(XYZ.Zero, axis, angle);
        }

        /// <summary>
        /// 两个坐标是否相等(包含Z坐标)
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsEqual(this XYZ first, XYZ second, double tolerance = 0.0)
        {
            return first.X.IsEqual(second.X, tolerance) &&
                first.Y.IsEqual(second.Y, tolerance) &&
                first.Z.IsEqual(second.Z, tolerance);
        }

        /// <summary>
        /// 创建直线
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Line CreateBoundExt(XYZ pt1, XYZ pt2)
        {
            double shortCurveTolerance = 0.001;
            if (!pt1.IsEqual(pt2, shortCurveTolerance))
            {
                return Line.CreateBound(pt1, pt2);
            }
            return null;
        }

    }

    public static class EnumerableExtend
    {

        public static int GetCount(this IEnumerable items)
        {
            return items.Cast<object>().Count();
        }

        public static object[] GetArrary(this IEnumerable items)
        {
            return items.Cast<object>().ToArray();
        }
        /// <summary>
        /// 获取T类型的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T[] GetArraryT<T>(this IEnumerable items)
        {
            return (from object value in items select (T)value).ToArray();
        }
        /// <summary>
        /// 获取指定项
        /// </summary>
        /// <param name="items"></param>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static object GetByIndex(this IEnumerable items, int intIndex)
        {
            int index = 0;
            foreach (object value in items)
            {
                if (intIndex == index)
                {
                    return value;
                }
                index++;
            }
            return null;
        }

        public static T GetByIndexT<T>(this IEnumerable items, int intIndex)
        {
            var item = items.GetByIndex(intIndex);
            return (T)item;
        }

        public static List<T> ToList<T>(this IEnumerable items)
        {
            return items.OfType<T>().ToList();
        }
        /// <summary>
        /// 非null,并且cout>0
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsNotNullEmptyExt(this IEnumerable items)
        {
            return !items.IsNullOrEmptyExt();
        }

        public static bool IsNullOrEmptyExt(this IEnumerable items)
        {
            if (items is string)
            {
                return string.IsNullOrEmpty((string)items);
            }
            if (items != null)
            {
                return (items.GetCount() == 0);
            }
            return true;
        }

    }

    public static class CurveExtend
    {

        /// <summary>
        /// 起点
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static XYZ StartPoint(this Curve curve)
        {
            return curve.GetEndPoint(0);
        }

        /// <summary>
        /// 终点
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static XYZ EndPoint(this Curve curve)
        {
            return curve.GetEndPoint(1);
        }

        /// <summary>
        /// 中点
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static XYZ MiddlePoint(this Curve curve)
        {
            if (!curve.IsBound)
            {
                return null;
            }
            double endParameter = curve.GetEndParameter(0);
            double endParameter2 = curve.GetEndParameter(1);
            return curve.Evaluate((endParameter2 + endParameter) / 2.0, false);
        }

        /// <summary>
        /// 创建直线
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Line CreateBoundExt(XYZ pt1, XYZ pt2)
        {
            double shortCurveTolerance = 0.001;
            if (!pt1.IsEqual(pt2, shortCurveTolerance))
            {
                return Line.CreateBound(pt1, pt2);
            }
            return null;
        }

        /// <summary>
        /// 曲线坐标转换  2015-10-27
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static Curve OfCurve(this Transform tf, Curve curve)
        {
            Curve result = curve;
            XYZ xYZ = tf.OfPoint(curve.StartPoint());
            XYZ xYZ2 = tf.OfPoint(curve.EndPoint());
            if (curve is Line)
            {
                result = CreateBoundExt(xYZ, xYZ2);
            }
            else if (curve is Arc)
            {
                XYZ pointOnArc = tf.OfPoint(curve.MiddlePoint());
                result = Arc.Create(xYZ, xYZ2, pointOnArc);
            }
            return result;
        }

        /// <summary>
        /// 线
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static Line GetLine(this Curve curve)
        {
            return curve as Line;
        }

        /// <summary>
        /// 弧
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static Arc GetArc(this Curve curve)
        {
            return curve as Arc;
        }

        /// <summary>
        /// 创建模型线 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static ModelCurve NewModelCurveExt(this Line line, Document doc, SketchPlane sp = null)
        {
            if (!line.IsBound)
                return null;

            if (sp == null)
            {
                XYZ xYZ = XYZ.BasisZ;
                XYZ direction = line.Direction;
                if (xYZ.IsEqual(direction, 0.0) || xYZ.IsEqual(-direction, 0.0))
                {
                    xYZ = XYZ.BasisY;
                }
                XYZ normal = direction.CrossProduct(direction.VectorRotate(xYZ, Math.PI / 2));
                Plane plane = Plane.CreateByNormalAndOrigin(normal, line.StartPoint());
                sp = SketchPlane.Create(doc, plane);
            }
            return doc.Create.NewModelCurve(line, sp);
        }


        public static ModelCurve NewModelCurveExt(this Arc arc, Document doc, SketchPlane sp = null)
        {
            if (sp == null)
            {
                XYZ normal = arc.Normal;
                Plane plane = Plane.CreateByNormalAndOrigin(normal, arc.StartPoint());
                sp = SketchPlane.Create(doc, plane);
            }
            return doc.Create.NewModelCurve(arc, sp);
        }

        /// <summary>
        /// 生成模型线 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static ModelCurve NewModelCurveExt(this Curve curve, Document doc, SketchPlane sp = null)
        {
            if (curve is Line)
            {
                return curve.GetLine().NewModelCurveExt(doc, sp);
            }
            if (curve is Arc)
            {
                return curve.GetArc().NewModelCurveExt(doc, sp);
            }
            return null;
        }
    }

    /// <summary>
    /// 文档对象扩展
    /// </summary>
    public static class DocumentExtend
    {

        /// <summary>
        /// 类别相等
        /// </summary>
        /// <param name="cat1"></param>
        /// <param name="cat2"></param>
        /// <returns></returns>
        /// 
        public static bool IsEqual(this Category cat1, Category cat2)
        {
            return cat1.Id.IsEqual(cat2.Id);
        }
        /// <summary>
        /// 相等 
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="bic"></param>
        /// <returns></returns>
        public static bool IsEqual(this Category cat, BuiltInCategory bic)
        {
            return cat != null && cat.Id.IntegerValue == (int)bic;
        }

        /// <summary>
        /// 通过ID整数值判断相等
        /// </summary>
        /// <param name="elemId1"></param>
        /// <param name="elemId2"></param>
        /// <returns></returns>
        public static bool IsEqual(this ElementId elemId1, ElementId elemId2)
        {
            return elemId1.IntegerValue == elemId2.IntegerValue;
        }

        /// <summary>
        /// 通过元素Id值判断相等
        /// </summary>
        /// <param name="elem1"></param>
        /// <param name="elem2"></param>
        /// <returns></returns>
        /// 相等
        public static bool IsEqual(this Element elem1, Element elem2)
        {
            return elem2 != null && elem1.Id.IsEqual(elem2.Id);
        }

        /// <summary>
        /// 名字查找View
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="strViewName"></param>
        /// <param name="vt"></param>
        /// <param name="blnNormal">false视图样板</param>
        /// <returns></returns>
        public static Autodesk.Revit.DB.View GetView(this Document doc, string strViewName = "", ViewType vt = ViewType.Undefined, bool blnNormal = true)
        {
            List<Autodesk.Revit.DB.View> views = doc.GetViews(blnNormal);
            Autodesk.Revit.DB.View result;
            if (strViewName.IsNullOrEmptyExt())
            {
                Autodesk.Revit.DB.View arg_63_0;
                if (vt != ViewType.Undefined)
                {
                    arg_63_0 = views.Find((Autodesk.Revit.DB.View p) => p.ViewType == vt);
                }
                else
                {
                    arg_63_0 = views[0];
                }
                result = arg_63_0;
            }
            else
            {
                Autodesk.Revit.DB.View arg_BE_0;
                if (vt != ViewType.Undefined)
                {
                    arg_BE_0 = views.FindAll((Autodesk.Revit.DB.View p) => p.ViewType == vt).Find((Autodesk.Revit.DB.View p) => p.Name == strViewName);
                }
                else
                {
                    arg_BE_0 = views.Find((Autodesk.Revit.DB.View p) => p.Name == strViewName);
                }
                result = arg_BE_0;
            }
            return result;
        }

        /// <summary>
        /// 所有常规视图
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="blnNormal">True排除样板</param>
        /// <returns></returns>
        public static List<Autodesk.Revit.DB.View> GetViews(this Document doc, bool blnNormal = true)
        {
            IList<Autodesk.Revit.DB.View> source = doc.FilterElements<View>();
            List<Autodesk.Revit.DB.View> arg_3B_0;
            if (!blnNormal)
            {
                arg_3B_0 = source.ToList<Autodesk.Revit.DB.View>();
            }
            else
            {
                arg_3B_0 = (from view in source
                            where view.IsNormal()
                            select view).ToList<Autodesk.Revit.DB.View>();
            }
            List<Autodesk.Revit.DB.View> source2 = arg_3B_0;
            return (from v in source2
                    orderby v.Name
                    select v).ToList<Autodesk.Revit.DB.View>();
        }
        /// <summary>
        /// 平面视图 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="vf"></param>
        /// <param name="blnNormal">false视图样板</param>
        /// <returns></returns>
        public static List<ViewPlan> GetViewPlans(this Document doc, ViewFamily vf, bool blnNormal = true)
        {
            List<ViewPlan> viewPlans = doc.GetViewPlans(blnNormal);
            List<ViewPlan> arg_4A_0;
            if (!blnNormal)
            {
                arg_4A_0 = viewPlans.ToList<ViewPlan>();
            }
            else
            {
                arg_4A_0 = (from view in viewPlans
                            where view.IsNormal()
                            select view).ToList<ViewPlan>();
            }
            List<ViewPlan> list = arg_4A_0;
            list.Sort(new ViewPlanCompare());
            if (vf != ViewFamily.Invalid)
            {
                list = viewPlans.FindAll((ViewPlan p) => p.GetViewFamily() == vf);
            }
            return list;
        }

        public static List<ViewPlan> GetViewPlans(this Document doc, Level level, bool blnNormal = true, ViewFamily vFamily = ViewFamily.Invalid)
        {
            if (level == null)
            {
                return null;
            }
            List<ViewPlan> list = (vFamily == ViewFamily.Invalid) ? doc.GetViewPlans(blnNormal) : doc.GetViewPlans(vFamily, blnNormal);
            return list.FindAll((ViewPlan p) => p.GenLevel != null && p.GenLevel.IsEqual(level));
        }

        /// <summary>
        /// 所有常规视图
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="blnNormal">True排除样板</param>
        /// <returns></returns>
        public static List<ViewPlan> GetViewPlans(this Document doc, bool blnNormal = true)
        {
            IList<ViewPlan> source = doc.FilterElements<ViewPlan>();
            List<ViewPlan> viewplans;
            if (!blnNormal)
            {
                viewplans = source.ToList<ViewPlan>();
            }
            else
            {
                viewplans = (from view in source
                             where view.IsNormal()
                             select view).ToList<ViewPlan>();
            }
            List<ViewPlan> source2 = viewplans;
            return (from v in source2
                    orderby v.Name
                    select v).ToList<ViewPlan>();
        }

        /// <summary>
        /// 视图类型取数据
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="vt">不区分建筑、结构</param>
        /// <param name="blnNormal">false视图样板</param>
        /// <returns></returns>
        public static List<Autodesk.Revit.DB.View> GetViews(this Document doc, ViewType vt, bool blnNormal = true)
        {
            List<Autodesk.Revit.DB.View> views = doc.GetViews(blnNormal);
            return views.FindAll((Autodesk.Revit.DB.View p) => p.ViewType == vt);
        }

        /// <summary>
        /// 视图类型取数据
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="vf">区分建筑、结构</param>
        /// <param name="blnNormal">false视图样板</param>
        /// <returns></returns>
        public static List<Autodesk.Revit.DB.View> GetViews(this Document doc, ViewFamily vf, bool blnNormal = true)
        {
            List<Autodesk.Revit.DB.View> views = doc.GetViews(blnNormal);
            return views.FindAll((Autodesk.Revit.DB.View p) => p.GetViewFamily() == vf);
        }

        /// <summary>
        /// 视图的类型
        /// </summary>
        /// <param name="view"></param>
        /// <returns>区分建筑结构</returns>
        public static ViewFamily GetViewFamily(this Autodesk.Revit.DB.View view)
        {
            ViewFamily result = ViewFamily.Invalid;
            ViewFamilyType viewFamilyType = view.GetElementType() as ViewFamilyType;
            if (viewFamilyType != null)
            {
                result = viewFamilyType.ViewFamily;
            }
            return result;
        }

        /// <summary>
        /// 图元类型
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ElementType GetElementType(this Element element)
        {
            ElementId typeId = element.GetTypeId();
            if (typeId != null && typeId != ElementId.InvalidElementId)
            {
                Element element2 = element.Document.GetElement(typeId);
                return element2 as ElementType;
            }
            return null;
        }


        /// <summary>
        /// 平面视图 
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static bool IsPlan(this Autodesk.Revit.DB.View view)
        {
            return view.ViewType == ViewType.EngineeringPlan ||
                view.ViewType == ViewType.FloorPlan ||
                view.ViewType == ViewType.CeilingPlan ||
                view.ViewType == ViewType.AreaPlan ||
                view.ViewType == ViewType.Detail ||
                view.ViewType == ViewType.DraftingView ||
                view.ViewType == ViewType.DrawingSheet ||
                view.ViewType == ViewType.Detail;
        }

        /// <summary>
        /// 常规视图 
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static bool IsNormal(this Autodesk.Revit.DB.View view)
        {
            return !view.IsTemplate && view.ViewType != ViewType.Internal;
        }

        /// <summary>
        /// rvt项目名称
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetShortTitle(this Document doc)
        {
            string text = doc.Title;
            string text2 = Path.GetExtension(text);
            if (!string.IsNullOrEmpty(text2))
            {
                if (!text2.StartsWith("."))
                {
                    text2 = "." + text2;
                }
                int num = text.IndexOf(text2, StringComparison.Ordinal);
                if (num > 0)
                {
                    text = text.Substring(0, num);
                }
            }
            if (text.Length <= 15)
            {
                return doc.Title;
            }
            if (!string.IsNullOrEmpty(text2))
            {
                return text.Substring(0, 12) + "..." + text2;
            }
            return text.Substring(0, 12) + "...";
        }

        /// <summary>
        /// 文档信息
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetPathNameOrTitle(this Document doc)
        {
            if (string.IsNullOrEmpty(doc.PathName))
            {
                return doc.Title;
            }
            return doc.PathName;
        }

        /// <summary>
        /// 文档所有图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="isOnlyInstance">默认只取实例图元</param>
        /// <returns></returns>
        public static List<Element> GetAllElements(this Document doc, bool isOnlyInstance = true)
        {
            if (isOnlyInstance)
            {
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
                return filteredElementCollector.ToElements().ToList<Element>();
            }
            FilteredElementCollector filteredElementCollector2 = new FilteredElementCollector(doc).WhereElementIsElementType();
            FilteredElementCollector other = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            FilteredElementCollector filteredElementCollector3 = filteredElementCollector2.UnionWith(other);
            return filteredElementCollector3.ToElements().ToList<Element>();
        }

        /// <summary>
        /// 获取一个项目中的所有链接模型构件
        /// </summary>
        /// <param name="allElem"></param>
        /// <returns></returns>
        public static List<Element> GetLinkElements(this IEnumerable<Element> allElem)
        {
            List<Element> list = new List<Element>();
            List<Element> list2 = allElem.ToList<Element>().FindAll((Element p) => p is RevitLinkInstance);
            foreach (Element current in list2)
            {
                RevitLinkInstance revitLinkInstance = current as RevitLinkInstance;
                if (revitLinkInstance != null)
                {
                    list.AddRange(revitLinkInstance.GetLinkElements());
                }
            }
            return list;
        }

        public static List<RevitLinkInstance> GetLinkInstances(this Document doc)
        {
            List<RevitLinkInstance> list = new List<RevitLinkInstance>();
            List<Element> list2 = doc.FilterElements(BuiltInCategory.OST_RvtLinks);
            if (list2.Count > 0)
            {
                foreach (Element current in list2)
                {
                    RevitLinkInstance revitLinkInstance = current as RevitLinkInstance;
                    if (revitLinkInstance != null)
                    {
                        list.Add(revitLinkInstance);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 取一个链接项目中的所有模型构件,递归调用
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<Element> GetLinkElements(this RevitLinkInstance link)
        {
            List<Element> list = new List<Element>();
            Document linkDocument = link.GetLinkDocument();
            if (linkDocument == null)
            {
                return list;
            }
            List<Element> allElements = linkDocument.GetAllElements(true);
            list.AddRange(allElements);
            list.AddRange(allElements.GetLinkElements());
            return list;
        }

        public static Category GetCategoryExt(this Document doc, BuiltInCategory category)
        {
            try
            {
                return doc.Settings.Categories.get_Item(category);
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// 子类型
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bic"></param>
        /// <returns></returns>
        public static List<Category> GetSubCategory(this Document doc, BuiltInCategory bic)
        {
            Category categoryExt = doc.GetCategoryExt(bic);
            return categoryExt.SubCategories.ToList<Category>();
        }

        /// <summary>
        /// 当前项目的长度单位
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static DisplayUnitType GetDisplayUnitType(this Document doc)
        {
            DisplayUnitType result;
            try
            {
                UnitType unitType = UnitType.UT_Length;
                Units units = doc.GetUnits();
                FormatOptions formatOptions = units.GetFormatOptions(unitType);
                result = formatOptions.DisplayUnits;
            }
            catch
            {
                result = DisplayUnitType.DUT_DECIMAL_FEET;
            }
            return result;
        }

        /// <summary>
        /// 过滤类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<T> FilterElements<T>(this Document doc, Predicate<T> condition) where T : Element
        {
            List<T> list = doc.FilterElements<T>();
            List<T> list2 = new List<T>();
            foreach (T current in list)
            {
                if (condition(current))
                {
                    list2.Add(current);
                }
            }
            return list2;
        }

        /// <summary>
        /// 类型过滤图元 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Element> FilterElements(this Document doc, Type type)
        {
            ElementClassFilter filter = new ElementClassFilter(type);
            return doc.FilterElements(filter);
        }

        /// <summary>
        /// 过滤器过滤图元 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Element> FilterElements(this Document doc, ElementFilter filter)
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            return filteredElementCollector.WherePasses(filter).ToList<Element>();
        }

        /// <summary>
        /// 分类过滤实例图元 
        ///             </summary>
        /// <param name="doc"></param>
        /// <param name="bic"></param>
        /// <returns></returns>
        public static List<Element> FilterElements(this Document doc, BuiltInCategory bic)
        {
            ElementCategoryFilter filter = new ElementCategoryFilter(bic);
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            return filteredElementCollector.WherePasses(filter).WhereElementIsNotElementType().ToList<Element>();
        }

        ///// <summary>
        ///// 分类过滤类型图元
        ///// </summary>
        ///// <param name="doc"></param>
        ///// <param name="bic"></param>
        ///// <returns></returns>
        //public static List<ElementType> FilterElementTypes(this Document doc, BuiltInCategory bic)
        //{
        //    return doc.FilterElementTypes(bic);
        //}

        /// <summary>
        /// 获取类型
        /// </summary>
        /// 2015-07-24
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="bic"></param>
        /// <returns></returns>
        public static List<T> FilterElementTypes<T>(this Document doc, BuiltInCategory bic)
        {
            ElementCategoryFilter filter = new ElementCategoryFilter(bic);
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            return filteredElementCollector.WherePasses(filter).WhereElementIsElementType().ToList<T>();
        }

        /// <summary>
        /// id返回图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="intId"></param>
        /// <returns></returns>
        public static Element GetElement(this Document doc, int intId)
        {
            ElementId id = new ElementId(intId);
            return doc.GetElement(id);
        }

        public static T GetElement<T>(this Document doc, ElementId id) where T : Element
        {
            return doc.GetElement(id) as T;
        }

        public static T GetElement<T>(this Document doc, string name) where T : Element
        {
            return doc.GetElement(name) as T;
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<T> GetElements<T>(this Document doc) where T : Element
        {
            return doc.FilterElements<T>();
        }

        /// <summary>
        /// 根据名称判断某类图元是否存在  2015-11-25
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static bool IsExistElement<T>(this Document doc, string strName) where T : Element
        {
            return doc.GetElements<T>().Exists((T p) => p.Name == strName);
        }

        /// <summary>
        /// 分类图元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="bic"></param>
        /// <returns></returns>
        public static List<T> GetElements<T>(this Document doc, BuiltInCategory bic) where T : Element
        {
            ElementCategoryFilter filter = new ElementCategoryFilter(bic);
            return doc.FilterElements<T>(filter);
        }

        /// <summary>
        /// 相交的图元，慢过滤 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static List<T> GetElements<T>(this Document doc, Element elem) where T : Element
        {
            ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(elem);
            return doc.FilterElements<T>(filter);
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, Type type)
        {
            return doc.FilterElements(type);
        }

        public static BuiltInCategory GetCategoryExt(this Element elem)
        {
            BuiltInCategory result = BuiltInCategory.INVALID;
            if (elem.Category != null)
            {
                result = (BuiltInCategory)elem.Category.Id.IntegerValue;
            }
            return result;
        }

        /// <summary>
        /// 通过类型获取图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="eleType"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, ElementType eleType)
        {
            BuiltInCategory categoryExt = eleType.GetCategoryExt();
            List<Element> list = (categoryExt != BuiltInCategory.INVALID) ? doc.FilterElements(categoryExt) : doc.GetAllElements(true);
            return list.FindAll((Element p) => p.GetElementType() != null && p.GetElementType().IsEqual(eleType));
        }

        public static BoundingBoxXYZ GetBoundingBoxExt(this Element element, View view = null, bool blnCheck = true)
        {
            return element.get_BoundingBox(view);
        }

        /// <summary>
        /// 与当前相交的图元，快速过滤 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elem"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, Element elem, double tolerance = 0.0)
        {
            BoundingBoxXYZ boundingBoxExt = elem.GetBoundingBoxExt(null, true);
            if (boundingBoxExt != null)
            {
                return doc.GetElements(boundingBoxExt, tolerance);
            }
            return new List<Element>();
        }

        /// <summary>
        /// 相交的图元 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="box"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, BoundingBoxXYZ box, double tolerance = 0.0)
        {
            Outline otl = new Outline(box.Min, box.Max);
            return doc.GetElements(otl, tolerance);
        }

        /// <summary>
        /// 根据 BoundingBox获取特定类别相交图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="box"></param>
        /// <param name="category">类别</param>
        /// <param name="tolerance">误差值</param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, BoundingBoxXYZ box, BuiltInCategory category, double tolerance = 0.0)
        {
            Outline outline = new Outline(box.Min, box.Max);
            BoundingBoxIntersectsFilter filter = tolerance.IsEqual(0.0, 0.0) ? new BoundingBoxIntersectsFilter(outline) : new BoundingBoxIntersectsFilter(outline, tolerance);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(category);
            LogicalAndFilter filter3 = new LogicalAndFilter(filter, filter2);
            return doc.GetElements(filter3);
        }

        /// <summary>
        /// 根据 BoundingBox获取特定类别相交图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="box"></param>
        /// <param name="category">类别</param>
        /// <param name="tolerance">误差值</param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, Outline outline, BuiltInCategory category, double tolerance = 0.0)
        {
            BoundingBoxIntersectsFilter filter = tolerance.IsEqual(0.0, 0.0) ?
                new BoundingBoxIntersectsFilter(outline) : new BoundingBoxIntersectsFilter(outline, tolerance);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(category);
            LogicalAndFilter filter3 = new LogicalAndFilter(filter, filter2);
            return doc.GetElements(filter3);
        }

        public static List<Element> GetElements(this Document doc, BoundingBoxXYZ box, ElementFilter filter, double tolerance = 0.0)
        {
            Outline outline = new Outline(box.Min, box.Max);
            BoundingBoxIntersectsFilter filter2 = tolerance.IsEqual(0.0, 0.0) ? new BoundingBoxIntersectsFilter(outline) : new BoundingBoxIntersectsFilter(outline, tolerance);
            LogicalAndFilter filter3 = new LogicalAndFilter(filter2, filter);
            return doc.GetElements(filter3);
        }

        public static List<Element> GetLinkElements(this Document doc, BoundingBoxXYZ box, ElementFilter filter, double tolerance = 0.0)
        {
            List<Element> result = new List<Element>();
            List<Element> elements = doc.GetElements(typeof(RevitLinkInstance));
            List<RevitLinkInstance> instectLinks = new List<RevitLinkInstance>();
            Outline boxLine = new Outline(box.Min, box.Max);
            elements.ForEach(delegate (Element p)
            {
                BoundingBoxXYZ boundingBoxExt = p.GetBoundingBoxExt(doc.ActiveView, true);
                Outline outline = new Outline(boundingBoxExt.Min, boundingBoxExt.Max);
                if (outline.Intersects(boxLine, tolerance) && p is RevitLinkInstance)
                {
                    instectLinks.Add(p as RevitLinkInstance);
                }
            });
            instectLinks.ForEach(delegate (RevitLinkInstance p)
            {
                result.AddRange(p.GetLinkDocument().GetElements(box, filter, 0.0));
            });
            return result;
        }

        /// <summary>
        /// 根据 BoundingBox获取指定类形相交图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="box"></param>
        /// <param name="type">指定类型</param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, BoundingBoxXYZ box, Type type, double tolerance = 0.0)
        {
            Outline otl = new Outline(box.Min, box.Max);
            return doc.GetElements(otl, tolerance).FindAll((Element p) => p.GetType() == type);
        }

        /// <summary>
        /// 相交的图元 
        /// 通过此方法可以找到相连接图元,如两个首尾相接的墙
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="otl"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, Outline otl, double tolerance = 0.0)
        {
            if (tolerance.IsEqual(0.0, 0.0))
            {
                BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(otl);
                return doc.FilterElements(filter);
            }
            BoundingBoxIntersectsFilter filter2 = new BoundingBoxIntersectsFilter(otl, tolerance);
            return doc.FilterElements(filter2);
        }

        /// <summary>
        /// 获取图元，快速过滤 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elem"></param>
        /// <param name="filter"></param>
        /// <param name="dSub"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, Element elem, ElementFilter filter, double dSub = 0.0)
        {
            BoundingBoxXYZ boundingBoxExt = elem.GetBoundingBoxExt(null, true);
            if (boundingBoxExt == null)
            {
                return new List<Element>();
            }
            XYZ source = new XYZ(dSub, dSub, dSub);
            Outline otl = new Outline(boundingBoxExt.Min.Add(source), boundingBoxExt.Max.Subtract(source));
            return doc.GetElements(otl, filter);
        }

        /// <summary>
        /// 获取图元，快速过滤 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="otl"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, Outline otl, ElementFilter filter)
        {
            if (otl.IsEmpty)
            {
                return new List<Element>();
            }
            BoundingBoxIntersectsFilter filter2 = new BoundingBoxIntersectsFilter(otl);
            LogicalAndFilter filter3 = new LogicalAndFilter(filter2, filter);
            return doc.FilterElements(filter3);
        }

        /// <summary>
        /// 过滤器过滤图元 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, ElementFilter filter)
        {
            return doc.FilterElements(filter);
        }

        /// <summary>
        /// 图元ID转Element
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, IEnumerable<ElementId> ids)
        {
            List<Element> list = new List<Element>();
            foreach (ElementId current in ids)
            {
                Element element = doc.GetElement(current);
                if (element != null)
                {
                    list.Add(element);
                }
            }
            return list;
        }

        /// <summary>
        /// 框选楼板用
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pickedBox">切记，PickedBox的Z轴有可能不是当前平面Z轴  2015-12-02</param>
        /// <param name="filter"></param>
        /// <param name="dZOffset"></param>
        /// <param name="dZExtend"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, PickedBox pickedBox, ElementFilter filter, double dZOffset, double dZExtend = 0.0)
        {
            double x = Math.Max(pickedBox.Max.X, pickedBox.Min.X);
            double y = Math.Max(pickedBox.Max.Y, pickedBox.Min.Y);
            double x2 = Math.Min(pickedBox.Max.X, pickedBox.Min.X);
            double y2 = Math.Min(pickedBox.Max.Y, pickedBox.Min.Y);
            double num = pickedBox.Min.Z + dZOffset.ToApi();
            double num2 = 110.0.ToApi();
            double num3 = 10.0.ToApi();
            if (!dZExtend.IsZero(0.0))
            {
                num2 = dZExtend.ToApi();
                num3 = dZExtend.ToApi();
            }
            double num4 = num - num2;
            double num5 = num + num3;
            if (num5 < num4)
            {
                num4 = num + num3;
                num5 = num - num2;
            }
            XYZ minimumPoint = new XYZ(x2, y2, num4);
            XYZ maximumPoint = new XYZ(x, y, num5);
            Outline outline = new Outline(minimumPoint, maximumPoint);
            ElementFilter filter2;
            if (filter != null)
            {
                filter2 = new LogicalAndFilter(filter, new BoundingBoxIntersectsFilter(outline));
            }
            else
            {
                filter2 = new BoundingBoxIntersectsFilter(outline);
            }
            return doc.FilterElements(filter2);
        }

        /// <summary>
        /// 过滤类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<T> FilterElements<T>(this Document doc, ElementFilter filter = null) where T : class
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            if (filter != null)
            {
                return filteredElementCollector.WherePasses(filter).OfClass(typeof(T)).ToList<T>();
            }
            return filteredElementCollector.OfClass(typeof(T)).ToList<T>();
        }

        /// <summary>
        /// FilteredElementCollector(Document document, ElementId viewId)
        /// </summary>
        /// <param name="view"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Element> GetVisibleElements(this View view, ElementFilter filter = null)
        {
            if (view.GetPrimaryViewId() != ElementId.InvalidElementId)
            {
                view = (View)view.Document.GetElement(view.GetPrimaryViewId());
            }
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(view.Document, view.Id);
            if (filter == null)
            {
                return filteredElementCollector.ToElements().ToList<Element>();
            }
            return filteredElementCollector.WherePasses(filter).ToElements().ToList<Element>();
        }

        /// <summary>
        /// 获取视图中两点构成的区域内可见图形
        /// </summary>
        /// <param name="view"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="elemType"></param>
        /// <returns></returns>
        public static List<Element> GetVisibleElements(this View view, XYZ pt1, XYZ pt2,
            BuiltInCategory elemType = BuiltInCategory.INVALID)
        {
            if (view.GetPrimaryViewId() != ElementId.InvalidElementId)
            {
                view = (View)view.Document.GetElement(view.GetPrimaryViewId());
            }
            FilteredElementCollector filter = new FilteredElementCollector(view.Document, view.Id);
            if (elemType != BuiltInCategory.INVALID)
            {
                filter = filter.OfCategory(elemType);
            }

            double elevation = view.GenLevel.Elevation;
            XYZ min = new XYZ(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y), elevation - 0.001);
            XYZ max = new XYZ(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y), elevation + 10.0);

            BoundingBoxIntersectsFilter intersectFilter = new BoundingBoxIntersectsFilter(new Outline(min, max));

            return filter.WherePasses(intersectFilter).ToList<Element>();
        }

        /// <summary>
        /// 获取视图中两点构成的区域内可见图形
        /// </summary>
        /// <param name="view"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="elemType"></param>
        /// <returns></returns>
        public static List<Element> GetVisibleElements(this View view, XYZ pt1, XYZ pt2,
            ElementFilter efilter = null)
        {
            if (view.GetPrimaryViewId() != ElementId.InvalidElementId)
            {
                view = (View)view.Document.GetElement(view.GetPrimaryViewId());
            }
            FilteredElementCollector filter = new FilteredElementCollector(view.Document, view.Id);
            if (efilter != null)
            {
                filter = filter.WherePasses(efilter);
            }

            double elevation = view.GenLevel.Elevation;
            //XYZ min = new XYZ(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y), elevation - 0.001);
            //XYZ max = new XYZ(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y), elevation + 10.0);
            XYZ min = new XYZ(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y), pt1.Z);
            XYZ max = new XYZ(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y), pt2.Z);

            BoundingBoxIntersectsFilter intersectFilter = new BoundingBoxIntersectsFilter(new Outline(min, max));

            return filter.WherePasses(intersectFilter).ToList();
        }

        /// <summary>
        /// FilteredElementCollector(Document document), filteredElementCollector.OwnedByView
        /// </summary>
        /// <param name="view"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Element> GetViewElements(this View view, ElementFilter filter = null)
        {
            if (view.GetPrimaryViewId() != ElementId.InvalidElementId)
            {
                view = (View)view.Document.GetElement(view.GetPrimaryViewId());
            }
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(view.Document);
            List<Element> list;
            if (filter == null)
            {
                list = filteredElementCollector.OwnedByView(view.Id).ToList<Element>();
            }
            else
            {
                list = filteredElementCollector.OwnedByView(view.Id).WherePasses(filter).ToList<Element>();
            }
            list.RemoveAll((Element p) => p.Category == null || p.Category.IsEqual(BuiltInCategory.OST_SunStudy));
            return list;
        }


        /// <summary>
        /// 点获取图元 ElementQuickFilter
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pt"></param>
        /// <param name="tolerance"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Element> GetElements(this Document doc, XYZ pt, double tolerance = 0.0, ElementFilter filter = null)
        {
            if (filter == null)
            {
                return doc.FilterElements(new BoundingBoxContainsPointFilter(pt, tolerance));
            }
            ElementFilter filter2 = new LogicalAndFilter(filter, new BoundingBoxContainsPointFilter(pt, tolerance));
            return doc.FilterElements(filter2);
        }

        public static List<SlabEdgeType> GetSlabEdgeTypes(this Document doc)
        {
            return doc.FilterElements<SlabEdgeType>();
        }

        public static List<RoomTagType> GetRoomTagTypes(this Document doc)
        {
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            filteredElementCollector.OfClass(typeof(FamilySymbol));
            filteredElementCollector.OfCategory(BuiltInCategory.OST_RoomTags);
            return filteredElementCollector.Cast<RoomTagType>().ToList<RoomTagType>();
        }

        public static List<T> GetElements<T>(this Document doc, IEnumerable<ElementId> elementIds) where T : Element
        {
            List<T> list = new List<T>();
            if (elementIds == null)
            {
                return list;
            }
            foreach (ElementId current in elementIds)
            {
                T element = doc.GetElement<T>(current);
                if (element != null)
                {
                    list.Add(element);
                }
            }
            return list;
        }

        public static void ActiveViewFocus(this UIApplication application)
        {
            try
            {
                View activeView = application.ActiveUIDocument.ActiveView;
                IList<UIView> openUIViews = application.ActiveUIDocument.GetOpenUIViews();
                foreach (UIView current in openUIViews)
                {
                    if (activeView == null || current.ViewId.IntegerValue != activeView.Id.IntegerValue)
                    {
                        View activeView2 = application.ActiveUIDocument.Document.GetElement(current.ViewId) as View;
                        application.ActiveUIDocument.ActiveView = activeView2;
                        application.ActiveUIDocument.ActiveView = activeView;
                        return;
                    }
                }
                List<View> views = application.ActiveUIDocument.Document.GetViews(true);
                foreach (View current2 in views)
                {
                    if (current2.Id.IntegerValue != activeView.Id.IntegerValue)
                    {
                        application.ActiveUIDocument.ActiveView = current2;
                        application.ActiveUIDocument.ActiveView = activeView;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 创建文字18版
        /// (18上支持不传递width)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pView"></param>
        /// <param name="origin"></param>
        /// <param name="textAlign"></param>
        /// <param name="strText"></param>
        /// <param name="noteType"></param>
        /// <returns></returns>
        public static TextNote NewTextNoteExt(this Document doc, View pView, XYZ origin,
            TextAlignFlags textAlign, string strText, TextNoteType noteType)
        {
            TextNote textNote = TextNote.Create(doc, pView.Id, origin, strText, noteType.Id);
            if (textNote != null)
            {
                Parameter parameter = textNote.get_Parameter(BuiltInParameter.TEXT_ALIGN_HORZ);
                if (!parameter.IsReadOnly)
                {
                    if (StorageType.Integer == parameter.StorageType)
                    {
                        parameter.Set(textAlign.GetHashCode());
                    }
                }
            }
            return textNote;
        }

        /// <summary>
        /// 转换成CurveArray 
        /// </summary>
        /// <param name="listCurve"></param>
        /// <returns></returns>
        public static CurveArray ToCurveArray(this IList<Curve> listCurve)
        {
            CurveArray curveArray = new CurveArray();
            foreach (Curve current in listCurve)
            {
                curveArray.Append(current);
            }
            return curveArray;
        }
        /// <summary>
        /// 生成模型线
        /// </summary>
        /// <param name="ca"></param>
        public static List<ModelCurve> NewModelCurveExt(this CurveArray ca)
        {
            List<ModelCurve> list = new List<ModelCurve>();
            foreach (Curve curve in ca)
            {
                //ModelCurve modelCurve = curve.NewModelCurveExt(null);
                //if (modelCurve != null)
                //{
                //    list.Add(modelCurve);
                //}
            }
            return list;
        }


        /// <summary>
        /// 测试solid生成的是不是正确。
        /// </summary>
        /// <param name="document"></param>
        /// <param name="elementFaceSolid"></param>
        public static void CreateLineForSolid(this Document document, Solid elementFaceSolid)
        {
            //测试，做几个model curve
            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("createModelCurve");
                /*
                foreach (CurveLoop surveLoop in shouldBeCalElementLC)
                {
                    foreach (Curve curve in surveLoop)
                    {
                        Creator.CreateModelCurve(curve);
                    }
                                            
                }
                 */

                foreach (Face face in elementFaceSolid.Faces)
                {

                    if (face == null)
                    {
                        continue;
                    }
                    Mesh mesh = face.Triangulate();
                    if (mesh == null)
                        continue;

                    if (mesh.Visibility == Visibility.Invisible)
                    {
                        continue;
                    }
                    //遍历mesh
                    for (int i = 0; i < mesh.NumTriangles; i++)
                    {
                        MeshTriangle triangle = mesh.get_Triangle(i);
                        XYZ vertex1 = triangle.get_Vertex(0);
                        XYZ vertex2 = triangle.get_Vertex(1);
                        XYZ vertex3 = triangle.get_Vertex(2);

                        List<XYZ> triangleList = new List<XYZ>();
                        triangleList.Add(vertex1);
                        triangleList.Add(vertex2);
                        triangleList.Add(vertex3);



                        Line l1 = Line.CreateBound(vertex1, vertex2);
                        Line l2 = Line.CreateBound(vertex2, vertex3);
                        Line l3 = Line.CreateBound(vertex3, vertex1);

                        List<Curve> list = new List<Curve>() { l1, l2, l3 };
                        list.ToCurveArray().NewModelCurveExt();
                    }

                }

                transaction.Commit();

            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class ViewPlanCompare : IComparer<ViewPlan>
    {
        /// <summary>
        ///             由低到高排序
        /// </summary>
        /// <param name="level1"></param>
        /// <param name="level2"></param>
        /// <returns></returns>
        public int CompareTo(Level level1, Level level2)
        {
            if (level1.Elevation.IsEqual(level2.Elevation, 0.0))
            {
                return 0;
            }
            if (level1.Elevation <= level2.Elevation)
            {
                return -1;
            }
            return 1;
        }

        public int Compare(ViewPlan view1, ViewPlan view2)
        {
            return CompareTo(view1.GenLevel, view2.GenLevel);
        }
    }


    public enum DrivenTypes
    {
        NotSupport = 0,
        Point,
        Curve,
        Face
    }

    /// <summary>
    /// 单位转换
    /// </summary>
    public static class DoubleExtend
    {
        /// <summary>
        /// 英制转公制单位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FromApi(this double value)
        {
            return value.FromApi(Configuration.DisplayUnitType);
        }

        public static double FromApi2(this double value)
        {
            return value.FromApi(Configuration.DisplayUnitType, 2);
        }

        public static double FromApi(this double value, int intType)
        {
            return value.FromApi(Configuration.DisplayUnitType, intType);
        }

        public static double FromApi(this double value, DisplayUnitType displayType)
        {
            switch (displayType)
            {
                case DisplayUnitType.DUT_FAHRENHEIT:
                    return value * ImperialDutRatio(displayType) - 459.67;
                case DisplayUnitType.DUT_CELSIUS:
                    return value - 273.15;
                default:
                    return value *= ImperialDutRatio(displayType);
            }
        }

        /// <summary>
        /// 转平方米
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FromApiToSquareM(this double value)
        {
            return value.FromApi(DisplayUnitType.DUT_SQUARE_METERS);
        }

        /// <summary>
        /// 转米
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FromApiToM(this double value)
        {
            return value.FromApi(DisplayUnitType.DUT_METERS);
        }

        public static double FromApi(this double value, DisplayUnitType displayType, int intType)
        {
            return value *= Math.Pow(ImperialDutRatio(displayType), intType);
        }

        /// <summary>
        /// 公制转英制
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToApi(this double value)
        {
            return value.ToApi(Configuration.DisplayUnitType);
        }

        public static double ToApi(this double value, DisplayUnitType from)
        {
            switch (from)
            {
                case DisplayUnitType.DUT_FAHRENHEIT:
                    return (value + 459.67) / ImperialDutRatio(from);
                case DisplayUnitType.DUT_CELSIUS:
                    return value + 273.15;
                default:
                    return value /= ImperialDutRatio(from);
            }
        }

        private static double ImperialDutRatio(DisplayUnitType dut)
        {
            switch (dut)
            {
                case DisplayUnitType.DUT_ACRES: return 2.29568411386593E-05;
                case DisplayUnitType.DUT_AMPERES: return 1;
                case DisplayUnitType.DUT_ATMOSPHERES: return 3.23793722675857E-05;
                case DisplayUnitType.DUT_BARS: return 3.28083989501312E-05;
                case DisplayUnitType.DUT_BRITISH_THERMAL_UNITS: return 8.80550918411529E-05;
                case DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR: return 0.316998330628151;
                case DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_SECOND: return 8.80550918411529E-05;
                case DisplayUnitType.DUT_CALORIES: return 0.0221895098882201;
                case DisplayUnitType.DUT_CALORIES_PER_SECOND: return 0.0221895098882201;
                case DisplayUnitType.DUT_CANDELAS: return 1;
                case DisplayUnitType.DUT_CANDELAS_PER_SQUARE_METER: return 10.7639104167097;
                case DisplayUnitType.DUT_CANDLEPOWER: return 1;
                case DisplayUnitType.DUT_CELSIUS: return 1;
                case DisplayUnitType.DUT_CENTIMETERS: return 30.48;
                case DisplayUnitType.DUT_CENTIMETERS_PER_MINUTE: return 1828.8;
                case DisplayUnitType.DUT_CENTIPOISES: return 3280.83989501312;
                case DisplayUnitType.DUT_CUBIC_CENTIMETERS: return 28316.846592;
                case DisplayUnitType.DUT_CUBIC_FEET: return 1;
                case DisplayUnitType.DUT_CUBIC_FEET_PER_KIP: return 14593.9029372064;
                case DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE: return 60;
                case DisplayUnitType.DUT_CUBIC_INCHES: return 1728;
                case DisplayUnitType.DUT_CUBIC_METERS: return 0.028316846592;
                case DisplayUnitType.DUT_CUBIC_METERS_PER_HOUR: return 101.9406477312;
                case DisplayUnitType.DUT_CUBIC_METERS_PER_KILONEWTON: return 92.90304;
                case DisplayUnitType.DUT_CUBIC_METERS_PER_SECOND: return 0.028316846592;
                case DisplayUnitType.DUT_CUBIC_MILLIMETERS: return 28316846.592;
                case DisplayUnitType.DUT_CUBIC_YARDS: return 0.037037037037037;
                case DisplayUnitType.DUT_CYCLES_PER_SECOND: return 1;
                case DisplayUnitType.DUT_DECANEWTONS: return 0.03048;
                case DisplayUnitType.DUT_DECANEWTONS_PER_METER: return 0.1;
                case DisplayUnitType.DUT_DECANEWTONS_PER_SQUARE_METER: return 0.328083989501312;
                case DisplayUnitType.DUT_DECANEWTON_METERS: return 0.009290304;
                case DisplayUnitType.DUT_DECANEWTON_METERS_PER_METER: return 0.03048;
                case DisplayUnitType.DUT_DECIMAL_DEGREES: return 57.2957795130823;
                case DisplayUnitType.DUT_DECIMAL_FEET: return 1;
                case DisplayUnitType.DUT_DECIMAL_INCHES: return 12;
                case DisplayUnitType.DUT_DEGREES_AND_MINUTES: return 57.2957795130823;
                case DisplayUnitType.DUT_FAHRENHEIT: return 1.8;
                case DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES: return 1;
                case DisplayUnitType.DUT_FEET_OF_WATER: return 0.00109764531546318;
                case DisplayUnitType.DUT_FEET_OF_WATER_PER_100FT: return 0.109761336731934;
                case DisplayUnitType.DUT_FEET_PER_KIP: return 14593.9029372064;
                case DisplayUnitType.DUT_FEET_PER_MINUTE: return 60;
                case DisplayUnitType.DUT_FEET_PER_SECOND: return 1;
                case DisplayUnitType.DUT_FIXED: return 1;
                case DisplayUnitType.DUT_FOOTCANDLES: return 1.0000000387136;
                case DisplayUnitType.DUT_FOOTLAMBERTS: return 3.1415927449471;
                case DisplayUnitType.DUT_FRACTIONAL_INCHES: return 12;
                case DisplayUnitType.DUT_GALLONS_US: return 7.48051905367236;
                case DisplayUnitType.DUT_GALLONS_US_PER_HOUR: return 26929.8685932205;
                case DisplayUnitType.DUT_GALLONS_US_PER_MINUTE: return 448.831143220342;
                case DisplayUnitType.DUT_GENERAL: return 1;
                case DisplayUnitType.DUT_HECTARES: return 9.290304E-06;
                case DisplayUnitType.DUT_HERTZ: return 1;
                case DisplayUnitType.DUT_HORSEPOWER: return 0.00012458502883053;
                case DisplayUnitType.DUT_INCHES_OF_MERCURY: return 0.000968831370233344;
                case DisplayUnitType.DUT_INCHES_OF_WATER: return 0.0131845358262865;
                case DisplayUnitType.DUT_INCHES_OF_WATER_PER_100FT: return 1.31845358262865;
                case DisplayUnitType.DUT_INV_CELSIUS: return 1;
                case DisplayUnitType.DUT_INV_FAHRENHEIT: return 0.555555555555556;
                case DisplayUnitType.DUT_INV_KILONEWTONS: return 3280.83989501312;
                case DisplayUnitType.DUT_INV_KIPS: return 14593.9029372064;
                case DisplayUnitType.DUT_JOULES: return 0.09290304;
                case DisplayUnitType.DUT_KELVIN: return 1;
                case DisplayUnitType.DUT_KILOAMPERES: return 0.001;
                case DisplayUnitType.DUT_KILOCALORIES: return 2.21895098882201E-05;
                case DisplayUnitType.DUT_KILOCALORIES_PER_SECOND: return 2.21895098882201E-05;
                case DisplayUnitType.DUT_KILOGRAMS_FORCE: return 0.0310810655372411;
                case DisplayUnitType.DUT_KILOGRAMS_FORCE_PER_METER: return 0.101971999794098;
                case DisplayUnitType.DUT_KILOGRAMS_FORCE_PER_SQUARE_METER: return 0.334553805098747;
                case DisplayUnitType.DUT_KILOGRAMS_PER_CUBIC_METER: return 35.3146667214886;
                case DisplayUnitType.DUT_KILOGRAM_FORCE_METERS: return 0.00947350877575109;
                case DisplayUnitType.DUT_KILOGRAM_FORCE_METERS_PER_METER: return 0.0310810655372411;
                case DisplayUnitType.DUT_KILONEWTONS: return 0.0003048;
                case DisplayUnitType.DUT_KILONEWTONS_PER_CUBIC_METER: return 0.0107639104167097;
                case DisplayUnitType.DUT_KILONEWTONS_PER_METER: return 0.001;
                case DisplayUnitType.DUT_KILONEWTONS_PER_SQUARE_METER: return 0.00328083989501312;
                case DisplayUnitType.DUT_KILONEWTON_METERS: return 9.290304E-05;
                case DisplayUnitType.DUT_KILONEWTON_METERS_PER_DEGREE: return 9.290304E-05;
                case DisplayUnitType.DUT_KILONEWTON_METERS_PER_DEGREE_PER_METER: return 0.0003048;
                case DisplayUnitType.DUT_KILONEWTON_METERS_PER_METER: return 0.0003048;
                case DisplayUnitType.DUT_KILOPASCALS: return 0.00328083989501312;
                case DisplayUnitType.DUT_KILOVOLTS: return 9.290304E-05;
                case DisplayUnitType.DUT_KILOVOLT_AMPERES: return 9.290304E-05;
                case DisplayUnitType.DUT_KILOWATTS: return 9.290304E-05;
                case DisplayUnitType.DUT_KILOWATT_HOURS: return 2.58064E-08;
                case DisplayUnitType.DUT_KIPS: return 0.224808943099711;
                case DisplayUnitType.DUT_KIPS_PER_CUBIC_FOOT: return 6.85217658567918E-05;
                case DisplayUnitType.DUT_KIPS_PER_CUBIC_INCH: return 3.96537996856434E-08;
                case DisplayUnitType.DUT_KIPS_PER_FOOT: return 6.85217658567918E-05;
                case DisplayUnitType.DUT_KIPS_PER_INCH: return 5.71014715473265E-06;
                case DisplayUnitType.DUT_KIPS_PER_SQUARE_FOOT: return 6.85217658567918E-05;
                case DisplayUnitType.DUT_KIPS_PER_SQUARE_INCH: return 4.75845596227721E-07;
                case DisplayUnitType.DUT_KIP_FEET: return 6.85217658567918E-05;
                case DisplayUnitType.DUT_KIP_FEET_PER_DEGREE: return 6.85217658567918E-05;
                case DisplayUnitType.DUT_KIP_FEET_PER_DEGREE_PER_FOOT: return 2.08854342331501E-05;
                case DisplayUnitType.DUT_KIP_FEET_PER_FOOT: return 6.85217658567918E-05;
                case DisplayUnitType.DUT_LITERS: return 28.316846592;
                case DisplayUnitType.DUT_LITERS_PER_SECOND: return 28.316846592;
                case DisplayUnitType.DUT_LUMENS: return 1;
                case DisplayUnitType.DUT_LUX: return 10.7639104167097;
                case DisplayUnitType.DUT_MEGANEWTONS: return 3.048E-07;
                case DisplayUnitType.DUT_MEGANEWTONS_PER_METER: return 1E-06;
                case DisplayUnitType.DUT_MEGANEWTONS_PER_SQUARE_METER: return 3.28083989501312E-06;
                case DisplayUnitType.DUT_MEGANEWTON_METERS: return 9.290304E-08;
                case DisplayUnitType.DUT_MEGANEWTON_METERS_PER_METER: return 3.048E-07;
                case DisplayUnitType.DUT_MEGAPASCALS: return 3.28083989501312E-06;
                case DisplayUnitType.DUT_METERS: return 0.3048;
                case DisplayUnitType.DUT_METERS_CENTIMETERS: return 0.3048;
                case DisplayUnitType.DUT_METERS_PER_KILONEWTON: return 1000;
                case DisplayUnitType.DUT_METERS_PER_SECOND: return 0.3048;
                case DisplayUnitType.DUT_MILLIAMPERES: return 1000;
                case DisplayUnitType.DUT_MILLIMETERS: return 304.8;
                case DisplayUnitType.DUT_MILLIMETERS_OF_MERCURY: return 0.0246083170946002;
                case DisplayUnitType.DUT_MILLIVOLTS: return 92.90304;
                case DisplayUnitType.DUT_NEWTONS: return 0.3048;
                case DisplayUnitType.DUT_NEWTONS_PER_METER: return 1;
                case DisplayUnitType.DUT_NEWTONS_PER_SQUARE_METER: return 3.28083989501312;
                case DisplayUnitType.DUT_NEWTON_METERS: return 0.09290304;
                case DisplayUnitType.DUT_NEWTON_METERS_PER_METER: return 0.3048;
                case DisplayUnitType.DUT_PASCALS: return 3.28083989501312;
                case DisplayUnitType.DUT_PASCALS_PER_METER: return 10.7639104167097;
                case DisplayUnitType.DUT_PASCAL_SECONDS: return 3.28083989501312;
                case DisplayUnitType.DUT_PERCENTAGE: return 100;
                case DisplayUnitType.DUT_POUNDS_FORCE: return 224.80894309971;
                case DisplayUnitType.DUT_POUNDS_FORCE_PER_CUBIC_FOOT: return 0.0685217658567918;
                case DisplayUnitType.DUT_POUNDS_FORCE_PER_FOOT: return 0.0685217658567918;
                case DisplayUnitType.DUT_POUNDS_FORCE_PER_SQUARE_FOOT: return 0.0685217658567917;
                case DisplayUnitType.DUT_POUNDS_FORCE_PER_SQUARE_INCH: return 0.000475845616460903;
                case DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_FOOT: return 2.20462262184878;
                case DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_INCH: return 0.00127582327653286;
                case DisplayUnitType.DUT_POUNDS_MASS_PER_FOOT_HOUR: return 7936.64143865559;
                case DisplayUnitType.DUT_POUNDS_MASS_PER_FOOT_SECOND: return 2.20462262184878;
                case DisplayUnitType.DUT_POUND_FORCE_FEET: return 0.0685217658567918;
                case DisplayUnitType.DUT_POUND_FORCE_FEET_PER_FOOT: return 0.0685217658567918;
                case DisplayUnitType.DUT_RANKINE: return 1.8;
                case DisplayUnitType.DUT_SQUARE_CENTIMETERS: return 929.0304;
                case DisplayUnitType.DUT_SQUARE_FEET: return 1;
                case DisplayUnitType.DUT_SQUARE_FEET_PER_KIP: return 14593.9029372064;
                case DisplayUnitType.DUT_SQUARE_INCHES: return 144;
                case DisplayUnitType.DUT_SQUARE_METERS: return 0.09290304;
                case DisplayUnitType.DUT_SQUARE_METERS_PER_KILONEWTON: return 304.8;
                case DisplayUnitType.DUT_SQUARE_MILLIMETERS: return 92903.04;
                case DisplayUnitType.DUT_THERMS: return 8.80547457016663E-10;
                case DisplayUnitType.DUT_TONNES_FORCE: return 3.10810655372411E-05;
                case DisplayUnitType.DUT_TONNES_FORCE_PER_METER: return 0.000101971999794098;
                case DisplayUnitType.DUT_TONNES_FORCE_PER_SQUARE_METER: return 0.000334553805098747;
                case DisplayUnitType.DUT_TONNE_FORCE_METERS: return 9.47350877575109E-06;
                case DisplayUnitType.DUT_TONNE_FORCE_METERS_PER_METER: return 3.10810655372411E-05;
                case DisplayUnitType.DUT_VOLTS: return 0.09290304;
                case DisplayUnitType.DUT_VOLT_AMPERES: return 0.09290304;
                case DisplayUnitType.DUT_WATTS: return 0.09290304;
                case DisplayUnitType.DUT_WATTS_PER_SQUARE_FOOT: return 0.09290304;
                case DisplayUnitType.DUT_WATTS_PER_SQUARE_METER: return 1;
                /*------------------------new in Revit 2009-----------------------------------------*/
                case DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_CUBIC_FOOT: return 0.31699833062815;
                case DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT: return 0.316998330628151;
                case DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT_FAHRENHEIT: return 0.176110194261872;
                case DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_CUBIC_FOOT: return 60;
                case DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_SQUARE_FOOT: return 60;
                case DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_TON_OF_REFRIGERATION: return 2271305.33644539;
                case DisplayUnitType.DUT_CURRENCY: return 1;
                case DisplayUnitType.DUT_LITERS_PER_SECOND_CUBIC_METER: return 1000;
                case DisplayUnitType.DUT_LITERS_PER_SECOND_KILOWATTS: return 304800;
                case DisplayUnitType.DUT_LITERS_PER_SECOND_SQUARE_METER: return 304.8;
                case DisplayUnitType.DUT_LUMENS_PER_WATT: return 10.7639104167097;
                case DisplayUnitType.DUT_RATIO_10: return 10;
                case DisplayUnitType.DUT_RATIO_12: return 12;
                case DisplayUnitType.DUT_RISE_OVER_FOOT: return 1;
                case DisplayUnitType.DUT_RISE_OVER_INCHES: return 12;
                case DisplayUnitType.DUT_RISE_OVER_MMS: return 1000;
                case DisplayUnitType.DUT_SLOPE_DEGREES: return 57.2957795130824;
                case DisplayUnitType.DUT_SQUARE_FEET_PER_TON_OF_REFRIGERATION: return 37855.0889407566;
                case DisplayUnitType.DUT_SQUARE_METERS_PER_KILOWATTS: return 1000;
                case DisplayUnitType.DUT_TON_OF_REFRIGERATION: return 2.64165275523459E-05;
                case DisplayUnitType.DUT_WATTS_PER_CUBIC_FOOT: return 0.09290304;
                case DisplayUnitType.DUT_WATTS_PER_CUBIC_METER: return 3.28083989501312;
                case DisplayUnitType.DUT_WATTS_PER_SQUARE_METER_KELVIN: return 1;
                default: return 1;
            }
        }
    }
}
