using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* Description: AccLooker（在Windows系统下侦测UI控件属性，用于UI Automation辅助测试调试）
 *          By: HalfasonYc
 *        Time: 2020-09-22
 *       Email: Endless_yangc@foxmail.com
 *     Version: 1.0
 */
namespace AccessibleProject
{
    public partial class AccLooker : Form
    {
        bool _isRunning;

        AccessibleHelper _accessibleHelper;

        public AccLooker()
        {
            InitializeComponent();
            this.InitConfig();
        }

        private void InitConfig()
        {
            this._isRunning = false;
        }

        private void button_Scan_MouseDown(object sender, MouseEventArgs e)
        {
            this._isRunning = true;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        private void button_Scan_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._isRunning)
            {
                this._isRunning = false;
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        private void button_Scan_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._isRunning)
            {
                this._accessibleHelper = AccessibleHelper.CreateByPoint(Cursor.Position);
                var inforr = this._accessibleHelper?.GetInformation();
                if (inforr.HasValue)
                {
                    var infor = inforr.Value;
                    this.InforPropertyGrid.SelectedObject = infor;
                    AccessibleHelper.DrawBorderByRectangle(infor.AccLocation);
                }
            }
        }
    }
}
