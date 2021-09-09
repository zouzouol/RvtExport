using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLib
{
    #region VertexLookupXyz
    /// <summary>
    /// A vertex lookup class to eliminate 
    /// duplicate vertex definitions.
    /// </summary>
    public class VertexLookupXyz : Dictionary<XYZ, int>
    {
        #region XyzEqualityComparer
        /// <summary>
        /// Define equality for Revit XYZ points.
        /// Very rough tolerance, as used by Revit itself.
        /// </summary>
        class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            const double _sixteenthInchInFeet
              = 1.0 / (16.0 * 12.0);

            public bool Equals(XYZ p, XYZ q)
            {
                return p.IsAlmostEqualTo(q,
                  _sixteenthInchInFeet);
            }

            public int GetHashCode(XYZ p)
            {
                return p.PointString().GetHashCode();
            }
        }
        #endregion // XyzEqualityComparer

        public VertexLookupXyz()
          : base(new XyzEqualityComparer())
        {
        }

        /// <summary>
        /// Return the index of the given vertex,
        /// adding a new entry if required.
        /// </summary>
        public int AddVertex(XYZ p)
        {
            return ContainsKey(p)
              ? this[p]
              : this[p] = Count;
        }
    }
    #endregion // VertexLookupXyz

    #region VertexLookupInt
    /// <summary>
    /// An integer-based 3D point class.
    /// </summary>
    public class PointInt : IComparable<PointInt>
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        //public PointInt( int x, int y, int z )
        //{
        //  X = x;
        //  Y = y;
        //  Z = z;
        //}

        /// <summary>
        /// Consider a Revit length zero 
        /// if is smaller than this.
        /// </summary>
        const double _eps = 1.0e-9;

        /// <summary>
        /// Conversion factor from feet to millimetres.
        /// </summary>
        const double _feet_to_mm = 25.4 * 12;

        /// <summary>
        /// Conversion a given length value 
        /// from feet to millimetre.
        /// </summary>
        static long ConvertFeetToMillimetres(double d)
        {
            if (0 < d)
            {
                return _eps > d
                  ? 0
                  : (long)(_feet_to_mm * d + 0.5);

            }
            else
            {
                return _eps > -d
                  ? 0
                  : (long)(_feet_to_mm * d - 0.5);

            }
        }

        public PointInt(XYZ p, bool switch_coordinates)
        {
            X = ConvertFeetToMillimetres(p.X);
            Y = ConvertFeetToMillimetres(p.Y);
            Z = ConvertFeetToMillimetres(p.Z);

            if (switch_coordinates)
            {
                X = -X;
                long tmp = Y;
                Y = Z;
                Z = tmp;
            }
        }

        public int CompareTo(PointInt a)
        {
            long d = X - a.X;

            if (0 == d)
            {
                d = Y - a.Y;

                if (0 == d)
                {
                    d = Z - a.Z;
                }
            }
            return (0 == d) ? 0 : ((0 < d) ? 1 : -1);
        }
    }

    /// <summary>
    /// A vertex lookup class to eliminate 
    /// duplicate vertex definitions.
    /// </summary>
    public class VertexLookupInt : Dictionary<PointInt, int>
    {
        #region PointIntEqualityComparer
        /// <summary>
        /// Define equality for integer-based PointInt.
        /// </summary>
        class PointIntEqualityComparer : IEqualityComparer<PointInt>
        {
            public bool Equals(PointInt p, PointInt q)
            {
                return 0 == p.CompareTo(q);
            }

            public int GetHashCode(PointInt p)
            {
                return (p.X.ToString()
                  + "," + p.Y.ToString()
                  + "," + p.Z.ToString())
                  .GetHashCode();
            }
        }
        #endregion // PointIntEqualityComparer

        public VertexLookupInt()
          : base(new PointIntEqualityComparer())
        {
        }

        /// <summary>
        /// Return the index of the given vertex,
        /// adding a new entry if required.
        /// </summary>
        public int AddVertex(PointInt p)
        {
            return ContainsKey(p)
              ? this[p]
              : this[p] = Count;
        }
    }
    #endregion // VertexLookupInt
}
