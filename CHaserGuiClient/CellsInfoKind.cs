using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oika.Apps.CHaserGuiClient
{
    public enum CellsInfoKind
    {
        Unknown,
        Around,
        LookLeft,
        LookRight,
        LookUp,
        LookDown,
        SearchLeft,
        SearchRight,
        SearchUp,
        SearchDown,
    }

    public static class CellsInfoKindExt
    {
        public static CellsInfoKind FromMethod(MethodKind method, DirectionKind direction)
        {
            if (method == MethodKind.Walk || method == MethodKind.Put)
            {
                return CellsInfoKind.Around;
            }
            if (method == MethodKind.Look)
            {
                return direction == DirectionKind.Left ? CellsInfoKind.LookLeft
                    : direction == DirectionKind.Up ? CellsInfoKind.LookUp
                    : direction == DirectionKind.Right ? CellsInfoKind.LookRight
                    : direction == DirectionKind.Down ? CellsInfoKind.LookDown
                    : CellsInfoKind.Unknown;
            }
            if (method == MethodKind.Search)
            {
                return direction == DirectionKind.Left ? CellsInfoKind.SearchLeft
                    : direction == DirectionKind.Up ? CellsInfoKind.SearchUp
                    : direction == DirectionKind.Right ? CellsInfoKind.SearchRight
                    : direction == DirectionKind.Down ? CellsInfoKind.SearchDown
                    : CellsInfoKind.Unknown;
            }
            return CellsInfoKind.Unknown;
        }
    }
}
