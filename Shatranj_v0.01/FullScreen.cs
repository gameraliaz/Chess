using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shatranj_v0._01
{
    class FullScreen
    {
        public void EnterFullScreenMode(Form targetForm)
        {
            targetForm.WindowState = FormWindowState.Normal;
            targetForm.FormBorderStyle = FormBorderStyle.None;
            targetForm.WindowState = FormWindowState.Maximized;
        }
        public void LeaveFullScreenMode(Form targetForm)
        {
            targetForm.FormBorderStyle = FormBorderStyle.Sizable;
            targetForm.WindowState = FormWindowState.Normal;
        }
    }
}
