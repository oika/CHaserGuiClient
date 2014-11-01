using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Oika.Apps.CHaserGuiClient.ViewModels
{
    public class AroundFloorPanelContext
    {
        public CellsInfoKind Location { get; private set; }

        public ReadOnlyCollection<CellKind> Cells { get; private set; }

        public AroundFloorPanelContext(CellsInfoKind location, ReadOnlyCollection<CellKind> cells)
        {
            this.Location = location;
            this.Cells = cells;
        }
    }
}
