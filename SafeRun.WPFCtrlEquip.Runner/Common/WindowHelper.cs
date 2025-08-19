




using MicroSvr.Comm;
using MicroSvr.RPC.Client;
using MicroSvr.XRun;
using SafeRun.CtrlBase;
using SafeRun.CtrlBiz.SpiService;
using SafeRun.CtrlEquipTasks.Spi;
using SafeRun.EquipData.Server;
using SafeRun.EquipData.Spi;
using SafeRun.WMS.Spi;
using SafeRun.WPFCtrlEquip.WPFRunner.Common;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Input;
using static DevExpress.Data.Helpers.FindSearchRichParser;

namespace SafeRun.WPFCtrlEquip.WPFRunner
{
    public static class WindowHelper
    {

        private static List<ObservableEvent> AllNodeEvents = new List<ObservableEvent>();

        private static CancellationTokenSource _cancellationTokenSource;

        private static ConcurrentDictionary<string, BizNode> keyBizNodes = new ConcurrentDictionary<string, BizNode>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 绑定服务
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static void BindingToServer(this DependencyObject panel)
        {
            AllNodeEvents.Clear();//开启时初始化数据
            _cancellationTokenSource?.Cancel();

            //1.获取全部自定义控件
            var items = panel.FindTagControls();

            //2.自定义控件绑定
            if (items != null)
            {
                foreach (var element in items)
                {
                    //2_1.控件绑定数据节点
                    BindingControl(element, AllNodeEvents);
                }
            }

            //3.读取数据库数据+赋值给全节点数据字段AllNodeDatas
            if (AllNodeEvents.Count <= 0) return;
            UpdNodeDatas(AllNodeEvents);




            //5.获取数采上业务节点
            var demandNodes = AllNodeEvents.Select(s => s.NodeName).ToList();
            GetDataMiningBizNodes(XApp.Current.AppId, demandNodes);

            //6.异步任务进行全节点轮循读取PLC数据
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            TaskAsyncAllNodeEvents(token);

            //7.异步进行
        }

        #region 异步任务进行

        /// <summary>
        /// 异步任务进行全节点轮循读取PLC数据。
        /// </summary>
        /// <param name="token"></param>
        private static void TaskAsyncAllNodeEvents(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        // 检查取消令牌
                        //token.ThrowIfCancellationRequested();
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (Application.Current == null)
                        {
                            continue;
                        }
                        //若需通用方案，可考虑SynchronizationContext进行实现,Dispatcher为针对xaml的UI线程
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UpdNodeDatas(AllNodeEvents);
                        });

                        //添加若AllNodeEvents 为0 则跳出
                        if (AllNodeEvents.Count <= 0) break;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        await Task.Delay(500);
                    }
                }

            });
        }

        #endregion

        private static void GetDataMiningBizNodes(string appId, List<string> demandNodes)
        {
            //1.获取配置的业务节点RSF_APP、MDC_BIZ、MDC_BIZ_NODE
            var context = GetBizContext(appId);
            if (context == null) return;

            //2.拼接需要的节点
            var keyVals = new Dictionary<string, BizNode>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var nodeName in demandNodes)
            {
                var nodeItem = context.MdcBiz.Nodes.FirstOrDefault(m => m.NodeName == nodeName);
                if (nodeItem == null) continue;
                keyVals.TryAdd(nodeItem.BlockNodeId, nodeItem);
            }
            keyBizNodes.Clear();
            keyBizNodes = new ConcurrentDictionary<string, BizNode>(keyVals);
        }

        /// <summary>
        /// 自定义控件绑定
        /// </summary>
        /// <param name="taglink"></param>
        /// <param name="allNodeDatas"></param>
        private static void BindingControl(ITagLink taglink, List<ObservableEvent> allNodeEvents)
        {
            //1.判断是否为可视化控件
            var ctrl = taglink as UIElement;//先判断是否为可视化控件，UIElement为WPF最底层可视化元素基类

            if (ctrl == null) return;
            var complex = taglink as ITagReader;//判断是否为自定义的控件，所有定义的的都继承HMIControlBase<-ITagReader，所有自定义控件都有读取属性
            var tagReadText = complex.TagReadText;
            if (string.IsNullOrEmpty(tagReadText)) return;

            //2.初始化控件配置读取属性
            foreach (var item in tagReadText.Split(';'))
            {
                var items = item.Split(':');
                if (items.Length < 2) continue;

                complex.SetTagReader(items[0], items[1]);
            }
            //2_2.绑定操作事件
            complex.UpdAction = SetUpdNodes;
            complex.DeleteAction = SetDeleteAction;
            complex.CompleteAction = SetCompleteAction;
            complex.QueryAction = QueryAction;

            //2_3.校验控件是否在WMS中配置使用 hyx 2025-01-22
            if (!string.IsNullOrEmpty(complex.SystemNo))
            {
                var crnControl = complex as Stacker;
                if(crnControl != null)
                {
                    if(complex.SystemNo == "CRN05")
                    {

                    }
                    var crnParam = new CommonQuery() { CrnNo = complex.SystemNo };
                    var crnList = RouteRpcApi.WMSCrnApi().GetWCSCrnList(crnParam);
                    var crnItem = crnList.FirstOrDefault();
                    if (crnItem != null)
                    {
                        crnControl.UseTxt = crnItem.UseFlag == 1 ? "启用" : "未启用";
                    }
                    crnControl.AsyncRefreshStock(QueryCrnStockAction);
                }
                
            }


            //3.绑定数据变更事件,处理组件显示值
            var showDatas = complex.ShowTags;
            if (showDatas == null || showDatas.Count <= 0) return;
            foreach (var item in showDatas)
            {
                var contrName = item.ContrName;
                var nodesNames = item.NodeNames;

                //3.1.对值变更事件进行绑定
                foreach (var nodeName in nodesNames)
                {
                    if (nodeName.IndexOf("CV724") >= 0)
                    {

                    }

                    var valueChangeEvent = allNodeEvents.FirstOrDefault(m => m.NodeName == nodeName);
                    if (valueChangeEvent == null)
                    {
                        valueChangeEvent = new ObservableEvent() { NodeName = nodeName, NodeVal = "" };
                        allNodeEvents.Add(valueChangeEvent);
                    }

                    valueChangeEvent.ValueChanged += (sender, args) =>
                    {
                        var newValue = args.NewValue;
                        if (nodesNames.Count > 1)//多个值获取方式
                        {
                            newValue = string.Join("+", nodesNames.Select(m => allNodeEvents.Find(n => n.NodeName == m).NodeVal));
                        }
                        complex.SetShowControls(contrName, newValue);
                    };

                    //用于处理页面重复打开时，因数值无变化，导致数据不显示问题
                    if (!string.IsNullOrEmpty(valueChangeEvent.NodeVal))
                    {
                        complex.SetShowControls(contrName, valueChangeEvent.NodeVal);
                    }

                }
            }

        }


        public static IEnumerable<ITagLink> FindTagControls(this DependencyObject parent)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    var t = child as ITagLink;

                    if (t != null)
                        yield return t;
                    else
                    {
                        var children = FindTagControls(child);
                        foreach (var item in children)
                            yield return item;
                    }
                }
            }
        }

        #region 数据处理

        /// <summary>
        /// 初始化所有数据节点
        /// </summary>
        private static List<ObservableEvent> InitNodeDatas()
        {
            //var dbNodeList = DBDataClass.GetEquipBlocks();
            var allNodeEvents = new List<ObservableEvent>();
            //foreach (var node in dbNodeList)
            //{
            //    var nodeName = node.NodeName;
            //    var nodeVal = node.DefaultValue;

            //    if(allNodeEvents.Exists(m=>m.NodeName == nodeName)) continue;
            //    //FieldVal = "" 为了后续从数据库中读取到数据，可以触发值变更事件
            //    var valueChangeEvent = new ObservableEvent() { NodeName = nodeName, NodeVal = "" };

            //    allNodeEvents.Add(valueChangeEvent);
            //}

            return allNodeEvents;
        }

        /// <summary>
        /// 初始化所有数据节点
        /// 目前是固定取MDC_BLOCK_NODE
        /// </summary>
        private static List<ObservableEvent> UpdNodeDatas(List<ObservableEvent> allNodeEvents)
        {

            var demandNodes = allNodeEvents.Select(s => s.NodeName).ToList();

            //更新节点值
            var values = GetScadaNodeValues(XApp.Current.AppId, demandNodes);
            if (values == null || values.Count <= 0)
            {
                return null;
            }
            foreach (var item in values)
            {
                var updItem = allNodeEvents.FirstOrDefault(r => r.NodeName.Equals(item.key, StringComparison.InvariantCultureIgnoreCase));
                if (updItem == null) continue;
                updItem.NodeVal = item.val;
            }

            return allNodeEvents;
        }

        #endregion



        #region 读取业务节点

        private static EquipDataService getCtrlService()
        {
            //var center = RpcHelper.Create<EquipDataService>();
            //return center;
            return RouteRpcApi.EquipDataApi();
        }

        /// <summary>
        /// RSF_APP中的APPID,
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="demandNodes"></param>
        /// <returns></returns>
        private static List<(string key, string val)> GetScadaNodeValues(string appId, List<string> demandNodes)
        {
            /* 为减少请求次数，只请求一次-2025-03-11 hyx
            //1.获取配置的业务节点RSF_APP、MDC_BIZ、MDC_BIZ_NODE
            var context = GetBizContext(appId);
            if (context == null) return null;

            //2.拼接需要的节点
            var keyVals = new Dictionary<string, BizNode>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var nodeName in demandNodes)
            {
                var nodeItem = context.MdcBiz.Nodes.FirstOrDefault(m => m.NodeName == nodeName);
                if (nodeItem == null) continue;
                keyVals.TryAdd(nodeItem.BlockNodeId, nodeItem);
            }*/
            var keyVals = keyBizNodes;
            //3.读取scada的节点数据
            //var kvs = GetKeyValues(keyVals.Keys.ToArray(), context);
            //var kvs = RNode.Instance.GetKeyValues(keyVals.Keys.ToArray());
            var kvs = RouteRpcApi.GetRemoteNodes(keyVals.Keys.ToArray());

            if (kvs == null) return null;

            //4.拼接返回值
            var result = new List<(string key, string val)>();
            foreach (var key in keyVals.Keys)
            {
                var item = kvs.FirstOrDefault(m => m.Key == key);
                if (item == null) continue;
                var nodeItem = keyVals[key];

                //做测试使用
                if (nodeItem.NodeName == "CV403_SensorData")
                {
                    //Crn01R_CraneRow
                    //var val = new Random().Next(1, 70);
                    ////val = val >= 70 ? 0 : val;
                    //result.Add((key: nodeItem.NodeName, val: val.ToString()));
                    //CV323_TaskNo_1
                    //result.Add((key: nodeItem.NodeName, val: "323"));
                    //continue;
                }

                result.Add((key: nodeItem.NodeName, val: item.Value));
            }

            return result;
        }

        private static SafeRun.CtrlBiz.SpiService.BizContext GetBizContext(string appId)
        {
            //var cs = getCtrlService().GetMdcBizs(XApp.Current.AppId);

            var cs = getCtrlService().GetMdcBizs(appId);

            SafeRun.CtrlBiz.SpiService.BizContext context = null;
            foreach (var c in cs)
            {
                //BizContextStore.Instance.AddContext(new BizContext() { MdcBiz = c });
                context = new SafeRun.CtrlBiz.SpiService.BizContext() { MdcBiz = c };
            }

            return context;
        }


        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="XException"></exception>
        private static BizNodeKeyValue[] GetKeyValues(string[] keys, SafeRun.CtrlBiz.SpiService.BizContext context)
        {
            var kvs = RNode.Instance.GetKeyValues(keys);
            var result = new List<BizNodeKeyValue>();
            foreach (var node in context.MdcBiz.Nodes)
            {
                var key = node.BlockNodeId.ToLower();
                var kv = kvs.Where(kv => kv.Key.ToLower() == key).FirstOrDefault();
                var isNull = (kv == null || string.IsNullOrWhiteSpace(kv.Value));
                var expired = 0;
                if (!isNull)
                {
                    expired = kv.IsExpired;
                }
                if (isNull || expired > 0)
                {
                    if (node.IsNullAble == 0)
                    {
                        throw new XException($"设备数据读取失败[{node.BlockNodeId}][IsNull={isNull}][Expired={expired}]");
                    }
                    kv = new() { Key = key, Value = node.DefaultValue };
                }
                result.Add(new BizNodeKeyValue()
                {
                    BizNodeId = node.BizNodeId,
                    BlockNodeId = node.BlockNodeId,
                    Value = kv.Value,
                    Time = kv.Time,
                    IsExpired = kv.IsExpired,
                });
            }
            return result.ToArray();
        }

        #endregion

        #region 委托操作
        private static TaskCompleteService GetTaskCompleteApi()
        {
            //return RpcHelper.Create<TaskCompleteService>();//rpc调用其他服务API接口
            return RouteRpcApi.CtrlEquipTasksApi();//rpc调用其他服务API接口
        }

        private static TaskService GetTaskQueryApi()
        {
            //return RpcHelper.Create<TaskService>();//rpc调用其他服务API接口
            return RouteRpcApi.WMSTaskApi();
        }

        private static StockLocStatusService GetStockLocStatusApi()
        {
            //return RpcHelper.Create<TaskService>();//rpc调用其他服务API接口
            return RouteRpcApi.WMSStockLocStatusApi();
        }

        /// <summary>
        /// 更新节点值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<(string key, string val)> SetUpdNodes(Dictionary<string, string> param)
        {
            var resultSet = new List<(string key, string val)>();
            foreach (var item in param)
            {
                var key = item.Key;
                var val = item.Value;
                if (string.IsNullOrEmpty(key)) continue;
                //1.获取节点
                //var nodeItem = keyBizNodes.FirstOrDefault(m => m.Key == key);
                var nodeItem = keyBizNodes.Values.FirstOrDefault(m => m.NodeName == key);
                if (nodeItem.Equals(default(KeyValuePair<string, BizNode>))) continue;
                //2.设置节点值
                var result = SetPlcNodeVal(nodeItem.BlockNodeId, val);
                if (result)
                {
                    resultSet.Add(new("UpdResult", "true"));
                }
                else
                {
                    resultSet.Add(new("Error", "false"));
                }


            }

            return resultSet;
        }
        private static bool SetPlcNodeVal(string blockNodeId, string nodeVal)
        {
            var result = getCtrlService().SetKeyValues(new KeyValue[] { new KeyValue() { Key = blockNodeId, Value = nodeVal } });
            return result > 0;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<(string key, string val)> SetDeleteAction(Dictionary<string, string> param)
        {
            var taskCompServiceApi = GetTaskCompleteApi();
            var taskNoStr = param["TaskNo"] ?? "";
            if (string.IsNullOrEmpty(taskNoStr) || taskNoStr.Trim() == "0")
            {
                return new List<(string key, string val)>() { ("Error", "Task No 任务号为空,不允许处理!!!") };
            }
            var taskNo = int.Parse(taskNoStr);
            if (taskNo <= 0)
            {
                return new List<(string key, string val)>() { ("Error", "Task No 任务号错误,查询失败!!!") };
            }
            //var result = taskCompServiceApi.DeleteTask(taskNo);

            //return new List<(string key, string val)>() { ("Result", result) };
            return new List<(string key, string val)>() { ("Result", "false") };
        }

        /// <summary>
        /// 强制任务完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<(string key, string val)> SetCompleteAction(Dictionary<string, string> param)
        {
            var taskCompServiceApi = GetTaskCompleteApi();

            var taskNoStr = param["TaskNo"] ?? "";
            var contrType = param["ContrType"] ?? "";
            if (string.IsNullOrEmpty(taskNoStr) || taskNoStr.Trim() == "0")
            {
                return new List<(string key, string val)>() { ("Error", "Task No 任务号为空,不予处理!!!") };
            }

            if (string.IsNullOrEmpty(contrType))
            {
                return new List<(string key, string val)>() { ("Error", "设备类型为空,不予处理!!!") };
            }

            if (!(contrType == "CRN" || contrType == "CV"))
            {
                return new List<(string key, string val)>() { ("Error", "设备非可操作类型,不予处理!!!") };
            }
            var taskNo = int.Parse(taskNoStr);
            if (taskNo <= 0)
            {
                return new List<(string key, string val)>() { ("Error", "Task No 任务号错误,查询失败!!!") };
            }


            if (contrType == "CV")
            {
                var result = taskCompServiceApi.UpDataCvTaskComp("", taskNo);
                var resultStr = result == 1 ? "成功" : "失败";
                return new List<(string key, string val)>() { ("Result", resultStr) };
            }
            else if (contrType == "CRN")
            {
                var result = taskCompServiceApi.UpDataCrnTaskComp(taskNo);
                var resultStr = result == 1 ? "成功" : "失败";
                return new List<(string key, string val)>() { ("Result", resultStr) };
            }
            else
            {
                return new List<(string key, string val)>() { ("Result", "无效处理") };
            }

            //return new List<(string key, string val)>() { ("Result", result) };
        }

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public static List<(string key, string val)> QueryAction(Dictionary<string, string> queryParam)
        {
            var wcsTask = new WCSTask();
            var queryTaskApi = GetTaskQueryApi();
            //var test = string.Empty;
            //test.RpcDecode(typeof(object));

            //queryTaskApi.GetWCSTasks(queryParam);
            //List<WCSTask> resultTask = queryTaskApi.GetWCSTasks(wcsTask);

            foreach (var key in queryParam.Keys)
            {
                if (key == "TaskNo")
                {
                    var val = queryParam[key];
                    wcsTask.TaskNo = string.IsNullOrEmpty(val) ? 0 : int.Parse(val);
                }
            }

            if (wcsTask.TaskNo <= 0)
            {
                return new List<(string key, string val)>() { ("Error", "Task No 任务号错误,查询失败!!!") };
            }

            //wcsTask.TaskNo = 10248;
            var resultDetail = queryTaskApi.GetTaskDetail(wcsTask);

            if (resultDetail == null) return null;
            var resultVal = new List<(string key, string val)>();
            resultVal.Add(("TaskNo", resultDetail.TaskNo.ToString()));
            resultVal.Add(("SLocNo", resultDetail.SLocNo ?? "".ToString()));
            resultVal.Add(("DLocNo", resultDetail.DLocNo ?? "".ToString()));
            resultVal.Add(("TaskType", resultDetail.TaskType ?? "".ToString()));
            resultVal.Add(("IOType", resultDetail.IOType ?? "".ToString()));
            resultVal.Add(("TaskTypeName", resultDetail.TaskTypeName ?? "".ToString()));
            return resultVal;
        }


        /// <summary>
        /// 查询堆垛机库存
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public static List<(string key, string val)> QueryCrnStockAction(Dictionary<string, string> queryParam)
        {
            var queryItem = new CommonQuery();
            var queryApi = GetStockLocStatusApi();

            foreach (var key in queryParam.Keys)
            {
                if (key == "CrnNo")
                {
                    var val = queryParam[key];
                    queryItem.CrnNo = val;
                }
            }

            if (string.IsNullOrEmpty(queryItem.CrnNo))
            {
                return new List<(string key, string val)>() { ("Error", "查询堆垛机号不存在!!!") };
            }

            var resultDetails = queryApi.GetUtilization(queryItem);
            var resultDetail = resultDetails.FirstOrDefault(m => m.CrnNo == queryItem.CrnNo);
            if (resultDetail == null) return null;
            var resultVal = new List<(string key, string val)>();
            resultVal.Add(("CrnNo", resultDetail.CrnNo.ToString()));
            resultVal.Add(("EmptyNum", (resultDetail.EmptyNum ?? 0).ToString()));//空库位数
            resultVal.Add(("MaterNum", (resultDetail.MaterNum ?? 0).ToString()));//有货位数
            resultVal.Add(("PalletNum", (resultDetail.PalletNum ?? 0).ToString()));//空托库位数
            resultVal.Add(("Ratio", resultDetail.ratio));//库位使用率

            return resultVal;
        }


        #endregion

    }

    #region 自定义值变更事件和对象
    // 自定义 EventArgs 类
    /// <summary>
    /// 值变更事件
    /// </summary>
    public class ValueChangedEventArgs : EventArgs
    {
        public string NewValue { get; }

        public ValueChangedEventArgs(string newValue)
        {
            NewValue = newValue;
        }
    }

    /// <summary>
    /// 值变更对象
    /// </summary>
    public class ObservableEvent
    {
        public string ContrName { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;

        private string _nodeVal = string.Empty;
        public string NodeVal
        {
            get => _nodeVal; set
            {
                if (_nodeVal != value)
                {
                    _nodeVal = value;
                    // 触发事件，通知所有订阅者
                    //OnValueChanged(new ValueChangedEventArgs(value));
                    OnValueChanged(new ValueChangedEventArgs(value));
                }
            }
        }

        // 声明事件，登记多个事件
        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            // 检查是否有订阅者，然后触发事件
            ValueChanged?.Invoke(this, e);
        }
    }
    #endregion

}
