using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoSoReporting.Module
{
    public partial class DoSoSheetFrom : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public DoSoSheetFrom(bool addMailMerge)
        {
            InitializeComponent(addMailMerge);
        }
    }
}
