using MicroSvr.RPC.Client;
using SafeRun.CtrlEquip.Spi;
using SafeRun.CtrlEquipTasks.Spi;
using SafeRun.EquipData.Spi;
using SafeRun.WMS.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SafeRun.WPFCtrlEquip.WPFRunner.Common
{
    public class RouteRpcApi
    {
        public RouteRpcApi() { }
        //public RouteRpcApi()
        //{
        //}
        //private static RouteRpcApi _instance;
        //public static RouteRpcApi Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = Singleton.Create<RouteRpcApi>();
        //        }
        //        return _instance;
        //    }
        //}
        private static T RPCConnect<T>()
        {
            return RpcHelper.Create<T>();//rpc调用其他服务API接口
        }

        public static TaskService WMSTaskApi()
        {
            return RPCConnect<TaskService>();//rpc调用其他服务API接口
        }

        public static TaskCompleteService CtrlEquipTasksApi()
        {
            return RPCConnect<TaskCompleteService>();//rpc调用其他服务API接口
        }

        public static EquipDataService EquipDataApi()
        {
            return RPCConnect<EquipDataService>();//rpc调用其他服务API接口
        }

        public static EquipContextHelper CtrlEquipContextApi()
        {
            //return EquipContextHelper.Instance;//rpc调用其他服务API接口
            return RPCConnect<EquipContextHelper>();//rpc调用其他服务API接口
        }

        #region EquipService
        private static EquipService getCtrlService()
        {
            var center = RpcHelper.Create<EquipService>();
            return center;
        }

        public static int SetKeyValues(KeyValue[] keys)
        {
            return getCtrlService().SetKeyValues(keys);
        }

        public static KeyValue[] GetRemoteNodes(string[] keys)
        {
            return getCtrlService().GetKeyValues(keys);
        }
        #endregion

        public static CrnService WMSCrnApi()
        {
            return RPCConnect<CrnService>();//rpc调用其他服务API接口
        }

        public static ErrorAlarmService WMSErrorAlarmApi()
        {
            return RPCConnect<ErrorAlarmService>();//rpc调用其他服务API接口
        }

        public static StockLocStatusService WMSStockLocStatusApi()
        {
            return RPCConnect<StockLocStatusService>();//rpc调用其他服务API接口
        }
    }
}
