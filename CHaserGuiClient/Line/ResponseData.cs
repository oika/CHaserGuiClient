using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Oika.Apps.CHaserGuiClient.Line
{
    public class ResponseData
    {
        public bool IsGameSet { get; private set; }

        public ReadOnlyCollection<CellKind> Cells { get; private set; }

        public const int TotalLength = 10;

        public ResponseData(string chars)
        {
            if (chars.Length != TotalLength) throw new ArgumentException("受信した " + chars + " は不正な電文です");

            var stateVal = chars[0];
            if (stateVal == '0')
            {
                IsGameSet = true;
            }
            else if (stateVal != '1')
            {
                throw new ArgumentException("制御情報" + stateVal + "は不正な値です");
            }


            var list = new List<CellKind>();
            foreach (var c in chars.Skip(1))
            {
                int val;
                if (!int.TryParse(c.ToString(), out val) || !Enum.IsDefined(typeof(CellKind), val))
                {
                    throw new ArgumentException("周辺情報" + c + "は不正な値です");
                }

                list.Add((CellKind)val);
            }
            this.Cells = new ReadOnlyCollection<CellKind>(list);
        }
    }
}
