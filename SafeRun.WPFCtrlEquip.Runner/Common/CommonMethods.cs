using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SafeRun.WPFCtrlEquip.WPFRunner.Common
{
    public class CommonMethods
    {
        #region 

        public enum ValidPositions
        {
            Pos0 = 0,
            Pos1 = 1,
            Pos2 = 2,
            Pos3 = 3,
            Pos4 = 4,
            Pos5 = 5,
            Pos6 = 6,
            Pos7 = 7,
            Pos8 = 8,
            Pos9 = 9,
            Pos10 = 10,
            Pos11 = 11,
            Pos12 = 12,
            Pos13 = 13,
            Pos14 = 14,
            Pos15 = 15

        }
        /// <summary>
        /// 将相关实数转化成int类型
        /// </summary>
        /// <param name="DBPieceType"></param>
        /// <param name="boolNode"></param>
        /// <returns></returns>
        public static int UshortChangeInt(ushort data, ValidPositions vPosition)
        {
            var position = (int)vPosition;
            //var result = (data & (1 << position));
            int result = (data >> position) & 1;

            return result;
        }
        #endregion
    }
}
