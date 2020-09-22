using Accessibility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/* Description: AccessibleHelper，通过P/Invoke与IAccessible结合，完成UI解析动作
 *          By: HalfasonYc
 *        Time: 2020-09-22
 *       Email: Endless_yangc@foxmail.com
 *     Version: 1.0
 */

namespace AccessibleProject
{
    public class AccessibleHelper
    {
        IAccessible _self = null;
        IntPtr _hwnd = IntPtr.Zero;
        int _id = 0;

        public static AccessibleHelper CreateByHwnd(IntPtr hwnd)
        {
            Guid guid = NativeMethods.IAccessibleGuid;
            IAccessible accessible = null;
            if (NativeMethods.AccessibleObjectFromWindow(hwnd, NativeMethods.ObjId.Window, ref guid, ref accessible) == 0)
            {
                AccessibleHelper accessibleHelper = new AccessibleHelper();
                accessibleHelper._self = accessible;
                accessibleHelper._hwnd = hwnd;
                return accessibleHelper;
            }
            return null;
        }

        public static AccessibleHelper CreateByPoint(Point point)
        {
            if (NativeMethods.AccessibleObjectFromPoint(point, out IAccessible accessible, out object childId) == IntPtr.Zero)
            {
                AccessibleHelper accessibleHelper = CreateByObject(accessible);
                accessibleHelper._id = Convert.ToInt32(childId);
                return accessibleHelper;
            }
            return null;
        }

        public static AccessibleHelper CreateByObject(IAccessible accessible)
        {
            AccessibleHelper accessibleHelper = new AccessibleHelper();
            accessibleHelper._self = accessible;
            NativeMethods.WindowFromAccessibleObject(accessible, ref accessibleHelper._hwnd);
            return accessibleHelper;
        }

        public AccessibleInformation GetInformation()
        {
            AccessibleInformation infor = new AccessibleInformation();
            int x, y, width, height;
            int index = 0;
            //有些接口方法并未实现，有些属性无效，或者其他原因， 总之IAccessible的接口方法不安全，需要自行处理异常
  getValue: try
            {
                switch (index)
                {
                    case 0:
                        infor.AccDescription = _self.accDescription[_id];
                        break;
                    case 1:
                        infor.AccHelp = _self.accHelp[_id];
                        break;
                    case 2:
                        infor.AccDefaultAction = _self.accDefaultAction[_id];
                        break;
                    case 3:
                        infor.AccName = _self.accName[_id];
                        break;
                    case 4:
                        infor.AccRole = _self.accRole[_id];
                        break;
                    case 5:
                        infor.AccState = _self.accState[_id];
                        break;
                    case 6:
                        infor.AccValue = _self.accValue[_id];
                        break;
                    case 7:
                        infor.AccKeyboardShortcut = _self.accKeyboardShortcut[_id];
                        break;
                    case 8:
                        _self.accLocation(out x, out y, out width, out height, _id);
                        infor.AccLocation = new Rectangle(x, y, width, height);
                        break;
                    case 9:
                        infor.AccChildId = _id;
                        break;
                    case 10:
                        if (_id == 0)
                        {
                            infor.AccChildCount = _self.accChildCount;
                        }
                        else
                        {
                            infor.AccChildCount = (_self.accChild[_id] as IAccessible).accChildCount;
                        }
                        break;
                }
            }
            catch
            {
                //catch (System.NotImplementedException)
                //catch (System.ArgumentException)
                //catch (System.Runtime.InteropServices.COMException)
            }
            if (++index != 11)
            {
                goto getValue;
            }
            infor.HandleWindow = _hwnd;
            StringBuilder stringBuilder = new StringBuilder(200);
            if (NativeMethods.GetClassName(_hwnd, stringBuilder, 200) != 0)
            {
                infor.ClassNameWindow = stringBuilder.ToString();
            }

            int buffer = NativeMethods.GetWindowTextLength(_hwnd) + 1;
            stringBuilder = new StringBuilder(buffer);
            if (NativeMethods.GetWindowText(_hwnd, stringBuilder, buffer) != 0)
            {
                infor.TitleWindow = stringBuilder.ToString();
            }
            return infor;
        }

        public IAccessible Accessible
        {
            get
            {
                return _self;
            }
        }

        public IntPtr Hwnd
        {
            get
            {
                return _hwnd;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// 在桌面实时绘画矩形，并自动清除
        /// </summary>
        /// <param name="accLocation"></param>
        public static void DrawBorderByRectangle(Rectangle accLocation)
        {
            //获取桌面设备场景
            IntPtr deskHwnd = NativeMethods.GetDesktopWindow();
            IntPtr deskDC = NativeMethods.GetWindowDC(deskHwnd);
            var oldR2 = NativeMethods.SetROP2(deskDC, NativeMethods.BinaryRasterOperations.R2_NOTXORPEN);

            IntPtr newPen = NativeMethods.CreatePen(NativeMethods.PenStyle.PS_SOLID, 2, (uint)ColorTranslator.ToWin32(Color.Red));
            IntPtr oldPen = NativeMethods.SelectObject(deskDC, newPen);
            //使用新笔画图
            NativeMethods.Rectangle(deskDC, accLocation.Left, accLocation.Top, accLocation.Right, accLocation.Bottom);
            Task.Delay(100).Wait();
            NativeMethods.Rectangle(deskDC, accLocation.Left, accLocation.Top, accLocation.Right, accLocation.Bottom);
            NativeMethods.SetROP2(deskDC, oldR2);
            //还原设备状态
            NativeMethods.SelectObject(deskDC, oldPen);
            NativeMethods.DeleteObject(newPen);
            NativeMethods.ReleaseDC(deskHwnd, deskDC);
        }


        private static class NativeMethods
        {
            //winable.h constants
            public enum ObjId : uint
            {
                Window = 0x00000000,
                SysMenu = 0xFFFFFFFF,
                TitleBar = 0xFFFFFFFE,
                Menu = 0xFFFFFFFD,
                Client = 0xFFFFFFFC,
                Vscroll = 0xFFFFFFFB,
                Hscroll = 0xFFFFFFFA,
                Sizegrip = 0xFFFFFFF9,
                Caret = 0xFFFFFFF8,
                Cursor = 0xFFFFFFF7,
                Alert = 0xFFFFFFF6,
                Sound = 0xFFFFFFF5,
            }
            public enum BinaryRasterOperations
            {

                R2_BLACK = 1,
                R2_NOTMERGEPEN = 2,
                R2_MASKNOTPEN = 3,
                R2_NOTCOPYPEN = 4,
                R2_MASKPENNOT = 5,
                R2_NOT = 6,
                R2_XORPEN = 7,
                R2_NOTMASKPEN = 8,
                R2_MASKPEN = 9,
                R2_NOTXORPEN = 10,
                R2_NOP = 11,
                R2_MERGENOTPEN = 12,
                R2_COPYPEN = 13,
                R2_MERGEPENNOT = 14,
                R2_MERGEPEN = 15,
                R2_WHITE = 16
            }

            public enum PenStyle : int
            {
                PS_SOLID = 0, //The pen is solid.
                PS_DASH = 1, //The pen is dashed.
                PS_DOT = 2, //The pen is dotted.
                PS_DASHDOT = 3, //The pen has alternating dashes and dots.
                PS_DASHDOTDOT = 4, //The pen has alternating dashes and double dots.
                PS_NULL = 5, //The pen is invisible.
                PS_INSIDEFRAME = 6,// Normally when the edge is drawn, it’s centred on the outer edge meaning that half the width of the pen is drawn
                                   // outside the shape’s edge, half is inside the shape’s edge. When PS_INSIDEFRAME is specified the edge is drawn
                                   //completely inside the outer edge of the shape.
                PS_USERSTYLE = 7,
                PS_ALTERNATE = 8,
                PS_STYLE_MASK = 0x0000000F,

                PS_ENDCAP_ROUND = 0x00000000,
                PS_ENDCAP_SQUARE = 0x00000100,
                PS_ENDCAP_FLAT = 0x00000200,
                PS_ENDCAP_MASK = 0x00000F00,

                PS_JOIN_ROUND = 0x00000000,
                PS_JOIN_BEVEL = 0x00001000,
                PS_JOIN_MITER = 0x00002000,
                PS_JOIN_MASK = 0x0000F000,

                PS_COSMETIC = 0x00000000,
                PS_GEOMETRIC = 0x00010000,
                PS_TYPE_MASK = 0x000F0000
            }

            //Guid obtained from OleAcc.idl from Platform SDK
            readonly static Guid _iAccessibleGuid = new Guid("618736e0-3c3d-11cf-810c-00aa00389b71");
            public static Guid IAccessibleGuid
            {
                get
                {
                    return _iAccessibleGuid;
                }
            }

            /// Return Type: HWND->HWND__*
            ///hWndParent: HWND->HWND__*
            ///hWndChildAfter: HWND->HWND__*
            ///lpszClass: LPCSTR->CHAR*
            ///lpszWindow: LPCSTR->CHAR*
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "FindWindowEx")]
            public static extern System.IntPtr FindWindowEx([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWndParent, 
                [System.Runtime.InteropServices.InAttribute()] System.IntPtr hWndChildAfter, 
                [System.Runtime.InteropServices.InAttribute()] string lpszClass, 
                [System.Runtime.InteropServices.InAttribute()] string lpszWindow);

            /// <summary>
            /// AccessibleObjectFromWindow
            /// </summary>
            /// <param name="hwnd">Handle</param>
            /// <param name="id">ObjId</param>
            /// <param name="iid">IAccessibleGuid</param>
            /// <param name="ppvObject"></param>
            /// <returns></returns>
            [DllImport("oleacc.dll")]
            public static extern int AccessibleObjectFromWindow(IntPtr hwnd, ObjId id, ref Guid iid, ref IAccessible ppvObject);

            /// <summary>
            /// The AccessibleChildren function retrieves the child ID or IDispatch interface of each child within an accessible container object.
            /// </summary>
            /// <param name="paccContainer">[in] Pointer to the container object's IAccessible interface.</param>
            /// <param name="iChildStart">[in] Specifies the zero-based index of the first child retrieved. This parameter is an index, not a child ID. Typically, this parameter is set to zero (0).</param>
            /// <param name="cChildren">[in] Specifies the amount of children to retrieve.An application calls IAccessible.accChildCount to retrieve the current number of children.</param>
            /// <param name="rgvarChildren">[out] Pointer to an array of VARIANT structures that receives information about the container's children. If the vt member of an array element is VT_I4, then the lVal member for that element is the child ID. If the vt member of an array element is VT_DISPATCH, then the pdispVal member for that element is the address of the child object's IDispatch interface.</param>
            /// <param name="pcObtained">[out] Address of a variable that receives the number of elements in the rgvarChildren array filled in by the function. This value is the same as the cChildren parameter, unless you ask for more children than the number that exist. Then, this value will be less than cChildren.</param>
            /// <returns></returns>
            [DllImport("oleacc.dll")]
            public static extern uint AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] object[] rgvarChildren, out int pcObtained);

            [DllImport("oleacc.dll")]
            public static extern IntPtr AccessibleObjectFromPoint(Point pt, [Out, MarshalAs(UnmanagedType.Interface)] out IAccessible accObj, [Out] out object childID);


            [DllImport("oleacc.dll")]
            public static extern uint WindowFromAccessibleObject(IAccessible pacc, ref IntPtr phwnd);

            /// Return Type: BOOL->int
            ///hWnd: HWND->HWND__*
            ///lpRect: LPRECT->tagRECT*
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetWindowRect")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool GetWindowRect([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, [System.Runtime.InteropServices.OutAttribute()] out Rectangle lpRect);

            /// Return Type: BOOL->int
            ///hWnd: HWND->HWND__*
            ///lpRect: RECT*
            ///bErase: BOOL->int
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "InvalidateRect")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool InvalidateRect([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, 
                [System.Runtime.InteropServices.InAttribute()] System.IntPtr lpRect, 
                [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool bErase);

            /// Return Type: BOOL->int
            ///hWnd: HWND->HWND__*
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "UpdateWindow")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UpdateWindow([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd);

            /// Return Type: HWND->HWND__*
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetDesktopWindow")]
            public static extern System.IntPtr GetDesktopWindow();

            /// Return Type: HDC->HDC__*
            ///hWnd: HWND->HWND__*
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetWindowDC")]
            public static extern System.IntPtr GetWindowDC([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd);

            /// Return Type: int
            ///hdc: HDC->HDC__*
            ///rop2: int
            [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll", EntryPoint = "SetROP2")]
            public static extern BinaryRasterOperations SetROP2([System.Runtime.InteropServices.InAttribute()] System.IntPtr hdc, BinaryRasterOperations rop2);
            
            /// Return Type: HPEN->HPEN__*
            ///iStyle: int
            ///cWidth: int
            ///color: COLORREF->DWORD->unsigned int
            [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll", EntryPoint = "CreatePen")]
            public static extern System.IntPtr CreatePen(PenStyle iStyle, int cWidth, uint color);

            /// Return Type: HGDIOBJ->void*
            ///hdc: HDC->HDC__*
            ///h: HGDIOBJ->void*
            [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll", EntryPoint = "SelectObject")]
            public static extern System.IntPtr SelectObject([System.Runtime.InteropServices.InAttribute()] System.IntPtr hdc, [System.Runtime.InteropServices.InAttribute()] System.IntPtr h);

            /// Return Type: BOOL->int
            ///hdc: HDC->HDC__*
            ///left: int
            ///top: int
            ///right: int
            ///bottom: int
            [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll", EntryPoint = "Rectangle")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool Rectangle([System.Runtime.InteropServices.InAttribute()] System.IntPtr hdc, int left, int top, int right, int bottom);

            /// Return Type: BOOL->int
            ///ho: HGDIOBJ->void*
            [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool DeleteObject([System.Runtime.InteropServices.InAttribute()] System.IntPtr ho);

            /// Return Type: int
            ///hWnd: HWND->HWND__*
            ///hDC: HDC->HDC__*
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "ReleaseDC")]
            public static extern int ReleaseDC([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, [System.Runtime.InteropServices.InAttribute()] System.IntPtr hDC);

            /// Return Type: int
            ///hWnd: HWND->HWND__*
            ///lpClassName: LPWSTR->WCHAR*
            ///nMaxCount: int
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetClassName")]
            public static extern int GetClassName([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, 
                [System.Runtime.InteropServices.OutAttribute()] System.Text.StringBuilder lpClassName, 
                int nMaxCount);

            /// Return Type: int
            ///hWnd: HWND->HWND__*
            ///lpString: LPSTR->CHAR*
            ///nMaxCount: int
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetWindowText")]
            public static extern int GetWindowText([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, 
                [System.Runtime.InteropServices.OutAttribute()] System.Text.StringBuilder lpString, 
                int nMaxCount);

            /// Return Type: int
            ///hWnd: HWND->HWND__*
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetWindowTextLength")]
            public static extern int GetWindowTextLength([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd);
        }
    }
}
